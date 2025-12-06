using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyAndChill.API.Data;
using StudyAndChill.API.Dtos.Financial;
using StudyAndChill.API.Enums;
using StudyAndChill.API.Models;

namespace StudyAndChill.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinancialController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public FinancialController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("teacher/statement")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetMyStatement()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) return Unauthorized();
            var teacherId = int.Parse(userIdString);

            var records = await _context.TeacherFinancialRecords
                .Include(r => r.RelatedContract)
                .ThenInclude(c => c.Student)
                .Where(r => r.TeacherId == teacherId)
                .OrderByDescending(r => r.TransactionDate)
                .ToListAsync();

            decimal balance = 0;
            foreach (var record in records)
            {
                if (record.Type == FinancialRecordType.Credit)
                    balance += record.Amount;
                else
                    balance -= record.Amount;
            }

            var response = new TeacherStatementDto
            {
                CurrentBalance = balance,
                Transactions = records.Select(r => new FinancialTransactionDto
                {
                    Id = r.Id,
                    Date = r.TransactionDate,
                    Description = r.Description,
                    Amount = r.Amount,
                    Type = r.Type == FinancialRecordType.Credit ? "Entrada" : "Saída",
                    ContractInfo = r.RelatedContract != null ? r.RelatedContract.Student.Name : "Geral"
                }).ToList()
            };

            return Ok(response);
        }

        [HttpGet("admin/dashboard")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var ownerIdString = _configuration["OWNER_TEACHER_ID"] ?? _configuration["Financial:OwnerTeacherId"];

            int ownerId = 0;
            if (!string.IsNullOrEmpty(ownerIdString))
            {
                int.TryParse(ownerIdString, out ownerId);
            }

            var totalRevenue = await _context.Payments
                .Where(p => p.Status == PaymentStatus.Received)
                .SumAsync(p => p.NetValue ?? p.Value);

            var teacherExpenses = await _context.TeacherFinancialRecords
                .Where(r => r.Type == FinancialRecordType.Credit && r.TeacherId != ownerId)
                .SumAsync(r => r.Amount);

            var pendingRevenue = await _context.Payments
                .Where(p => p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.Overdue)
                .SumAsync(p => p.Value);

            var dashboard = new AdminDashboardDto
            {
                TotalRevenue = totalRevenue,
                TotalTeacherExpenses = teacherExpenses,
                NetProfit = totalRevenue - teacherExpenses,
                PendingRevenue = pendingRevenue
            };

            return Ok(dashboard);
        }

        [HttpGet("admin/statement")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminStatement()
        {
            var payments = await _context.Payments
                .Include(p => p.Contract)
                .ThenInclude(c => c.Student)
                .Where(p => p.Status == PaymentStatus.Received)
                .ToListAsync();

            var teacherPayments = await _context.TeacherFinancialRecords
                .Include(r => r.Teacher)
                .Where(r => r.Type == FinancialRecordType.Debit)
                .ToListAsync();

            var transactions = new List<FinancialTransactionDto>();

            transactions.AddRange(payments.Select(p => new FinancialTransactionDto
            {
                Id = p.Id,
                Date = p.PaymentDate ?? DateTime.UtcNow,
                Description = $"Mensalidade Recebida - {p.Contract.Student.Name}",
                Type = "Entrada",
                ContractInfo = $"Contrato #{p.ContractId}"
            }));

            transactions.AddRange(teacherPayments.Select(tp => new FinancialTransactionDto
            {
                Id = tp.Id,
                Date = tp.TransactionDate,
                Description = tp.Description,
                Amount = tp.Amount,
                Type = "Saída",
                ContractInfo = tp.RelatedContractId.HasValue ? $"Ref. Contrato #{tp.RelatedContractId}" : "Geral"
            }));

            transactions = transactions.OrderByDescending(t => t.Date).ToList();

            decimal totalBalance = transactions.Where(t => t.Type == "Entrada").Sum(t => t.Amount)
                                 - transactions.Where(t => t.Type == "Saída").Sum(t => t.Amount);

            var response = new TeacherStatementDto
            {
                CurrentBalance = totalBalance,
                Transactions = transactions
            };

            return Ok(response);
        }

        [HttpPost("admin/pay-teacher")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PayTeacher([FromBody] CreateTeacherPaymentDto dto)
        {
            var teacher = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == dto.TeacherId && u.Role == UserRole.Teacher);

            if (teacher == null)
            {
                return BadRequest("Professor não encontrado.");
            }

            var record = new TeacherFinancialRecord
            {
                TeacherId = dto.TeacherId,
                Type = FinancialRecordType.Debit,
                Amount = dto.Amount,
                TransactionDate = dto.PaymentDate ?? DateTime.UtcNow,
                Description = string.IsNullOrEmpty(dto.Description)
                ? "Repasse realizado"
                : dto.Description,
                RelatedContractId = null
            };

            _context.TeacherFinancialRecords.Add(record);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Pagamento registrado com sucesso!", recordId = record.Id });
        }

        

    }
}