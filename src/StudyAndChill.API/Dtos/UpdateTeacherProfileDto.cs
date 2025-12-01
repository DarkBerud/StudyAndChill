using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using StudyAndChill.API.Enums;

namespace StudyAndChill.API.Dtos
{
    public class UpdateTeacherProfileDto
    {
        [Required]
        public string Phone { get; set; }

        [Required]
        public DocumentType DocumentType { get; set; }

        [Required]
        public string DocumentNumber { get; set; }

        [Required]
        public AddressDto Address { get; set; }

        public string? BankCode { get; set; }
        public string? Agency { get; set; }
        public string? AccountNumber { get; set; }
        public string? PixKey { get; set; }
        public PixKeyType? PixKeyType { get; set; }
    }
}