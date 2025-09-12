using System;

namespace JISpeed.Api.DTOs
{

    // 用户详细信息DTO - 用于返回给前端的用户信息

    public class UserDetailDto
    {

        // 用户ID

        public string UserId { get; set; } = string.Empty;


        // 用户昵称

        public string Nickname { get; set; } = string.Empty;


        // 头像URL

        public string AvatarUrl { get; set; } = string.Empty;


        // 性别：1-男性，2-女性，3-其他

        public int Gender { get; set; }


        // 生日

        public DateTime? Birthday { get; set; }


        // 用户等级

        public int Level { get; set; }


        // 默认地址信息

        public AddressDto? DefaultAddress { get; set; }


        // 用户注册的身份验证账号信息

        public string? UserName { get; set; }


        // 用户邮箱

        public string? Email { get; set; }


        // 手机号

        public string? PhoneNumber { get; set; }


        // 统计信息

        public UserStatsDto Stats { get; set; } = new UserStatsDto();
    }


    // 地址信息DTO

    public class AddressDto
    {

        // 地址ID

        public string AddressId { get; set; } = string.Empty;


        // 收货人姓名

        public string ReceiverName { get; set; } = string.Empty;


        // 收货人手机号

        public string ReceiverPhone { get; set; } = string.Empty;


        // 详细地址

        public string DetailAddress { get; set; } = string.Empty;


        // 地址标签（如：家、公司等）

        public string? Label { get; set; }
    }


    // 用户统计信息DTO

    public class UserStatsDto
    {

        // 订单总数

        public int TotalOrders { get; set; }


        // 收藏数量

        public int FavoriteCount { get; set; }


        // 购物车商品数量

        public int CartItemCount { get; set; }


        // 可用优惠券数量

        public int AvailableCouponCount { get; set; }


        // 地址数量

        public int AddressCount { get; set; }
    }
}
