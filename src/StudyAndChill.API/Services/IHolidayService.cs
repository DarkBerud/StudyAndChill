using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyAndChill.API.Services
{
    public interface IHolidayService
    {
        Task<bool> IsHolidayAsync(DateTime date, int? teacherId = null, int? studentId = null);
    }
}