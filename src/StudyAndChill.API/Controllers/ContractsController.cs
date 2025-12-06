using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
    public class ContractsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ContractsController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpPost("{id}/upload")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadContractPdf(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Nenhum arquiuvo enviado.");
            }

            if (file.ContentType != "application/pdf")
            {
                return BadRequest("Apenas arquivos PDF são permitidos");
            }

            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound("Contrato não encontrado");
            }

            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "Uploads", "Contracts");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{contract.StudentId}_{Guid.NewGuid()}.pdf";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            contract.ContractPdfUrl = Path.Combine("Uploads", "Contracts", uniqueFileName);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Contrato enviado com sucesso.", path = contract.ContractPdfUrl });
        }

        [HttpGet("{id}/pdf")]
        [Authorize]
        public async Task<IActionResult> GetContractPdf(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null || string.IsNullOrEmpty(contract.ContractPdfUrl))
            {
                return NotFound("Contrato ou arquivo PDF não encontrado");
            }

            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (userRole != "Admin" && contract.StudentId != userId)
            {
                return Forbid();
            }

            var filePath = Path.Combine(_environment.ContentRootPath, contract.ContractPdfUrl);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Arquivo físico não encontrado no servidor.");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/pdf", "Contrato.pdf");
        }

        [HttpGet("my-contracts")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyContracts()
        {
            var studentIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (studentIdString == null) return Unauthorized();
            var studentId = int.Parse(studentIdString);

            var contracts = await _context.Contracts
                .Where(c => c.StudentId == studentId)
                .OrderByDescending(c => c.StartDate)
                .Select(c => new
                {
                    c.Id,
                    c.Type,
                    c.Status,
                    c.StartDate,
                    c.EndDate,
                    c.MonthlyAmount,
                    c.ContractPdfUrl
                }).ToListAsync();

            return Ok(contracts);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateContract([FromBody] CreateContractDto dto)
        {
            var student = await _context.Users.FindAsync(dto.StudentId);
            var teacher = await _context.Users.FindAsync(dto.TeacherId);

            if (student == null || teacher == null) return BadRequest("Aluno ou Professor não encontrado");

            var contract = new Models.Contract
            {
                StudentId = dto.StudentId,
                TeacherId = dto.TeacherId,
                Type = dto.Type,
                ClassDuration = dto.ClassDuration,
                MonthlyAmount = dto.MonthlyAmount,
                TeacherPaymentShare = dto.TeacherPaymentShare,
                DueDay = dto.DueDay,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = ContractStatus.Active,
                MakeUpQuota = 0,
                CanBookMakeUpClasses = true
            };

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            return Ok(new { contract.Id, message = "Novo contrato criado com sucesso." });
        }

        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CancelContract(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null) return NotFound();

            contract.Status = ContractStatus.Canceled;

            await _context.SaveChangesAsync();
            return Ok("Contrato Cancelado.");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null) return NotFound();

            var hasPayment = await _context.Payments.AnyAsync(p => p.ContractId == id);
            if (hasPayment)
            {
                return BadRequest("Não é possivel deletar um contrato que contenha pagamentos.");
            }

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllContracts([FromQuery] int? studentId,[FromQuery] int? teacherId)
        {
            var query = _context.Contracts
                .Include(c => c.Student)
                .Include(c => c.Teacher)
                .AsQueryable();

            if (studentId.HasValue)
            {
                query = query.Where(c => c.StudentId == studentId.Value);
            }

            if (teacherId.HasValue)
            {
                query = query.Where(c => c.TeacherId == teacherId.Value);
            }

            var contracts = await query
                .OrderByDescending(c => c.StartDate)
                .Select(c => new
                {
                    c.Id,
                    StudenName = c.Student.Name,
                    TeacheName = c.Teacher.Name,
                    c.Type,
                    c.Status,
                    c.EndDate,
                    c.MonthlyAmount,
                    c.ContractPdfUrl
                }).ToListAsync();

            return Ok(contracts);
        }

    }
}