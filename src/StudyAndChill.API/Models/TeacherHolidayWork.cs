using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudyAndChill.API.Models
{
    public class TeacherHolidayWork
    {
        public int Id { get; set; }
        
        [Required]
        public DateOnly Date { get; set; }

        public int TeacherId { get; set; }
        public User Teacher { get; set; } = null!;
    }
}