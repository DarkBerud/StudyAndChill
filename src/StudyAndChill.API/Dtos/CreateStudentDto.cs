using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudyAndChill.API.Dtos
{
    public class CreateStudentDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public int TeacherId { get; set; }
    }
}