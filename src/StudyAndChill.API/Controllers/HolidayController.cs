using System;
using System.Collections.Generic;
using System.Linq;
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
    [Authorize(Roles = "Admin")]
    public class HolidayController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HolidayController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateHoliday([FromBody] CreateHolidayDto dto)
        {
            if (await _context.Holidays.AnyAsync(h => h.Date == dto.Date))
            {
                return BadRequest("Já existe um feriado programado para esta data");
            }

            var holiday = new Holiday
            {
              Name = dto.Name,
              Date = dto.Date,
              IsGlobal = dto.IsGlobal  
            };

            if (!dto.IsGlobal && dto.TargetUserIds != null && dto.TargetUserIds.Any())
            {
                var users = await _context.Users
                    .Where(u => dto.TargetUserIds.Contains(u.Id))
                    .ToListAsync();
                
                if (!users.Any())
                {
                    return BadRequest("Nenhum usuário selecionado foi encontrado");
                }

                holiday.AffectedUsers = users;
            }

            _context.Holidays.Add(holiday);
            await _context.SaveChangesAsync();

            if (dto.AutoReschedule)
            {
                await ProcessAutomaticRescheduling(holiday);
            }

            return Ok(new {holiday.Id, holiday.Name, holiday.Date, holiday.IsGlobal, AffectedUsersCount = holiday.AffectedUsers.Count});
        }

        [HttpGet]
        public async Task<IActionResult> GetHolidays()
        {
            var holidays = await _context.Holidays
                .OrderBy(h => h.Date)
                .Select(h => new
                {
                    h.Id,
                    h.Name,
                    h.Date,
                    h.IsGlobal,
                    SpecificTargets = h.IsGlobal ? 0 : h.AffectedUsers.Count
                })
                .ToListAsync();
            return Ok(holidays);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHoliday(int id)
        {
            var holiday = await _context.Holidays.FindAsync(id);
            if (holiday == null) return NotFound();

            _context.Holidays.Remove(holiday);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task ProcessAutomaticRescheduling(Holiday holiday)
        {
            var affectedSessions = await _context.ClassSessions
                .Include(cs => cs.Contract)
                .Where(cs => cs.StartDate.Date == holiday.Date.ToDateTime(TimeOnly.MinValue).Date &&
                                cs.Status == ClassStatus.Scheduled)
                .ToListAsync();
            foreach (var session in affectedSessions)
            {
                var teacherWantsWork = await _context.TeacherHolidayWorks
                    .AnyAsync(w => w.TeacherId == session.TeacherId && w.Date == holiday.Date);

                if (teacherWantsWork)
                {
                    continue;
                }

                if (!holiday.IsGlobal)
                {
                    var isAffected = await _context.Holidays
                        .Where(h => h.Id == holiday.Id)
                        .AnyAsync(h => h.AffectedUsers.Any(u => u.Id == session.TeacherId || 
                                                                session.Students.Any(s => s.Id == u.Id)));
                }

                var currentContractEnd = session.Contract.EndDate.ToDateTime(TimeOnly.MinValue);
                var potentialNewDate = currentContractEnd.AddDays(1);

                while (potentialNewDate.DayOfWeek != session.StartDate.DayOfWeek)
                {
                    potentialNewDate = potentialNewDate.AddDays(1);
                }

                var newStartDateTime = potentialNewDate.Date.Add(session.StartDate.TimeOfDay);
                var duration = session.EndDate - session.StartDate;

                session.StartDate = newStartDateTime;
                session.EndDate = newStartDateTime.Add(duration);

                if (DateOnly.FromDateTime(newStartDateTime) > session.Contract.EndDate)
                {
                    session.Contract.EndDate = DateOnly.FromDateTime(newStartDateTime);
                }
            }

            if (affectedSessions.Any())
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}