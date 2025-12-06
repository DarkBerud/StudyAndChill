using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyAndChill.API.Dtos.Financial
{
    public class TeacherStatementDto
    {
        public decimal CurrentBalance { get; set; }
        public List<FinancialTransactionDto> Transactions { get; set; } = new();
    }

    public class FinancialTransactionDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string ContractInfo { get; set; }
    }
}