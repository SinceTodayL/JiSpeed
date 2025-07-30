using System.ComponentModel.DataAnnotations;

// 设置请求参数，由后端保证数据正确

namespace JISpeed.Api.DTOS
{
    // 登录请求体（username/email 2选1）
    public class UserLoginRequest
    {

        [EmailAddress(ErrorMessage = "邮箱格式不正确")]
        public string? Email { get; set; }

        [Display(Name = "用户名")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        [MinLength(6, ErrorMessage = "密码长度不能少于6位")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public required string PassWord { get; set; }
    }

    // 注册请求体
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
        public string? PhoneNumber { get; set; }

        [Display(Name = "昵称")]
        public string? NickName { get; set; }

        [Display(Name = "性别")]
        public int Gender { get; set; } = 1; // 默认性别：男性

        [Display(Name = "生日")]
        public string? Birthday { get; set; }
    }

    public class AddFavoriteRequest
    {
        [Required(ErrorMessage = "菜品ID不能为空")]
        public string DishId { get; set; } = string.Empty;
    }

    public class AddAddressRequest
    {
        [Required(ErrorMessage = "收货人姓名不能为空")]
        public string ReceiverName { get; set; } = string.Empty;

        [Required(ErrorMessage = "收货人手机号不能为空")]
        public string ReceiverPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "详细地址不能为空")]
        public string DetailedAddress { get; set; } = string.Empty;

        public int IsDefault { get; set; } = 0;
    }

    public class AddToCartRequest
    {
        [Required(ErrorMessage = "菜品ID不能为空")]
        public string DishId { get; set; } = string.Empty;
    }
}