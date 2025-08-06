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
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _signingKey;

        /// <summary>
        /// 构造函数，注入配置
        /// </summary>
        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;

            // 从配置中获取密钥并创建签名密钥
            var jwtKey = _configuration["Jwt:Key"] ??
                         throw new ArgumentNullException("Jwt:Key", "JWT 密钥未在配置中设置");
            _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        }

        /// <summary>
        /// 生成 JWT Token
        /// </summary>
        /// <param name="user">Identity 用户实体</param>
        /// <param name="userType">自定义用户类型（1-4）</param>
        /// <returns>JWT 字符串</returns>
        public string GenerateToken(IdentityUser user, int userType)
        {
            // 1. 创建用户声明（Claims）
            var claims = new List<Claim>
            {
                // 标准声明（建议包含）
                new Claim(ClaimTypes.NameIdentifier, user.Id), // 用户ID
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty), // 用户名
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty), // 邮箱
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Token唯一标识（用于撤销）

                // 自定义声明（根据业务需求添加）
                new Claim("UserType", userType.ToString()), // 用户类型
                new Claim("IssuedAt", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()) // 签发时间
            };

            // 2. 创建签名凭据
            var credentials = new SigningCredentials(
                _signingKey,
                SecurityAlgorithms.HmacSha256 // 加密算法
            );

            // 3. 设置 Token 有效期
            var expires = DateTime.UtcNow.AddMinutes(
                double.Parse(_configuration["Jwt:ExpireMinutes"] ?? "60") // 默认为60分钟
            );

            // 4. 构建 JWT Token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"], // 发行人（配置中定义）
                audience: _configuration["Jwt:Audience"], // 受众（配置中定义）
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            // 5. 序列化 Token 并返回
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 验证 Token 有效性并返回用户声明
        /// </summary>
        /// <param name="token">JWT 字符串</param>
        /// <returns>验证通过的用户声明，失败则返回 null</returns>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // 验证 Token 并解析声明
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    // 验证签名密钥
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _signingKey,

                    // 验证发行人和受众（与生成时一致）
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],

                    // 验证过期时间
                    ValidateLifetime = true,

                    // 允许的时钟偏差（避免服务器时间差异导致的验证失败）
                    ClockSkew = TimeSpan.FromMinutes(5)
                }, out var validatedToken);

                // 确保 Token 是有效的 JWT 格式
                if (validatedToken is not JwtSecurityToken)
                    return null;

                return principal;
            }
            catch
            {
                // 验证失败（过期、签名错误、格式错误等）
                return null;
            }
        }
    }
}