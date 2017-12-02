using System.Threading.Tasks;
using GSwap.Models.Requests;
using GSwap.Models.Requests.Email;
using GSwap.Models.Requests.Tests;
using System;
using GSwap.Models.Requests.Users;

namespace GSwap.Services
{
    public interface IEmailService
    {
        Task<bool> Send(EmailRequest request);
        Task<bool> SendRecovery(RecoveryRequest request,Guid guidToken);
        Task<bool> ResetConfirmation(string userEmail);
        int GetIdByEmail(string email);
        Task<bool> ConfirmationSend(UserAddRequest request, Guid guidToken);
    }
}