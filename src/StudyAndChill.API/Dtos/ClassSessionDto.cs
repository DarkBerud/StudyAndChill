using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudyAndChill.API.Enums;

namespace StudyAndChill.API.Dtos
{
    public class ClassSessionDto
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public ClassStatus Status { get; set; }

        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;

        public List<StudentSummaryDto> Students { get; set; }
    }

    public class StudentSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}