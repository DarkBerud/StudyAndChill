using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using StudyAndChill.API.Enums;

namespace StudyAndChill.API.Dtos
{
    public class UpdateStudentProfileDto
    {
        [Required]
        public string Phone { get; set; }

        [Required]
        public DocumentType DocumentType { get; set; }

        [Required]
        public string DocumentNumber { get; set; }

        [Required]
        public AddressDto Address { get; set; }
    }
}