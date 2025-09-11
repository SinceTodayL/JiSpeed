
using JISpeed.Core.Entities.Common;

namespace JISpeed.Core.Interfaces.IServices
{
    public interface IEmailService
    {
        Task <string?> SendVerificationEmailAsync(ApplicationUser newUser);
        bool IsValidEmailAsync(string email);
    }
}