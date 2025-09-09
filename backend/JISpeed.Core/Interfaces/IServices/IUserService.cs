using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.Entities.User;
using JISpeed.Core.Entities.Common;

namespace JISpeed.Core.Interfaces.IServices
{

    /// 用户服务接口 - 定义用户模块的业务逻辑操作

    public interface IUserService
    {

        /// 创建用户实体（当ApplicationUser的UserType=1时调用）

        /// <param name="applicationUser">已创建的ApplicationUser</param>
        /// <param name="nickname">用户昵称，默认使用用户名</param>
        /// <returns>创建的User实体</returns>
        Task<User> CreateUserEntityAsync(ApplicationUser applicationUser, string? nickname = null);


        /// 根据ID获取用户信息

        /// <param name="userId">用户ID</param>
        /// <returns>用户实体</returns>
        Task<User> GetUserByIdAsync(string userId);

        /// 根据条件获取用户列表
        /// <param name="size">每页大小</param>
        /// <param name="page">页码</param>
        /// <param name="userId">用户ID过滤</param>
        /// <param name="nickname">昵称过滤</param>
        /// <returns>用户列表</returns>
        Task<List<User>> GetUsersByFiltersAsync(int? size, int? page, string? userId, string? nickname);


        /// 更新用户信息

        /// <param name="user">更新的用户实体</param>
        /// <returns>更新后的用户实体</returns>
        Task<User> UpdateUserAsync(User user);


        /// 根据ApplicationUserId获取用户信息

        /// <param name="applicationUserId">ApplicationUser ID</param>
        /// <returns>用户实体</returns>
        Task<User?> GetUserByApplicationUserIdAsync(string applicationUserId);

        // 收藏相关方法

        /// 添加收藏

        /// <param name="userId">用户ID</param>
        /// <param name="dishId">菜品ID</param>
        /// <returns>添加是否成功</returns>
        Task<bool> AddFavoriteAsync(string userId, string dishId);


        /// 移除收藏

        /// <param name="userId">用户ID</param>
        /// <param name="dishId">菜品ID</param>
        /// <returns>移除是否成功</returns>
        Task<bool> RemoveFavoriteAsync(string userId, string dishId);


        /// 检查是否已收藏

        /// <param name="userId">用户ID</param>
        /// <param name="dishId">菜品ID</param>
        /// <returns>是否已收藏</returns>
        Task<bool> IsFavoriteAsync(string userId, string dishId);


        /// 获取收藏数量

        /// <param name="userId">用户ID</param>
        /// <returns>收藏数量</returns>
        Task<int> GetFavoriteCountAsync(string userId);

        // 购物车相关方法

        /// 添加到购物车

        /// <param name="userId">用户ID</param>
        /// <param name="dishId">菜品ID</param>
        /// <returns>购物车项ID</returns>
        Task<CartItem?> AddToCartAsync(string userId, string dishId, string merchantId);


        /// 修改购物车商品数量

        /// <param name="userId"></param>
        /// <param name="cartId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        Task<CartItem?> UpdateCartItemQuantityAsync(string userId, string cartId, int quantity);


        /// 从购物车移除

        /// <param name="userId">用户ID</param>
        /// <param name="cartItemId">购物车项ID</param>
        /// <returns>移除是否成功</returns>
        Task<bool> RemoveFromCartAsync(string userId, string cartItemId);


        /// 检查是否在购物车中

        /// <param name="userId">用户ID</param>
        /// <param name="dishId">菜品ID</param>
        /// <returns>是否在购物车中</returns>
        Task<bool> IsInCartAsync(string userId, string dishId);


        /// 获取购物车项数量

        /// <param name="userId">用户ID</param>
        /// <returns>购物车项数量</returns>
        Task<int> GetCartItemCountAsync(string userId);

        // 地址相关方法

        /// 创建地址

        /// <param name="userId">用户ID</param>
        /// <param name="receiverName">收货人姓名</param>
        /// <param name="receiverPhone">收货人手机号</param>
        /// <param name="detailedAddress">详细地址</param>
        /// <param name="isDefault">是否默认地址</param>
        /// <returns>地址ID</returns>
        Task<string?> CreateAddressAsync(string userId, string receiverName, string receiverPhone, string detailedAddress, int isDefault);


        /// 更新地址

        /// <param name="userId">用户ID</param>
        /// <param name="addressId">地址ID</param>
        /// <param name="receiverName">收货人姓名</param>
        /// <param name="receiverPhone">收货人手机号</param>
        /// <param name="detailedAddress">详细地址</param>
        /// <param name="isDefault">是否默认地址</param>
        /// <returns>更新是否成功</returns>
        Task<bool> UpdateAddressAsync(string userId, string addressId, string receiverName, string receiverPhone, string detailedAddress, int isDefault);


        /// 删除地址

        /// <param name="userId">用户ID</param>
        /// <param name="addressId">地址ID</param>
        /// <returns>删除是否成功</returns>
        Task<bool> DeleteAddressAsync(string userId, string addressId);


        /// 获取地址数量

        /// <param name="userId">用户ID</param>
        /// <returns>地址数量</returns>
        Task<int> GetAddressCountAsync(string userId);


        /// 评论相关
        Task<bool> AddReviewAsync(string userId, string orderId, int rating, string? content = null, int isAnonymous = 2);

        /// 删除评论
        Task<bool> DeleteReviewAsync(string userId, string reviewId);

        /// 提交投诉
        Task<string> AddComplaintAsync(string UserId, string OrderId, int Role, int Status, string Description);
    }
}
