using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories.Common;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.Common
{
    public class CouponRepository : BaseRepository<Coupon, string>, ICouponRepository
    {
        public CouponRepository(OracleDbContext context) : base(context)
        {
        }

        // 重写：根据优惠券ID获取优惠券信息
        public override async Task<Coupon?> GetByIdAsync(string couponId)
        {
            return await _context.Coupons
                .FirstOrDefaultAsync(c => c.CouponId == couponId);
        }

        // 重写：根据优惠券ID获取优惠券详细信息（包含关联数据）
        public override async Task<Coupon?> GetWithDetailsAsync(string couponId)
        {
            return await _context.Coupons
                .Include(c => c.User)
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.CouponId == couponId);
        }

        // 重写：获取所有优惠券列表
        public override async Task<List<Coupon>> GetAllAsync()
        {
            return await _context.Coupons
                .Include(c => c.User)
                .OrderByDescending(c => c.StartTime)
                .ToListAsync();
        }

        // === 业务专用查询方法 ===

        // 根据用户ID获取优惠券列表
        // <param name="userId">用户ID</param>
        // <returns>优惠券列表</returns>
        public async Task<List<Coupon>> GetByUserIdAsync(
            string userId,
            int? size,int?page)
        {
            int pageSize = size??20;
            int currentPage = page ?? 1;
            return await _context.Coupons
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.StartTime)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // 根据类型获取优惠券列表 - 根据面额分类
        // <param name="couponType">优惠券类型 (可以按面额范围分类)</param>
        // <returns>优惠券列表</returns>
        public async Task<List<Coupon>> GetByTypeAsync(int couponType)
        {
            // 按面额范围分类: 1=小额(0-50), 2=中额(50-200), 3=大额(200+)
            return couponType switch
            {
                1 => await _context.Coupons
                    .Where(c => c.FaceValue <= 50)
                    .OrderByDescending(c => c.StartTime)
                    .ToListAsync(),
                2 => await _context.Coupons
                    .Where(c => c.FaceValue > 50 && c.FaceValue <= 200)
                    .OrderByDescending(c => c.StartTime)
                    .ToListAsync(),
                3 => await _context.Coupons
                    .Where(c => c.FaceValue > 200)
                    .OrderByDescending(c => c.StartTime)
                    .ToListAsync(),
                _ => await _context.Coupons
                    .OrderByDescending(c => c.StartTime)
                    .ToListAsync()
            };
        }

        // 根据状态获取优惠券列表 - 基于时间和使用情况判断状态
        // <param name="status">状态 (1=有效, 2=已过期, 3=已用完)</param>
        // <returns>优惠券列表</returns>
        public async Task<List<Coupon>> GetByStatusAsync(int status)
        {
            var now = DateTime.Now;
            return status switch
            {
                1 => await _context.Coupons // 有效
                    .Where(c => c.StartTime <= now && c.EndTime >= now && c.IssuedQty < c.TotalQty)
                    .OrderByDescending(c => c.StartTime)
                    .ToListAsync(),
                2 => await _context.Coupons // 已过期
                    .Where(c => c.EndTime < now)
                    .OrderByDescending(c => c.StartTime)
                    .ToListAsync(),
                3 => await _context.Coupons // 已用完
                    .Where(c => c.IssuedQty >= c.TotalQty)
                    .OrderByDescending(c => c.StartTime)
                    .ToListAsync(),
                _ => await _context.Coupons
                    .OrderByDescending(c => c.StartTime)
                    .ToListAsync()
            };
        }

        // 根据优惠券代码获取优惠券 - 使用 CouponId 作为代码
        // <param name="couponCode">优惠券代码</param>
        // <returns>优惠券实体，如果不存在则返回null</returns>
        public async Task<Coupon?> GetByCouponCodeAsync(string couponCode)
        {
            return await _context.Coupons
                .FirstOrDefaultAsync(c => c.CouponId == couponCode);
        }

        // 获取有效的优惠券列表
        // <returns>有效优惠券列表</returns>
        public async Task<List<Coupon>> GetValidCouponsAsync()
        {
            var now = DateTime.Now;
            return await _context.Coupons
                .Where(c => c.StartTime <= now &&
                           c.EndTime >= now &&
                           c.IssuedQty < c.TotalQty)
                .OrderByDescending(c => c.StartTime)
                .ToListAsync();
        }

        // 根据用户ID获取有效优惠券列表
        // <param name="userId">用户ID</param>
        // <returns>有效优惠券列表</returns>
        public async Task<List<Coupon>> GetValidByUserIdAsync(
            string userId,
            int?size,int?page)
        {
            int pageSize = size??20;
            int currentPage = page ?? 1;
            var now = DateTime.Now;
            return await _context.Coupons
                .Where(c => c.UserId == userId &&
                           c.StartTime <= now &&
                           c.EndTime >= now &&
                           c.IssuedQty < c.TotalQty)
                .OrderByDescending(c => c.StartTime)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // 获取过期优惠券列表
        // <returns>过期优惠券列表</returns>
        public async Task<List<Coupon>> GetExpiredByUserIdAsync(
            string userId,
            int?size,int?page)
        {
            int pageSize = size??20;
            int currentPage = page ?? 1;
            var now = DateTime.Now;
            return await _context.Coupons
                .Where(c => c.EndTime < now)
                .OrderByDescending(c => c.EndTime)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // 检查优惠券是否有效
        // <param name="couponId">优惠券ID</param>
        // <returns>优惠券是否有效</returns>
        public async Task<bool> IsValidAsync(string couponId)
        {
            var now = DateTime.Now;
            return await _context.Coupons
                .AnyAsync(c => c.CouponId == couponId &&
                              c.StartTime <= now &&
                              c.EndTime >= now &&
                              c.IssuedQty < c.TotalQty);
        }
        
        public async Task<List<Coupon>> GetValidByUserIdAndAmountAsync(
            string userId, 
            decimal amount,
            int? size,int? page)
        {
            int pageSize = size??20;
            int currentPage = page ?? 1;
            var now = DateTime.Now;
            return await _context.Coupons
                .Where(c => c.UserId == userId &&
                            c.StartTime <= now &&
                            c.EndTime >= now &&
                            c.IssuedQty < c.TotalQty &&
                            amount >= c.Threshold)
                .OrderByDescending(c => c.FaceValue)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
