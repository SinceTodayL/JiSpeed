using System.ComponentModel.DataAnnotations;

// 设置请求参数，由后端保证数据正确

namespace JISpeed.Api.DTOS
{

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