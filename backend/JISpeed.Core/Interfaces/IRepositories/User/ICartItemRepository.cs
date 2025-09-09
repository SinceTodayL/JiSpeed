using JISpeed.Core.Entities.User;

namespace JISpeed.Core.Interfaces.IRepositories.User
{

    /// 购物车项仓储接口

    public interface ICartItemRepository : IBaseRepository<CartItem, string>
    {

        /// 根据用户ID获取购物车项（分页）

        /// <param name="userId">用户ID</param>
        /// <param name="page">页码</param>
        /// <param name="size">每页大小</param>
        /// <returns>购物车项列表</returns>
        Task<List<CartItem>> GetByUserIdAsync(string userId, int page = 1, int size = 10);


        /// 根据用户ID获取购物车项数量

        /// <param name="userId">用户ID</param>
        /// <returns>购物车项数量</returns>
        Task<int> GetCountByUserIdAsync(string userId);


        /// 根据用户ID和菜品ID获取购物车项

        /// <param name="userId">用户ID</param>
        /// <param name="dishId">菜品ID</param>
        /// <returns>购物车项</returns>
        Task<CartItem?> GetByUserIdAndDishIdAsync(string userId, string dishId);


        /// 根据用户ID和购物车项ID获取购物车项

        /// <param name="userId">用户ID</param>
        /// <param name="cartItemId">购物车项ID</param>
        /// <returns>购物车项</returns>
        Task<CartItem?> GetByUserIdAndCartItemIdAsync(string userId, string cartItemId);


        /// 删除用户的所有购物车项

        /// <param name="userId">用户ID</param>
        /// <returns>删除的数量</returns>
        Task<int> DeleteByUserIdAsync(string userId);
    }
}
