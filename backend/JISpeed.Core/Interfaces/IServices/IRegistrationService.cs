using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories;

namespace JISpeed.Core.Interfaces.IServices
{
    
    public interface IRegistrationService
    {
        Task<PreRegistrationResult> PreRegisterUserAsync(ApplicationUser request,string passwordHash);
        Task<RegistrationResult> RegisterUserAsync(string Id, string token);

        

        Task<bool> CreateBusinessEntityAsync(ApplicationUser applicationUser);

    }
    public class PreRegistrationResult
    {
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string? ApplicationUserId { get; set; }
        
        /// 成功的邮件发送结果
        
        public static PreRegistrationResult Success(string applicationUserId)
        {
            return new PreRegistrationResult
            {
                IsSuccess = true,
                ApplicationUserId = applicationUserId,
            };
        }
        // 失败的邮件结果
        
        public static PreRegistrationResult Failure(params string[] errors)
        {
            return new PreRegistrationResult
            {
                IsSuccess = false,
                Errors = errors.ToList()
            };
        }
    }
    public class RegistrationResult
    {
        
        /// 是否成功
        public bool IsSuccess { get; set; }
        
        /// 错误信息
        public List<string> Errors { get; set; } = new List<string>();

        
        /// ApplicationUser ID
        public string? ApplicationUserId { get; set; }

        
        
        // 成功的注册结果
        
        public static RegistrationResult Success(string applicationUserId)
        {
            return new RegistrationResult
            {
                IsSuccess = true,
                ApplicationUserId = applicationUserId,
            };
        }

        
        // 失败的注册结果
        
        public static RegistrationResult Failure(params string[] errors)
        {
            return new RegistrationResult
            {
                IsSuccess = false,
                Errors = errors.ToList()
            };
        }
    }
}