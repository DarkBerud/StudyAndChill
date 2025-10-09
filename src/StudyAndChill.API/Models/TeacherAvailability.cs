using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyAndChill.API.Models
{
    public class TeacherAvailability
    {
        public int Id { get; set; }
        public int DayOfWeek { get; set; }
        public TimeOnly AvailableFrom { get; set; }
        public TimeOnly AvailableTo { get; set; }
        public int TeacherId { get; set; }
        public User Teacher { get; set; }
    }
}