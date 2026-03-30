using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyAndChill.API.Dtos
{
    public class InvoiceDto
    {
        public string Id { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PaymentLink { get; set; }
    }
}