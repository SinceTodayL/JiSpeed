/*using JISpeed.Api.DTOs;
using JISpeed.Core.Entities.User;
using JISpeed.Core.Interfaces.IRepositories;

namespace JISpeed.Api.Mappers
{
    
    // 用户信息映射器 - 负责将实体对象转换为DTO对象
    
    public static class UserMapper
    {
        
        // 将User实体转换为UserDetailDto
        
        // <param name="user">用户实体</param>
        // <param name="stats">用户统计信息</param>
        // <returns>用户详情DTO</returns>
        public static UserDetailDto ToUserDetailDto(User user, UserStats stats)
        {
            return new UserDetailDto
            {
                UserId = user.UserId,
                Nickname = user.Nickname,
                AvatarUrl = user.AvatarUrl,
                Gender = user.Gender,
                Birthday = user.Birthday,
                Level = user.Level,
                DefaultAddress = user.DefaultAddress != null ? ToAddressDto(user.DefaultAddress) : null,
                UserName = user.ApplicationUser?.UserName,
                Email = user.ApplicationUser?.Email,
                PhoneNumber = user.ApplicationUser?.PhoneNumber,
                Stats = ToUserStatsDto(stats)
            };
        }

        
        // 将Address实体转换为AddressDto
        
        // <param name="address">地址实体</param>
        // <returns>地址DTO</returns>
        public static AddressDto ToAddressDto(Address address)
        {
            return new AddressDto
            {
                AddressId = address.AddressId,
                ReceiverName = address.ReceiverName,
                ReceiverPhone = address.ReceiverPhone,
                DetailAddress = address.DetailedAddress,
                Label = address.IsDefault == 1 ? "默认地址" : "普通地址"
            };
        }

        
        // 将UserStats转换为UserStatsDto
        
        // <param name="stats">用户统计信息</param>
        // <returns>用户统计DTO</returns>
        public static UserStatsDto ToUserStatsDto(UserStats stats)
        {
            return new UserStatsDto
            {
                TotalOrders = stats.TotalOrders,
                FavoriteCount = stats.FavoriteCount,
                CartItemCount = stats.CartItemCount,
                AvailableCouponCount = stats.AvailableCouponCount,
                AddressCount = stats.AddressCount
            };
        }
    }
}*/
