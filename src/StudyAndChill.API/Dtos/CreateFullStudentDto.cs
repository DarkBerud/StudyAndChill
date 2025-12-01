using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using StudyAndChill.API.Enums;

namespace StudyAndChill.API.Dtos
{
    public class CreateFullStudentDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public DocumentType DocumentType { get; set; }

        [Required]
        public string DocumentNumber { get; set; }

        [Required]
        public AddressDto Address { get; set; }

        [Required]
        public int TeacherId { get; set; }

        [Required]
        public ContractType ContractType { get; set; }

        [Required]
        public ClassDuration ClassDuration { get; set; }

        [Required]
        public decimal MonthlyAmount { get; set; }

        [Required]
        [Range(1, 31)]
        public int DueDay { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }
    }
}