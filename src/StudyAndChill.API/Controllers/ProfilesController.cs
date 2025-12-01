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
using StudyAndChill.API.Enums;
using StudyAndChill.API.Models;
using StudyAndChill.API.Services;

namespace StudyAndChill.API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ProfilesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAsaasService _asaasService;

        public ProfilesController(AppDbContext context, IAsaasService asaasService)
        {
            _context = context;
            _asaasService = asaasService;
        }

        [HttpPut("teacher")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UpdateTeacherProfile([FromBody] UpdateTeacherProfileDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) return Unauthorized();
            var userId = int.Parse(userIdString);

            bool hasPix = !string.IsNullOrEmpty(dto.PixKey);
            bool hasBankAccount = !string.IsNullOrEmpty(dto.BankCode) &&
                                    !string.IsNullOrEmpty(dto.Agency) &&
                                    !string.IsNullOrEmpty(dto.AccountNumber);

            if (!hasPix && !hasBankAccount)
            {
                return BadRequest("É obrigatório informar pelo menos uma das formas de recebimento");
            }

            var profile = await _context.TeacherProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                profile = new TeacherProfile { UserId = userId };
                _context.TeacherProfiles.Add(profile);
            }

            profile.Phone = dto.Phone;
            profile.DocumentType = dto.DocumentType;
            profile.DocumentNumber = dto.DocumentNumber;

            profile.Address.Country = dto.Address.Country;
            profile.Address.PostalCode = dto.Address.PostalCode;
            profile.Address.Street = dto.Address.Street;
            profile.Address.Number = dto.Address.Number;
            profile.Address.Complement = dto.Address.Complement;
            profile.Address.Neighborhood = dto.Address.Neighborhood;
            profile.Address.City = dto.Address.City;
            profile.Address.State = dto.Address.State;

            profile.PixKey = dto.PixKey;
            profile.PixKeyType = dto.PixKeyType;
            profile.BankCode = dto.BankCode;
            profile.Agency = dto.Agency;
            profile.AccountNumber = dto.AccountNumber;

            await _context.SaveChangesAsync();

            return Ok("Perfil atualizado com sucesso");
        }

        [HttpPut("student")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UpdateStudentProfile([FromBody] UpdateStudentProfileDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) return Unauthorized();
            var userId = int.Parse(userIdString);

            var profile = await _context.StudentProfiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return NotFound("Usuário não encontrado");

                profile = new StudentProfile { UserId = userId, User = user };
                _context.StudentProfiles.Add(profile);
            }

            profile.Phone = dto.Phone;
            profile.DocumentType = dto.DocumentType;
            profile.DocumentNumber = dto.DocumentNumber;

            profile.Address.Country = dto.Address.Country;
            profile.Address.PostalCode = dto.Address.PostalCode;
            profile.Address.Street = dto.Address.Street;
            profile.Address.Number = dto.Address.Number;
            profile.Address.Complement = dto.Address.Complement;
            profile.Address.Neighborhood = dto.Address.Neighborhood;
            profile.Address.City = dto.Address.City;
            profile.Address.State = dto.Address.State;

            if (profile.DocumentType == DocumentType.CPF &&
                !string.IsNullOrEmpty(profile.DocumentNumber) &&
                string.IsNullOrEmpty(profile.AsaasCustomerId))
            {
                var asaasId = await _asaasService.CreateCustomer(profile.User, profile);

                if (!string.IsNullOrEmpty(asaasId))
                {
                    profile.AsaasCustomerId = asaasId;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
               Message = "Perfil atualizado com sucesso.",
               IntegratedWithAsaas = !string.IsNullOrEmpty(profile.AsaasCustomerId)
            });

        }

    }
}