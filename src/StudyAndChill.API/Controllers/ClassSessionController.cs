using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using StudyAndChill.API.Data;
using StudyAndChill.API.Dtos;
using StudyAndChill.API.Enums;
using StudyAndChill.API.Models;
using StudyAndChill.API.Services;

namespace StudyAndChill.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassSessionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHolidayService _holidayService;

        public ClassSessionController(AppDbContext context, IHolidayService holidayService)
        {
            _context = context;
            _holidayService = holidayService;
        }

        [HttpPost("schedule")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleDto dto)
        {
            var student = await _context.Users.FindAsync(dto.StudentId);
            if (student == null || student.Role != UserRole.Student)
            {
                return BadRequest("Estudande não encontrado");
            }

            var teacherExists = await _context.Users.AnyAsync(u => u.Id == dto.TeacherId && u.Role == UserRole.Teacher);
            if (!teacherExists)
            {
                return BadRequest("Professor não encontrado");
            }

            var contract = await _context.Contracts.FindAsync(dto.ContractId);

            if (contract == null)
            {
                return BadRequest("Contrato não encontrado");
            }

            if (contract.StudentId != dto.StudentId || contract.TeacherId != dto.TeacherId)
            {
                return BadRequest("O contrato não pertence ao aluno ou professor especificado");
            }

            if (contract.Status != ContractStatus.Active)
            {
                return BadRequest("O contrato não está ativo");
            }

            if (dto.EndDate > contract.EndDate)
            {
                return BadRequest("A data final do horário excede a data final do contrato");
            }

            if (dto.DurationInMinutes != contract.ClassDuration)
            {
                return BadRequest("A duração da aula especificada não corresponde à duração definida no contrato");
            }

            var classDates = new List<DateTime>();
            var startDate = DateTime.UtcNow.Date;

            while (startDate <= dto.EndDate.ToDateTime(TimeOnly.MinValue))
            {
                if (startDate.DayOfWeek == dto.DayOfWeek)
                {
                    classDates.Add(startDate.Add(dto.ClassTime.ToTimeSpan()));
                }
                startDate = startDate.AddDays(1);
            }

            var newSessions = new List<ClassSession>();

            foreach (var date in classDates)
            {
                if (await _holidayService.IsHolidayAsync(date))
                {
                    continue;
                }

                var time = TimeOnly.FromDateTime(date);

                var isAvailable = await _context.TeacherAvailabilities.AnyAsync(a =>
                a.TeacherId == dto.TeacherId &&
                a.DayOfWeek == date.DayOfWeek &&
                a.AvailableFrom <= time &&
                a.AvailableTo > time &&
                (a.Type == Enums.AvailabilityType.Regular || a.Type == Enums.AvailabilityType.Both)
                );

                if (!isAvailable)
                {
                    continue;
                }

                var isAlreadyBooked = await _context.ClassSessions.AnyAsync(cs =>
                cs.TeacherId == dto.TeacherId &&
                cs.StartDate == date
                );

                if (isAlreadyBooked)
                {
                    continue;
                }

                var classSession = new ClassSession
                {
                    TeacherId = dto.TeacherId,
                    StartDate = date,
                    Status = ClassStatus.Scheduled,
                    EndDate = date.AddMinutes((int)dto.DurationInMinutes),
                    ContractId = dto.ContractId
                };

                classSession.Students.Add(student);

                newSessions.Add(classSession);
            }

            if (newSessions.Any())
            {
                await _context.ClassSessions.AddRangeAsync(newSessions);
                await _context.SaveChangesAsync();
            }

            contract.MakeUpQuota = (int)Math.Floor(newSessions.Count * 0.25);


            await _context.SaveChangesAsync();


            return Ok($"{newSessions.Count} aulas foram marcadas com sucesso");
        }

        [HttpPost("makeup")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> BookMakeUpClass([FromBody] BookClassDto dto)
        {
            var studentIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (studentIdString == null)
            {
                return Unauthorized();
            }
            var studentId = int.Parse(studentIdString);

            var hasGroupClasses = await _context.ClassSessions
                .Where(cs => cs.Status == ClassStatus.Scheduled && cs.Students.Any(s => s.Id == studentId))
                .Include(cs => cs.Students)
                .AnyAsync(cs => cs.Students.Count > 1);

            if (hasGroupClasses)
            {
                return BadRequest("Alunos com aulas em grupo não podem marcar reposições individualmente. Fale com o seu professor.");
            }

            if (dto.StartTime < DateTime.UtcNow.AddDays(5))
            {
                return BadRequest("O agendamento deve ser feito com pelo menos 5 dias de antecedência");
            }

            var activeContract = await _context.Contracts.FirstOrDefaultAsync(c => c.StudentId == studentId && c.Status == ContractStatus.Active);
            if (activeContract == null || !activeContract.CanBookMakeUpClasses || activeContract.MakeUpQuota <= 0)
            {
                return BadRequest("Nenhum contrato ativo encontrado ou marcação de reposições indisponíveis.");
            }

            var authorizingSession = await _context.ClassSessions
                .FirstOrDefaultAsync(cs => cs.ContractId == activeContract.Id &&
                                        cs.Students.Any(s => s.Id == studentId) &&
                                        cs.Status == ClassStatus.MissedCanMakeUp &&
                                        cs.MakeUpExpiryDate.HasValue &&
                                        cs.MakeUpExpiryDate.Value >= DateTime.UtcNow);

            if (authorizingSession == null)
            {
                return BadRequest("Nenhuma aula de reposição encontrada");
            }

            var requestTime = TimeOnly.FromDateTime(dto.StartTime);
            var requestDay = dto.StartTime.DayOfWeek;

            var isAvailable = await _context.TeacherAvailabilities.AnyAsync(a =>
            a.TeacherId == activeContract.TeacherId &&
            a.DayOfWeek == requestDay &&
            a.AvailableFrom <= requestTime &&
            a.AvailableTo > requestTime &&
            (a.Type == Enums.AvailabilityType.MakeUp || a.Type == Enums.AvailabilityType.Both)
            );

            if (!isAvailable)
            {
                return BadRequest("Horário indisponivel");
            }

            var isAlreadyBooked = await _context.ClassSessions.AnyAsync(cs =>
            cs.TeacherId == activeContract.TeacherId &&
            cs.StartDate == dto.StartTime
            );

            if (isAlreadyBooked)
            {
                return BadRequest("Este horário já foi agendado");
            }

            var student = await _context.Users.FindAsync(studentId);
            if (student == null)
            {
                return NotFound("Aluno não encontrado");
            }

            var classSession = new ClassSession
            {
                StartDate = dto.StartTime,
                TeacherId = activeContract.TeacherId,
                EndDate = dto.StartTime.AddMinutes((int)dto.Duration),
                Status = ClassStatus.Scheduled,
                Students = new List<User> { student }
            };

            _context.ClassSessions.Add(classSession);
            activeContract.MakeUpQuota -= 1;

            if (authorizingSession != null)
            {
                authorizingSession.Status = ClassStatus.Reposed;
                authorizingSession.MakeUpExpiryDate = null;
            }

            await _context.SaveChangesAsync();

            return Ok("Aula de reposição agendada com sucesso!");
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin, Teacher")]
        public async Task<IActionResult> UpdateClassStatus(int id, [FromBody] UpdateClassStatusDto dto)
        {
            var classSession = await _context.ClassSessions.FindAsync(id);

            if (classSession == null)
            {
                return NotFound("Aula não encontrada");
            }

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdString == null || userRole == null)
            {
                return Unauthorized();
            }

            if (userRole == UserRole.Teacher.ToString())
            {
                var teacherId = int.Parse(userIdString);
                if (classSession.TeacherId != teacherId)
                {
                    return Forbid("Não é possivel atualizar esta aula.");
                }
            }

            if (classSession.StartDate > DateTime.UtcNow)
            {
                return BadRequest("Não é possível atualizar o status de uma aula que ainda não ocorreu.");
            }

            if (dto.NewStatus != ClassStatus.Completed &&
                dto.NewStatus != ClassStatus.AbsentNoMakeUp &&
                dto.NewStatus != ClassStatus.MissedCanMakeUp &&
                dto.NewStatus != ClassStatus.Reposed)
            {
                return BadRequest("Status inválido");
            }

            classSession.Status = dto.NewStatus;

            if (dto.NewStatus == ClassStatus.MissedCanMakeUp)
            {
                classSession.MakeUpExpiryDate = classSession.StartDate.AddMonths(1);
            }
            else
            {
                classSession.MakeUpExpiryDate = null;
            }

            await _context.SaveChangesAsync();

            return Ok("Status da aula atualizado com sucesso.");

        }

        [HttpPut("{id}/reschedule")]
        [Authorize(Roles = "Admin, Teacher")]
        public async Task<IActionResult> RescheduleClass(int id, [FromBody] RescheduleClassDto dto)
        {
            var classSession = await _context.ClassSessions.FindAsync(id);
            if (classSession == null)
            {
                return NotFound("Aula não encontrada");
            }

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdString == null || userRole == null)
            {
                return Unauthorized();
            }

            if (userRole == UserRole.Teacher.ToString())
            {
                var teacherId = int.Parse(userIdString);
                if (classSession.TeacherId != teacherId)
                {
                    return Forbid("Professores só podem reagendar as suas próprias aulas.");
                }
            }

            if (classSession.Status != ClassStatus.Scheduled)
            {
                return BadRequest($"A aula não pode ser reagendade. Status atual: {classSession.Status}. o reagendamento só é permitido para aulas 'Scheduled'.");
            }

            if (dto.NewStartTime <= DateTime.UtcNow.AddMinutes(5))
            {
                return BadRequest("Não é possivel marcar uma aula neste horário, por favor escolha uma data mais adiante");
            }

            var duration = classSession.EndDate - classSession.StartDate;
            var newEndTime = dto.NewStartTime.Add(duration);
            var newRequestedTime = TimeOnly.FromDateTime(dto.NewStartTime);
            var newRequestedDay = dto.NewStartTime.DayOfWeek;

            var isAvailable = await _context.TeacherAvailabilities.AnyAsync(a =>
            a.TeacherId == classSession.TeacherId &&
            a.DayOfWeek == newRequestedDay &&
            a.AvailableFrom <= newRequestedTime &&
            a.AvailableTo >= newRequestedTime &&
            (a.Type == Enums.AvailabilityType.Regular || a.Type == Enums.AvailabilityType.Both));

            if (!isAvailable)
            {
                return BadRequest("Não há disponibilidade do professor para o novo horário de aulas regulares");
            }

            var isAlreadyBooked = await _context.ClassSessions.AnyAsync(cs =>
            cs.Id != id &&
            cs.TeacherId == classSession.TeacherId &&
            (
                dto.NewStartTime < cs.EndDate && newEndTime > cs.StartDate
            ));

            if (isAlreadyBooked)
            {
                return BadRequest("O professor já tem um aula agendada neste horário");
            }

            classSession.StartDate = dto.NewStartTime;
            classSession.EndDate = newEndTime;

            await _context.SaveChangesAsync();

            return Ok("Aula reagendade com sucesso!");

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetClasses([FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] int? teacherId = null, [FromQuery] int? studentId = null)
        {
            start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
            end = DateTime.SpecifyKind(end, DateTimeKind.Utc);

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoleString = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdString == null || userRoleString == null) return Unauthorized();

            var currentUserId = int.Parse(userIdString);
            var currentUserRole = Enum.Parse<UserRole>(userRoleString);

            var query = _context.ClassSessions
                .Include(cs => cs.Teacher)
                .Include(cs => cs.Students)
                .Where(cs => cs.StartDate >= start && cs.EndDate <= end)
                .AsQueryable();

            switch (currentUserRole)
            {
                case UserRole.Student:
                    query = query.Where(cs => cs.Students.Any(s => s.Id == currentUserId));
                    break;
                case UserRole.Teacher:
                    query = query.Where(cs => cs.TeacherId == currentUserId);
                    break;
                case UserRole.Admin:
                    if (teacherId.HasValue)
                        query = query.Where(cs => cs.TeacherId == currentUserId);

                    if (studentId.HasValue)
                        query = query.Where(cs => cs.Students.Any(s => s.Id == currentUserId));
                    break;
            }

            var sessions = await query
                .OrderBy(cs => cs.StartDate)
                .Select(cs => new ClassSessionDto
                {
                    Id = cs.Id,
                    Start = cs.StartDate,
                    End = cs.EndDate,
                    Status = cs.Status,
                    TeacherId = cs.TeacherId,
                    TeacherName = cs.Teacher.Name,
                    Students = cs.Students.Select(s => new StudentSummaryDto
                    {
                        Id = s.Id,
                        Name = s.Name
                    }).ToList()
                }).ToListAsync();

            return Ok(sessions);
        }

    }
}