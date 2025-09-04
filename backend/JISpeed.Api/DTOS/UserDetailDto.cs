using System.ComponentModel.DataAnnotations;

namespace JISpeed.Api.DTOs
{

    /// 用户详细信息DTO

    public class UserDetailDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public int Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public int Level { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public AddressDto? DefaultAddress { get; set; }
        public UserStatsDto Stats { get; set; } = new UserStatsDto();
    }


    /// 地址信息DTO

    public class AddressDto
    {
        public string AddressId { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string DetailAddress { get; set; } = string.Empty;
    }


    /// 用户统计信息DTO

    public class UserStatsDto
    {
        public int TotalOrders { get; set; }
        public int FavoriteCount { get; set; }
        public int CartItemCount { get; set; }
        public int AvailableCouponCount { get; set; }
        public int AddressCount { get; set; }
    }


    /// 用户收藏DTO

    public class UserFavoriteDto
    {
        public string DishId { get; set; } = string.Empty;
        public string FavorAt { get; set; } = string.Empty;
    }


    /// 用户地址DTO

    public class UserAddressDto
    {
        public string AddressId { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string DetailedAddress { get; set; } = string.Empty;
        public int IsDefault { get; set; }
    }


    /// 用户购物车项DTO

    public class UserCartItemDto
    {
        public string DishId { get; set; } = string.Empty;
        public string CartId { get; set; } = string.Empty;
        public string AddedAt { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string MerchantId { get; set; } = string.Empty;

        public int Quantity { get; set; } = 1;
    }


    /// 用户评论DTO

    public class UserReviewDto
    {
        public string ReviewId { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string DishId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Content { get; set; } = string.Empty;
        public int IsAnonymous { get; set; }
        public string ReviewAt { get; set; } = string.Empty;
    }


    /// 用户投诉DTO

    public class UserComplaintDto
    {
        public string ComplaintId { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string ComplainantId { get; set; } = string.Empty;
        public int Role { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
    }


    /// 用户配置文件DTO

    public class UserProfileDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Account { get; set; } = string.Empty;
        public int Status { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
        public string? DeletedAt { get; set; }
        public string LastLoginAt { get; set; } = string.Empty;
        public string LastLoginIp { get; set; } = string.Empty;
        public string NickName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public int Gender { get; set; }
        public string? Birthday { get; set; }
        public int Level { get; set; }
        public string? DefaultAddrId { get; set; }
    }
}
