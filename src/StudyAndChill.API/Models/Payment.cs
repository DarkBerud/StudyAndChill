using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using StudyAndChill.API.Enums;

namespace StudyAndChill.API.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public DateOnly DueDate { get; set; }
        public DateTime? PaymentDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? NetValue { get; set; }

        public PaymentStatus Status { get; set; }

        public string? AsaasPaymentId { get; set; }
        public string? AsaasInvoiceUrl { get; set; }
        public string? AsaasPixQrCode { get; set; }

        public int ContractId { get; set; }
        public Contract Contract { get; set; } = null!;
    }
}