using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories.Common;

namespace JISpeed.Core.Interfaces.IServices
{
    public interface ICouponService
    {
        Task<List<string>> GetCouponIdByFilterAsync(
            string userId,
            bool? isActive,
            decimal? amount,
            int? size,int? page);
        Task<Coupon> GetCouponDetailByCouponIdAsync(string couponId);

        Task<bool> IssueCouponsAsync(
            string adminId,
            DateTime startTime, DateTime endTime,
            string? userId,
            decimal? faceValue, decimal? threshold);
    }
    
}