using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyAndChill.API.Data;
using StudyAndChill.API.Dtos;
using StudyAndChill.API.Enums;
using StudyAndChill.API.Models;

namespace StudyAndChill.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassSessionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClassSessionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
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
                    EndDate = date.AddMinutes((int)dto.DurationInMinutes)
                };

                classSession.Students.Add(student);

                newSessions.Add(classSession);
            }

            if (newSessions.Any())
                {
                    await _context.ClassSessions.AddRangeAsync(newSessions);
                    await _context.SaveChangesAsync();
                }

            return Ok($"{newSessions.Count} aulas foram marcadas com sucesso");
        }

    }
}