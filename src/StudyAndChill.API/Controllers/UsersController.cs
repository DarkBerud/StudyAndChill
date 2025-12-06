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
using StudyAndChill.API.Services;

namespace StudyAndChill.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IAsaasService _asaasService;

        public UsersController(AppDbContext context, IConfiguration configuration, IAsaasService asaasService)
        {
            _context = context;
            _configuration = configuration;
            _asaasService = asaasService;
        }

        [Authorize(Roles = "Admin")]
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

            if (user.Role == UserRole.Student)
            {
                var studentProfile = new StudentProfile
                {
                    User = user,

                };
                _context.StudentProfiles.Add(studentProfile);
            }
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

        [HttpPost("student-complete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFullStudent([FromBody] CreateFullStudentDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return BadRequest("Já existe um usuário registrado com este e-mail.");
            }

            var teacherExists = await _context.Users.AnyAsync(u => u.Id == dto.TeacherId && u.Role == UserRole.Teacher);
            if (!teacherExists)
            {
                return BadRequest("Professor não encontrado");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = new User
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Role = UserRole.Student,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();


                var profile = new StudentProfile
                {
                    UserId = user.Id,
                    Phone = dto.Phone,
                    DocumentType = dto.DocumentType,
                    DocumentNumber = dto.DocumentNumber,
                    Address = new Models.Address
                    {
                        Country = dto.Address.Country,
                        PostalCode = dto.Address.PostalCode,
                        Street = dto.Address.Street,
                        Number = dto.Address.Number,
                        Complement = dto.Address.Complement,
                        Neighborhood = dto.Address.Neighborhood,
                        City = dto.Address.City,
                        State = dto.Address.State

                    }
                };

                if (dto.DocumentType == DocumentType.CPF || dto.DocumentType == DocumentType.CNPJ)
                {
                    var asaasId = await _asaasService.CreateCustomer(user, profile);

                    if (string.IsNullOrEmpty(asaasId))
                    {
                        await transaction.RollbackAsync();
                        return BadRequest("Falha ao criar cliente no Asaas. Verifique os dados (CPF/Telefone) e tente novamente.");
                    }
                    profile.AsaasCustomerId = asaasId;
                }

                _context.StudentProfiles.Add(profile);
                await _context.SaveChangesAsync();

                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var firstDueDate = new DateOnly(today.Year, today.Month, dto.DueDay);

                if (firstDueDate < today)
                {
                    firstDueDate = firstDueDate.AddMonths(1);
                }

                string? subscriptionId = null;

                if (!string.IsNullOrEmpty(profile.AsaasCustomerId))
                {
                    subscriptionId = await _asaasService.CreateSubscription(
                        profile.AsaasCustomerId,
                        dto.MonthlyAmount,
                        firstDueDate,
                        dto.EndDate
                    );
                }

                var contract = new Contract
                {
                    StudentId = user.Id,
                    TeacherId = dto.TeacherId,
                    Type = dto.ContractType,
                    ClassDuration = dto.ClassDuration,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    Status = ContractStatus.Active,

                    MonthlyAmount = dto.MonthlyAmount,
                    TeacherPaymentShare = dto.TeacherPaymentShare,
                    DueDay = dto.DueDay,

                    AsaasSubscriptionId = subscriptionId,

                    MakeUpQuota = 0,
                    CanBookMakeUpClasses = false
                };
                _context.Contracts.Add(contract);
                await _context.SaveChangesAsync();

                var invitationToken = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
                var invitation = new UserInvitation
                {
                    UserId = user.Id,
                    Token = invitationToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    IsUsed = false
                };
                _context.UserInvitations.Add(invitation);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var invitationLink = $"https://studyandchill.com/set-password?token={invitationToken}";
                Console.WriteLine($"Convite gerado para {user.Email}: {invitationLink}");

                return Ok(new
                {
                    UserId = user.Id,
                    ContractId = contract.Id,
                    AsaasId = profile.AsaasCustomerId,
                    message = "Aluno criado com sucesso"
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Erro interno ao processar o cadastro completo.");
            }
        }
    }
}