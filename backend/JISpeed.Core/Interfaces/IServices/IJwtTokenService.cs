using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace JISpeed.Core.Interfaces.IServices
{

    /// <summary>
    /// JWT Token 服务，负责生成、验证 Token
    /// </summary>
    public interface IJwtTokenService
    {
        /// <summary>
        /// 为用户生成 JWT Token
        /// </summary>
        string GenerateToken(IdentityUser user, int userType);

        /// <summary>
        /// 验证 Token 并返回用户声明
        /// </summary>
        ClaimsPrincipal? ValidateToken(string token);
    }
}