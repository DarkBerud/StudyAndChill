using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyAndChill.API.Data;
using StudyAndChill.API.Dtos;
using StudyAndChill.API.Models;

namespace StudyAndChill.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TeachersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TeachersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTeachers()
        {
            var teachers = await _context.Teachers.ToListAsync();

            return Ok(teachers);
        }

        [HttpPost]
        public async Task<IActionResult> AddTeacher([FromBody] CreateTeacherDto createTeacherDto)
        {
            var newTeacher = new Teacher
            {
                Name = createTeacherDto.Name,
                Subject = createTeacherDto.Subject
            };

            _context.Teachers.Add(newTeacher);

            await _context.SaveChangesAsync();

            return Ok(newTeacher);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeacher(int id, [FromBody] CreateTeacherDto updateDto)
        {
            var teacher = await _context.Teachers.FindAsync(id);

            if (teacher == null)
            {
                return NotFound();
            }

            teacher.Name = updateDto.Name;
            teacher.Subject = updateDto.Subject;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);

            if (teacher == null)
            {
                return NotFound();
            }

            _context.Teachers.Remove(teacher);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}