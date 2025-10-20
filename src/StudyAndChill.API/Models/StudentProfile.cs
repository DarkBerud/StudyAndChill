using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyAndChill.API.Models
{
    public class StudentProfile
    {
        public int Id { get; set; }
        public int MakeUpClassQuota { get; set; } = 0;
        public bool CanBookMakeUoClasses { get; set; } = true;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}