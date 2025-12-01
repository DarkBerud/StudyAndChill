using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection; 
using StudyAndChill.API.Data;
using StudyAndChill.API.Dtos.External;

namespace StudyAndChill.API.Services
{
    public class HolidayService : IHolidayService
    {
        private readonly HttpClient _httpClient;
        private readonly IServiceScopeFactory _scopeFactory;

        private static readonly Dictionary<int, List<BrasilApiHolidayDto>> _holidaysCache = new();

        public HolidayService(HttpClient httpClient, IServiceScopeFactory scopeFactory)
        {
            _httpClient = httpClient;
            _scopeFactory = scopeFactory;
        }

        public async Task<bool> IsHolidayAsync(DateTime date, int? teacherId = null, int? studentId = null)
        {
            var year = date.Year;
            var dateOnly = DateOnly.FromDateTime(date);
            bool isHolidayFound = false;

           
            if (!_holidaysCache.ContainsKey(year))
            {
                try
                {
                    var response = await _httpClient.GetAsync($"https://brasilapi.com.br/api/feriados/v1/{year}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var holidays = JsonSerializer.Deserialize<List<BrasilApiHolidayDto>>(content);

                        if (holidays != null)
                        {
                            _holidaysCache[year] = holidays;
                        }
                    }
                }
                catch
                {
                    
                }
            }

           
            if (_holidaysCache.ContainsKey(year))
            {
                if(_holidaysCache[year].Any(h => h.Date.Date == date.Date)) isHolidayFound = true;
            }

            
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                
                if (!isHolidayFound)
                {
                    isHolidayFound = await context.Holidays.AnyAsync(h => 
                    h.Date == dateOnly &&
                    (
                        h.IsGlobal || 
                        (teacherId.HasValue && h.AffectedUsers.Any(u => u.Id == teacherId)) || 
                        (studentId.HasValue && h.AffectedUsers.Any(u => u.Id == studentId))
                    )
                    );
                }

                if (isHolidayFound && teacherId.HasValue)
                {
                    var wantsToWork = await context.TeacherHolidayWorks
                        .AnyAsync(w => w.TeacherId == teacherId.Value && w.Date == dateOnly);
                    
                    if (wantsToWork)
                    {
                        return false;
                    }
                }
            }
            return isHolidayFound;
        }
    }
}