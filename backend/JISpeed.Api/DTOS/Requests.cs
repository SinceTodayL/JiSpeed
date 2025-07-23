using System.ComponentModel.DataAnnotations;

// 设置请求参数，由后端保证数据正确

namespace JISpeed.Api.DTOS
{
    public class UserLoginRequest
    {
        [Required(ErrorMessage = "用户名不能为空")]
        [Display(Name = "邮箱")]
        public required string Emil { get; set; }
        
        [Required(ErrorMessage = "密码不能为空")]
        [MinLength(6, ErrorMessage = "密码长度不能少于6位")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
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
        
        [Required(ErrorMessage = "邮箱不能为空")]
        [Display(Name = "邮箱")]
        public required string Email { get; set; } 
        
        [Display(Name = "手机号码")]
        public string?PhoneNumber { get; set; } 
        
    }
}