using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using StudyAndChill.API.Enums;

namespace StudyAndChill.API.Models
{
    public class TeacherFinancialRecord
    {
        public int Id { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public FinancialRecordType Type { get; set; }

        public string Description { get; set; } = string.Empty;

        public int TeacherId { get; set; }
        public User Teacher { get; set; } = null!;

        public int? RelatedContractId { get; set; }
        public Contract? RelatedContract { get; set; }
    }
}