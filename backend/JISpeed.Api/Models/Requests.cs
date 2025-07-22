using System.ComponentModel.DataAnnotations;

// 设置请求参数，由后端保证数据正确

namespace JISpeed.Api.Models
{
    public class UserLoginRequest
    {
        public required string UserName { get; set; }
        public required string PassWord { get; set; }
    }

    public class UserRegisterRequest
    {
        [Required(ErrorMessage = "用户名不能为空")]
        [Display(Name = "用户名")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        [MinLength(6, ErrorMessage = "密码长度不能少于6位")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public required string PassWord { get; set; }

        [Required(ErrorMessage = "用户类型不能为空")]
        [Range(1, 4, ErrorMessage = "用户类型必须在1-4之间")]
        public int UserType { get; set; }

        [EmailAddress(ErrorMessage = "邮箱格式不正确")]
        public string? Email { get; set; }

        // 昵称（仅对普通用户有效）
        [StringLength(50, ErrorMessage = "昵称长度不能超过50个字符")]
        public string? Nickname { get; set; }
    }
}