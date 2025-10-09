using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyAndChill.API.Data;
using StudyAndChill.API.Dtos;
using StudyAndChill.API.Models;

namespace StudyAndChill.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeacherAvailabilityController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TeacherAvailabilityController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CreateAvailability([FromBody] List<CreateAvailabilityDto> dtos)
        {
            var teacherIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (teacherIdString == null)
            {
                return Unauthorized();
            }

            var teacherId = int.Parse(teacherIdString);

            var newAvailabilities = new List<TeacherAvailability>();

            foreach (var i in dtos)
            {
                var availability = new TeacherAvailability
                {
                    DayOfWeek = i.DayOfWeek,
                    AvailableFrom = i.AvailableFrom,
                    AvailableTo = i.AvailableTo,
                    TeacherId = teacherId
                };
                newAvailabilities.Add(availability);
            }

            await _context.TeacherAvailabilities.AddRangeAsync(newAvailabilities);

            await _context.SaveChangesAsync();

            return Ok("Disponibilidade salva com sucesso");
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetMyAvailability()
        {
            var teacherIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (teacherIdString == null)
            {
                return Unauthorized();
            }

            var teacherId = int.Parse(teacherIdString);

            var Availabilities = await _context.TeacherAvailabilities
                .Where(a => a.TeacherId == teacherId)
                .ToListAsync();

            return Ok(Availabilities);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteAvailability(int id)
        {

            var teacherIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (teacherIdString == null)
            {
                return Unauthorized();
            }

            var teacherId = int.Parse(teacherIdString);

            var availability = await _context.TeacherAvailabilities.FirstOrDefaultAsync(a => a.Id == id && a.TeacherId == teacherId);

            if (availability == null)
            {
                return NotFound();
            }

            _context.TeacherAvailabilities.Remove(availability);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UpdateAvailability(int id, [FromBody] CreateAvailabilityDto dto)
        {
            var teacherIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (teacherIdString == null)
            {
                return Unauthorized();
            }

            var teacherId = int.Parse(teacherIdString);

            var availability = await _context.TeacherAvailabilities.FirstOrDefaultAsync(a => a.Id == id && a.TeacherId == teacherId);

            if (availability == null)
            {
                return NotFound();
            }

            availability.AvailableFrom = dto.AvailableFrom;
            availability.AvailableTo = dto.AvailableTo;
            availability.DayOfWeek = dto.DayOfWeek;

            
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}