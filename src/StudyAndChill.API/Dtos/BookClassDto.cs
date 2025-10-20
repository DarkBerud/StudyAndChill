using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudyAndChill.API.Dtos
{
    public class BookClassDto
    {
        [Required]
        public int TeacherId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }
    }
}