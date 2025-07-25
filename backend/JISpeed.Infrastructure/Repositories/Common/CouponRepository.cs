using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories.Common;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.Common
{
    public class CouponRepository : ICouponRepository
    {
        private readonly OracleDbContext _context;

        public CouponRepository(OracleDbContext context)
        {
            _context = context;
        }

        // 根据优惠券ID获取优惠券信息
        // <param name="couponId">优惠券ID</param>
        // <returns>优惠券实体，如果不存在则返回null</returns>
        public async Task<Coupon?> GetByIdAsync(string couponId)
        {
            return await _context.Coupons
                .FirstOrDefaultAsync(c => c.CouponId == couponId);
        }

        // 根据优惠券ID获取优惠券详细信息（包含关联数据）
        // <param name="couponId">优惠券ID</param>
        // <returns>包含关联数据的优惠券实体，如果不存在则返回null</returns>
        public async Task<Coupon?> GetWithDetailsAsync(string couponId)
        {
            return await _context.Coupons
                .Include(c => c.User)
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.CouponId == couponId);
        }

        // 根据用户ID获取优惠券列表
        // <param name="userId">用户ID</param>
        // <returns>优惠券列表</returns>
        public async Task<List<Coupon>> GetByUserIdAsync(string userId)
        {
            return await _context.Coupons
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.StartTime)
                .ToListAsync();
        }

        // 根据用户ID获取有效优惠券列表
        // <param name="userId">用户ID</param>
        // <returns>有效优惠券列表</returns>
        public async Task<List<Coupon>> GetValidByUserIdAsync(string userId)
        {
            var now = DateTime.UtcNow;
            return await _context.Coupons
                .Where(c => c.UserId == userId &&
                           c.StartTime <= now &&
                           c.EndTime >= now &&
                           c.IssuedQty < c.TotalQty)
                .OrderByDescending(c => c.StartTime)
                .ToListAsync();
        }

        // 获取所有优惠券列表
        // <returns>优惠券列表</returns>
        public async Task<List<Coupon>> GetAllAsync()
        {
            return await _context.Coupons
                .Include(c => c.User)
                .OrderByDescending(c => c.StartTime)
                .ToListAsync();
        }

        // 获取过期优惠券列表
        // <returns>过期优惠券列表</returns>
        public async Task<List<Coupon>> GetExpiredCouponsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Coupons
                .Where(c => c.EndTime < now)
                .OrderByDescending(c => c.EndTime)
                .ToListAsync();
        }

        // 检查优惠券是否存在
        // <param name="couponId">优惠券ID</param>
        // <returns>优惠券是否存在</returns>
        public async Task<bool> ExistsAsync(string couponId)
        {
            return await _context.Coupons
                .AnyAsync(c => c.CouponId == couponId);
        }

        // 检查优惠券是否有效
        // <param name="couponId">优惠券ID</param>
        // <returns>优惠券是否有效</returns>
        public async Task<bool> IsValidAsync(string couponId)
        {
            var now = DateTime.UtcNow;
            return await _context.Coupons
                .AnyAsync(c => c.CouponId == couponId &&
                              c.StartTime <= now &&
                              c.EndTime >= now &&
                              c.IssuedQty < c.TotalQty);
        }

        // 创建新优惠券
        // <param name="coupon">优惠券实体</param>
        // <returns>创建的优惠券实体</returns>
        public async Task<Coupon> CreateAsync(Coupon coupon)
        {
            var entity = await _context.Coupons.AddAsync(coupon);
            return entity.Entity;
        }

        // 更新优惠券信息
        // <param name="coupon">优惠券实体</param>
        // <returns>更新的优惠券实体</returns>
        public async Task<Coupon> UpdateAsync(Coupon coupon)
        {
            var entity = _context.Coupons.Update(coupon);
            await Task.CompletedTask; // 修复CS1998警告
            return entity.Entity;
        }

        // 删除优惠券
        // <param name="couponId">优惠券ID</param>
        // <returns>是否删除成功</returns>
        public async Task<bool> DeleteAsync(string couponId)
        {
            var coupon = await GetByIdAsync(couponId);
            if (coupon == null)
                return false;

            _context.Coupons.Remove(coupon);
            return true;
        }

        // 保存更改
        // <returns>保存的记录数</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
