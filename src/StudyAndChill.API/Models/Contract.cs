using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using StudyAndChill.API.Enums;

namespace StudyAndChill.API.Models
{
    public class Contract
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public User Student { get; set; } = null!;

        public int TeacherId { get; set; }
        public User Teacher { get; set; } = null!;

        public ContractType Type { get; set; }
        public ClassDuration ClassDuration { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public ContractStatus Status { get; set; }

        public int MakeUpQuota { get; set; }
        public bool CanBookMakeUpClasses { get; set; } = true;
        
        public List<ClassSession> ClassSessions { get; set; } = new();

        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TeacherPaymentShare { get; set; }
        public int DueDay { get; set; }
        public string? AsaasSubscriptionId { get; set; }

        public List<Payment> Payments { get; set; } = new();
    }
}