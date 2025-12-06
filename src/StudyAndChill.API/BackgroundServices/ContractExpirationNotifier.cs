using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudyAndChill.API.Data;
using StudyAndChill.API.Enums;
using StudyAndChill.API.Models;

namespace StudyAndChill.API.BackgroundServices
{
    public class ContractExpirationNotifier : BackgroundService
    {
         private readonly IServiceScopeFactory _scopeFactory;
         private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);

         public ContractExpirationNotifier(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckContractsAndNotify();

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CheckContractsAndNotify()
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                    var ownerIdString = configuration["OWNER_TEACHER_ID"] ?? configuration["Financial:OwnerTeacherId"];
                    if (!int.TryParse(ownerIdString, out int adminId)) return;

                    var targetDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));

                    var expiringContracts = await context.Contracts
                        .Include(c => c.Student)
                        .Where(c => c.Status == ContractStatus.Active &&
                                    c.EndDate == targetDate)
                        .ToListAsync();

                    foreach (var contract in expiringContracts)
                    {
                        var notification = new Notification
                        {
                            UserId = adminId,
                            Title = "Renovação Necessária",
                            Message = $"O contrato do aluno {contract.Student.Name} vence dentro de 30 dias ({contract.EndDate})",
                            CreatedAt = DateTime.UtcNow,
                            IsRead = false
                        };

                        context.Notifications.Add(notification);
                    }

                    if (expiringContracts.Any())
                    {
                        await context.SaveChangesAsync();
                        Console.WriteLine($"[Notifier] {expiringContracts.Count} notificações de vencimento geradas.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Notifier] Erro ao verificar contratos: {ex.Message}");
            }
        }
    }
}