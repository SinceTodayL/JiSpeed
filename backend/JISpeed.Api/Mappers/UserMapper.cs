using JISpeed.Api.DTOs;
using JISpeed.Api.DTOS;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Entities.User;
using JISpeed.Core.Entities.Order;

namespace JISpeed.Api.Mappers
{
    public static class UserMapper
    {

        /// 将User实体映射为UserDetailDto

        public static UserDetailDto ToUserDetailDto(User user, UserStatsDto? stats = null)
        {
            return new UserDetailDto
            {
                UserId = user.UserId,
                Nickname = user.Nickname,
                AvatarUrl = user.AvatarUrl,
                Gender = user.Gender,
                Birthday = user.Birthday,
                Level = user.Level,
                UserName = user.ApplicationUser?.UserName,
                Email = user.ApplicationUser?.Email,
                PhoneNumber = user.ApplicationUser?.PhoneNumber,
                DefaultAddress = user.DefaultAddress != null ? ToAddressDto(user.DefaultAddress) : null,
                Stats = stats ?? new UserStatsDto()
            };
        }

        // 将User实体列表映射为UserDetailDto列表
        public static List<UserDetailDto> ToUserDetailDtoList(IEnumerable<User> users)
        {
            return users.Select(user => ToUserDetailDto(user)).ToList();
        }


        /// 将Address实体映射为AddressDto

        public static AddressDto ToAddressDto(Address address)
        {
            return new AddressDto
            {
                AddressId = address.AddressId,
                ReceiverName = address.ReceiverName,
                ReceiverPhone = address.ReceiverPhone,
                DetailAddress = address.DetailedAddress
            };
        }


        /// 将Address实体映射为UserAddressDto

        public static UserAddressDto ToUserAddressDto(Address address)
        {
            return new UserAddressDto
            {
                AddressId = address.AddressId,
                ReceiverName = address.ReceiverName,
                ReceiverPhone = address.ReceiverPhone,
                DetailedAddress = address.DetailedAddress,
                IsDefault = address.IsDefault
            };
        }


        /// 将Favorite实体映射为UserFavoriteDto

        public static UserFavoriteDto ToUserFavoriteDto(Favorite favorite)
        {
            return new UserFavoriteDto
            {
                DishId = favorite.DishId,
                FavorAt = favorite.FavorAt.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }


        /// 将CartItem实体映射为UserCartItemDto

        public static UserCartItemDto ToUserCartItemDto(CartItem cartItem)
        {
            return new UserCartItemDto
            {
                DishId = cartItem.DishId,
                CartId = cartItem.CartItemId,
                AddedAt = cartItem.AddedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                UserId = cartItem.UserId,
                MerchantId = cartItem.MerchantId,
                Quantity = cartItem.Quantity
            };
        }


        /// 将Review实体映射为UserReviewDto

        public static UserReviewDto ToUserReviewDto(Review review, string dishId = "")
        {
            return new UserReviewDto
            {
                ReviewId = review.ReviewId,
                OrderId = review.OrderId,
                DishId = dishId, // 需要从DishReviews关联表中获取
                UserId = review.UserId,
                Rating = review.Rating,
                Content = review.Content ?? string.Empty,
                IsAnonymous = review.IsAnonymous,
                ReviewAt = review.ReviewAt.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }


        /// 将Complaint实体映射为UserComplaintDto

        public static UserComplaintDto ToUserComplaintDto(Complaint complaint)
        {
            return new UserComplaintDto
            {
                ComplaintId = complaint.ComplaintId,
                OrderId = complaint.OrderId,
                ComplainantId = complaint.ComplainantId,
                Role = complaint.CmplRole,
                Description = complaint.CmplDescription ?? string.Empty,
                Status = complaint.CmplStatus.ToString(),
                CreatedAt = complaint.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }


        /// 将User和ApplicationUser实体映射为UserProfileDto

        public static UserProfileDto ToUserProfileDto(User user, ApplicationUser applicationUser, string lastLoginIp = "unknown")
        {
            return new UserProfileDto
            {
                UserId = user.UserId,
                Account = applicationUser.UserName ?? string.Empty,
                Status = applicationUser.Status,
                CreatedAt = applicationUser.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                DeletedAt = null,
                LastLoginAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                LastLoginIp = lastLoginIp,
                NickName = user.Nickname,
                AvatarUrl = user.AvatarUrl,
                Gender = user.Gender,
                Birthday = user.Birthday?.ToString("yyyy-MM-dd"),
                Level = user.Level,
                DefaultAddrId = user.DefaultAddrId
            };
        }


        /// 创建UserStatsDto

        public static UserStatsDto ToUserStatsDto(
            int totalOrders = 0,
            int favoriteCount = 0,
            int cartItemCount = 0,
            int availableCouponCount = 0,
            int addressCount = 0)
        {
            return new UserStatsDto
            {
                TotalOrders = totalOrders,
                FavoriteCount = favoriteCount,
                CartItemCount = cartItemCount,
                AvailableCouponCount = availableCouponCount,
                AddressCount = addressCount
            };
        }


        /// 将Address实体列表映射为UserAddressDto列表

        public static List<UserAddressDto> ToUserAddressDtoList(IEnumerable<Address> addresses)
        {
            return addresses.Select(ToUserAddressDto).ToList();
        }


        /// 将Favorite实体列表映射为UserFavoriteDto列表

        public static List<UserFavoriteDto> ToUserFavoriteDtoList(IEnumerable<Favorite> favorites)
        {
            return favorites.Select(ToUserFavoriteDto).ToList();
        }


        /// 将CartItem实体列表映射为UserCartItemDto列表

        public static List<UserCartItemDto> ToUserCartItemDtoList(IEnumerable<CartItem> cartItems)
        {
            return cartItems.Select(ToUserCartItemDto).ToList();
        }


        /// 将Review实体列表映射为UserReviewDto列表

        public static List<UserReviewDto> ToUserReviewDtoList(IEnumerable<Review> reviews)
        {
            return reviews.Select(r => ToUserReviewDto(r)).ToList();
        }


        /// 将Complaint实体列表映射为UserComplaintDto列表

        public static List<UserComplaintDto> ToUserComplaintDtoList(IEnumerable<Complaint> complaints)
        {
            return complaints.Select(ToUserComplaintDto).ToList();
        }
    }
}
