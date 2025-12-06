using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudyAndChill.API.Dtos.Financial
{
    public class CreateTeacherPaymentDto
    {
        [Required]
        public int TeacherId { get; set; }

        [Required]
        [Range(0.01, 1000000)]
        public decimal Amount { get; set; }

        public string? Description { get; set; }

        public DateTime? PaymentDate { get; set; }
    }
}