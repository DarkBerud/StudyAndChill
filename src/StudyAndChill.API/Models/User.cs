using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudyAndChill.API.Enums;

namespace StudyAndChill.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public UserRole Role { get; set; }
        public string? IconUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ClassSession> ClassSessions { get; set; } = new();
        public List<ClassSession> TaughtClasses { get; set; } = new();
        public List<Holiday> SpecificHoliday {get; set; } = new();

    }
}