using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace JISpeed.Core.Interfaces.IServices
{


    /// JWT Token 服务，负责生成、验证 Token

    public interface IJwtTokenService
    {

        /// 为用户生成 JWT Token

        string GenerateToken(string userName);

    }
}