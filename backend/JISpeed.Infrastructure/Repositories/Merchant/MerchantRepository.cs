using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Core.Interfaces.IRepositories.Merchant;
using JISpeed.Infrastructure.Data;
using JISpeed.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using MerchantEntity = JISpeed.Core.Entities.Merchant.Merchant;

namespace JISpeed.Infrastructure.Repositories
{
    public class MerchantRepository : BaseRepository<MerchantEntity, string>, IMerchantRepository
    {
        public MerchantRepository(OracleDbContext context) : base(context)
        {
        }

        // === 业务专用查询方法 ===

        // 保留兼容性方法：GetUserWithDetailsAsync 映射到 GetWithDetailsAsync
        // <param name="merchantId">商家ID</param>
        // <returns>商家实体详情，如果不存在则返回null</returns>
        public async Task<MerchantEntity?> GetUserWithDetailsAsync(string merchantId)
        {
            return await GetWithDetailsAsync(merchantId);
        }

        // 根据ApplicationUserId获取商家信息
        // <param name="applicationUserId">应用用户ID</param>
        // <returns>商家实体，如果不存在则返回null</returns>
        public async Task<MerchantEntity?> GetByApplicationUserIdAsync(string applicationUserId)
        {
            return await _context.Merchants
                .FirstOrDefaultAsync(u => u.ApplicationUserId == applicationUserId);
        }

        // 根据商家名称搜索商家
        // <param name="name">商家名称</param>
        // <returns>商家列表</returns>
        public async Task<List<MerchantEntity>> SearchByNameAsync(string name)
        {
            return await _context.Merchants
                .Where(m => m.MerchantName.Contains(name))
                .ToListAsync();
        }

        // 根据状态获取商家列表
        // <param name="status">状态</param>
        // <returns>商家列表</returns>
        public async Task<List<MerchantEntity>> GetByStatusAsync(int status)
        {
            return await _context.Merchants
                .Where(m => m.Status == status)
                .ToListAsync();
        }

        // 根据位置搜索商家
        // <param name="location">位置</param>
        // <returns>商家列表</returns>
        public async Task<List<MerchantEntity>> SearchByLocationAsync(string location)
        {
            return await _context.Merchants
                .Where(m => !string.IsNullOrEmpty(m.Location) && m.Location.Contains(location))
                .ToListAsync();
        }
    }
}