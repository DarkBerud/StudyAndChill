using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudyAndChill.API.Enums;

namespace StudyAndChill.API.Models
{
    public class ClassSession
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ClassStatus Status { get; set; }
        public int TeacherId { get; set; }
        public User Teacher { get; set; }
        public List<User> Students { get; set; } = new();
    }
}