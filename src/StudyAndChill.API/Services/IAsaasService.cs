using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudyAndChill.API.Models;

namespace StudyAndChill.API.Services
{
    public interface IAsaasService
    {
        Task<string?> CreateCustomer(User user, StudentProfile profile);
        Task<string?> CreateSubscription(string customerId, decimal value, DateOnly nextDueDate, DateOnly? endDate = null);
    }
}