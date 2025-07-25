using JISpeed.Core.Entities.Common;

namespace JISpeed.Core.Interfaces.IRepositories.Common
{
    // 优惠券仓储接口
    public interface ICouponRepository
    {
        // 根据优惠券ID获取优惠券信息
        // <param name="couponId">优惠券ID</param>
        // <returns>优惠券实体，如果不存在则返回null</returns>
        Task<Coupon?> GetByIdAsync(string couponId);

        // 根据优惠券ID获取优惠券详细信息（包含关联数据）
        // <param name="couponId">优惠券ID</param>
        // <returns>包含关联数据的优惠券实体，如果不存在则返回null</returns>
        Task<Coupon?> GetWithDetailsAsync(string couponId);

        // 根据用户ID获取优惠券列表
        // <param name="userId">用户ID</param>
        // <returns>优惠券列表</returns>
        Task<List<Coupon>> GetByUserIdAsync(string userId);

        // 根据用户ID获取有效优惠券列表
        // <param name="userId">用户ID</param>
        // <returns>有效优惠券列表</returns>
        Task<List<Coupon>> GetValidByUserIdAsync(string userId);

        // 获取所有优惠券列表
        // <returns>优惠券列表</returns>
        Task<List<Coupon>> GetAllAsync();

        // 获取过期优惠券列表
        // <returns>过期优惠券列表</returns>
        Task<List<Coupon>> GetExpiredCouponsAsync();

        // 检查优惠券是否存在
        // <param name="couponId">优惠券ID</param>
        // <returns>优惠券是否存在</returns>
        Task<bool> ExistsAsync(string couponId);

        // 检查优惠券是否有效
        // <param name="couponId">优惠券ID</param>
        // <returns>优惠券是否有效</returns>
        Task<bool> IsValidAsync(string couponId);

        // 创建新优惠券
        // <param name="coupon">优惠券实体</param>
        // <returns>创建的优惠券实体</returns>
        Task<Coupon> CreateAsync(Coupon coupon);

        // 更新优惠券信息
        // <param name="coupon">优惠券实体</param>
        // <returns>更新的优惠券实体</returns>
        Task<Coupon> UpdateAsync(Coupon coupon);

        // 删除优惠券
        // <param name="couponId">优惠券ID</param>
        // <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(string couponId);

        // 保存更改
        // <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();
    }
}
