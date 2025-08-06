using System.ComponentModel.DataAnnotations;

// 设置请求参数，由后端保证数据正确

namespace JISpeed.Api.DTOS
{
    // 登录请求体（username/email 2选1）
    public class UserLoginRequest
    {
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public required string PassWord { get; set; }
    }

    // 注册请求体
    public class UserRegisterRequest
    {
        public required string UserName { get; set; } 
        
        public required string PassWord { get; set; } 
        
        public required string Email { get; set; } 
        
        public string?PhoneNumber { get; set; } 
        
    }

    public class ApplicationUserDto
    {
        public required string ID { get; set; } 
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public required string UserName { get; set; } 
        public required int UserType { get; set; } 
        public required int Status { get; set; } 
        public DateTime Lockout_End{ get; set; } 
        
    }
    
    // 登录响应模型（包含 Token 信息）
    public class LoginResponse
    {
        public required string Id { get; set; }
        public required string Token { get; set; }
    }
    
}