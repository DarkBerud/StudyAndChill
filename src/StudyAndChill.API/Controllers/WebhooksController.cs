using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyAndChill.API.Data;
using StudyAndChill.API.Dtos.External.Asaas;
using StudyAndChill.API.Enums;
using StudyAndChill.API.Models;

namespace StudyAndChill.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhooksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public WebhooksController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("asaas")]
        public async Task<IActionResult> HandleAsaasWebhook(
            [FromHeader(Name = "asaas-access-token")] string receivedToken,
            [FromBody] AsaasWebhookEventDto payload)
        {
            var mySecretToken = _configuration["ASAAS_WEBHOOK_TOKEN"];
            if (receivedToken != mySecretToken)
            {
                return Unauthorized("Token de webhook inválido.");
            }

            if (payload.Event == "PAYMENT_RECEIVED")
            {
                await ProcessPaymentReceived(payload.Payment);
            }

            return Ok(new { received = true });
        }

        private async Task ProcessPaymentReceived(AsaasWebhookPaymentDto asaasPayment)
        {
            var contract = await _context.Contracts
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(c => c.AsaasSubscriptionId == asaasPayment.Subscription);

            if (contract == null)
            {
                Console.WriteLine($"[Webhook] Contrato não encontrado para a assinatura {asaasPayment.Subscription}");
                return;
            }

            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.AsaasPaymentId == asaasPayment.Id);

            if (payment == null)
            {
                payment = new Payment
                {
                    ContractId = contract.Id,
                    AsaasPaymentId = asaasPayment.Id,
                    DueDate = DateOnly.Parse(asaasPayment.DateCreated)
                };
                _context.Payments.Add(payment);
            }

            payment.Status = PaymentStatus.Received;
            payment.Value = asaasPayment.Value;
            payment.NetValue = asaasPayment.NetValue;
            if (DateTime.TryParse(asaasPayment.PaymentDate, out var pDate))
            {
                payment.PaymentDate = DateTime.SpecifyKind(pDate, DateTimeKind.Utc);
            }
            else
            {
                payment.PaymentDate = DateTime.UtcNow;
            }
            payment.AsaasInvoiceUrl = asaasPayment.InvoiceUrl;

            if (contract.MonthlyAmount > 0)
            {
                decimal ratio = contract.TeacherPaymentShare / contract.MonthlyAmount;

                decimal teacherCreditAmount = asaasPayment.Value * ratio;

                var financialRecord = new TeacherFinancialRecord
                {
                    TeacherId = contract.TeacherId,
                    TransactionDate = DateTime.UtcNow,
                    Type = FinancialRecordType.Credit,
                    Amount = teacherCreditAmount,
                    RelatedContractId = contract.Id,
                    Description = $"Recebimento Mensalidade - {asaasPayment.BillingType} (Ref: {asaasPayment.Id})"
                };
                _context.TeacherFinancialRecords.Add(financialRecord);
            }
            await _context.SaveChangesAsync();
        }
    }
}