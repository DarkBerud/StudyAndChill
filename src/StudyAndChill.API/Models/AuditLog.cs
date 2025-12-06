using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyAndChill.API.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        public int? UserId { get; set; }
        public string Type { get; set; }
        public string TableName { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;

        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? PrimaryKey { get; set; }
    }
}