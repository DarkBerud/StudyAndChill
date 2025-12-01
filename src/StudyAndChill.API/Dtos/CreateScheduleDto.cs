using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using StudyAndChill.API.Enums;

namespace StudyAndChill.API.Dtos
{
    public class CreateScheduleDto
    {
        [Required]
        public int StudentId { get; set; }
        [Required]
        public int TeacherId { get; set; }
        [Required]
        public DayOfWeek DayOfWeek { get; set; }
        [Required]
        public TimeOnly ClassTime { get; set; }
        [Required]
        public ClassDuration DurationInMinutes { get; set; }
        [Required]
        public DateOnly EndDate { get; set; }
        [Required]
        public int ContractId { get; set; }
    }
}