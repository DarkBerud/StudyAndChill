using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyAndChill.API.Dtos.Financial
{
    public class AdminDashboardDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalTeacherExpenses { get; set; }
        public decimal NetProfit { get; set; }

        public decimal PendingRevenue { get; set; }
    }
}