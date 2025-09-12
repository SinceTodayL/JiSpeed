using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace JISpeed.Application.Services.Common
{
    public class JwtTokenService : IJwtTokenService
    {


        /// 构造函数，注入配置

        public JwtTokenService()
        {
        }


        /// 生成 JWT Token

        /// <param name="userName">Identity 用户实体</param>
        /// <returns>JWT 字符串</returns>
        public string GenerateToken(string userName)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("your-256-bit-secret-key-here-123456")
            );
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userName)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}