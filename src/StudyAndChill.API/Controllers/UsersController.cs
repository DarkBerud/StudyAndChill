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
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request)
        {
            if (!Enum.TryParse<UserRole>(request.Role, true, out UserRole userRole))
            {
                return BadRequest("Tipo de usuário inválido");
            }

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                {
                    return BadRequest("Já existe um usuário registrado neste e-mail.");
                }

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Role = userRole,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var invitationToken = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));

            var invitation = new UserInvitation
            {
                UserId = user.Id,
                Token = invitationToken,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsUsed = false
            };

            _context.UserInvitations.Add(invitation);
            await _context.SaveChangesAsync();

            var invitationLink = $"https://studyandchill.com/set-password?token={invitationToken}";
            Console.WriteLine($"Invitation link for {user.Email}: {invitationLink}");

            return Ok(new { user.Id, user.Name, user.Email, user.Role });
        }
    }
}