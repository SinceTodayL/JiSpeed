using JISpeed.Core.Entities.Common;

namespace JISpeed.Core.Interfaces.IRepositories.Common
{
    // 优惠券仓储接口
    public interface ICouponRepository : IBaseRepository<Coupon, string>
    {
        // === 业务专用查询方法 ===

        // 根据用户ID获取优惠券列表
        // <param name="userId">用户ID</param>
        // <returns>优惠券列表</returns>
        Task<List<Coupon>> GetByUserIdAsync(
            string userId,
            int?size,int?page);

        // 根据类型获取优惠券列表
        // <param name="couponType">优惠券类型</param>
        // <returns>优惠券列表</returns>
        Task<List<Coupon>> GetByTypeAsync(int couponType);

        // 根据状态获取优惠券列表
        // <param name="status">状态</param>
        // <returns>优惠券列表</returns>
        Task<List<Coupon>> GetByStatusAsync(int status);

        // 获取有效的优惠券列表
        // <returns>有效优惠券列表</returns>
        Task<List<Coupon>> GetValidCouponsAsync();

        // 根据优惠券代码获取优惠券
        // <param name="couponCode">优惠券代码</param>
        // <returns>优惠券实体，如果不存在则返回null</returns>
        Task<Coupon?> GetByCouponCodeAsync(string couponCode);
        
        // 根据用户ID获取有效优惠券列表
        // <param name="userId">用户ID</param>
        // <returns>有效优惠券列表</returns>
        Task<List<Coupon>> GetValidByUserIdAsync(
            string userId,
            int?size,int?page);
        
        // 获取过期优惠券列表
        // <param name="userId">用户ID</param>
        // <returns>过期优惠券列表</returns>
        Task<List<Coupon>> GetExpiredByUserIdAsync(
            string userId,
            int?size,int?page);
        
        // 获取目前可用的优惠券列表
        // <param name="userId">用户ID</param>
        // <param name="amount">优惠前金额</param>
        // <returns>可用优惠券列表</returns>
        Task<List<Coupon>> GetValidByUserIdAndAmountAsync(
            string userId, 
            decimal amount,
            int? size,int? page);
    }
}
