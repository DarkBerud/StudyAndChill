using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using StudyAndChill.API.Enums;

namespace StudyAndChill.API.Models
{
    public class TeacherProfile
    {
        public int Id { get; set; }

        public DocumentType DocumentType { get; set; } = DocumentType.CPF;

        [Required]
        [StringLength(50)]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        public Address Address { get; set; } = new();

        public string? BankCode { get; set; }

        public string? Agency { get; set; }

        public string? AccountNumber { get; set; }

        public string? PixKey { get; set; }

        public PixKeyType? PixKeyType { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        [Range(1, 31)]
        public int? PreferredPaymentDay { get; set; }
    }
}