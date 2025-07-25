using JISpeed.Core.Entities.Merchant;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Constants;
using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Exceptions;
using Microsoft.Extensions.Logging;

namespace JISpeed.Application.Services.Merchant
{
    public class MerchantService : IMerchantService
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly ISalesStatRepository _salesStatRepository;
        private readonly IDishRepository _dishRepository;
        private readonly ILogger<MerchantService> _logger;

        public MerchantService(
            IMerchantRepository merchantRepository,
            ISalesStatRepository salesStatRepository,
            IDishRepository dishRepository,
            ILogger<MerchantService> logger
            )
        {
            _merchantRepository = merchantRepository;
            _salesStatRepository = salesStatRepository;
            _dishRepository = dishRepository;
            _logger = logger;
        }
        /// 创建用户实体（当ApplicationUser的UserType=2时调用）
        
        /// <param name="applicationUser">已创建的ApplicationUser</param>
        /// <param name="nickname">用户昵称，默认使用用户名</param>
        /// <returns>创建的Rider实体</returns>
        public async Task<JISpeed.Core.Entities.Merchant.Merchant> CreateUserEntityAsync(ApplicationUser applicationUser, string? nickname = null)
        {
            try
            {
                _logger.LogInformation("开始创建用户实体, ApplicationUserId: {ApplicationUserId}", applicationUser.Id);
            
                // 参数验证
                if (applicationUser == null)
                {
                    throw new ValidationException("ApplicationUser不能为空");
                }
            
                if (applicationUser.UserType != 2)
                {
                    throw new ValidationException($"UserType必须为2，当前值: {applicationUser.UserType}");
                }
            
                // 检查是否已存在关联的Rider实体
                //var existingUser = await _riderRepository.GetUserByApplicationUserIdAsync(applicationUser.Id);
                // if (existingUser != null)
                // {
                //     _logger.LogWarning("用户实体已存在, ApplicationUserId: {ApplicationUserId}", applicationUser.Id);
                //     throw new BusinessException("用户实体已存在");
                // }
            
                // 生成用户ID和昵称
                var userId = Guid.NewGuid().ToString("N");
                var userNickname = nickname ?? applicationUser.UserName ?? "用户" + userId.Substring(0, 8);
            
                // 创建User实体
                var user = new Core.Entities.Merchant.Merchant
                {
                    MerchantId = userId,
                    MerchantName = userNickname, 
                    ContactInfo = applicationUser.Email,
                    ApplicationUserId = applicationUser.Id
                };
                
                // 保存到数据库
                await _merchantRepository.CreateAsync(user);
                await _merchantRepository.SaveChangesAsync(); _logger.LogInformation("用户实体创建成功, UserId: {UserId}, ApplicationUserId: {ApplicationUserId}",
                    user.MerchantId, applicationUser.Id);
            
                return user;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is BusinessException))
            {
                _logger.LogError(ex, "创建用户实体时发生异常, ApplicationUserId: {ApplicationUserId}",
                    applicationUser?.Id);
                throw new BusinessException("创建用户实体失败");
            }
        }

        public async Task<Core.Entities.Merchant.Merchant?> GetMerchantDetailAsync(string merchantId)
        {
            try
            {
                _logger.LogInformation("开始获取商家详细信息, MerchantId: {MerchantId}", merchantId);

                if (string.IsNullOrWhiteSpace(merchantId))
                {
                    _logger.LogWarning("商家ID为空");
                    throw new ValidationException("商家ID不能为空");
                }

                var user = await _merchantRepository.GetUserWithDetailsAsync(merchantId);

                if (user == null)
                {
                    _logger.LogWarning("商家不存在, MerchantId: {MerchantId}", merchantId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"商家不存在，ID: {merchantId}");
                }

                _logger.LogInformation("成功获取商家详细信息, MerchantId: {MerchantId}, MerchantName: {MerchantName}",
                    merchantId, user.MerchantName);

                return user;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取商家详细信息时发生异常, MerchantId: {MerchantId}", merchantId);
                throw new BusinessException("获取商家信息失败");
            }
        }

        public async Task<List<SalesStat>> GetSalesStateAsync(string merchantId)
        {
            try
            {
                _logger.LogInformation("开始获取商家数据统计信息, MerchantId: {MerchantId}", merchantId);

                if (string.IsNullOrWhiteSpace(merchantId))
                {
                    _logger.LogWarning("商家ID为空");
                    throw new ValidationException("商家ID不能为空");
                }

                var data = await _salesStatRepository.GetDetailsAsync(merchantId);

                if (data == null ||!data.Any())
                {
                    _logger.LogWarning("无相关数据, MerchantId: {MerchantId}", merchantId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据，ID: {merchantId}");
                }

                _logger.LogInformation("成功获取商家数据统计信息, MerchantId: {MerchantId}", merchantId);

                return data;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取商家数据统计信息时发生异常, MerchantId: {MerchantId}", merchantId);
                throw new BusinessException("获取商家数据统计信息失败");
            }
        }

        public async Task<List<Dish>> GetDishesAsync(string merchantId)
        {
            try
            {
                _logger.LogInformation("开始获取商家菜品统计信息, MerchantId: {MerchantId}", merchantId);

                if (string.IsNullOrWhiteSpace(merchantId))
                {
                    _logger.LogWarning("商家ID为空");
                    throw new ValidationException("商家ID不能为空");
                }

                var data = await _dishRepository.GetDetailsAsync(merchantId);

                if (data == null ||!data.Any())
                {
                    _logger.LogWarning("无相关数据, MerchantId: {MerchantId}", merchantId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据，ID: {merchantId}");
                }

                _logger.LogInformation("成功获取商家菜品统计信息, MerchantId: {MerchantId}", merchantId);

                return data;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取商家菜品统计信息时发生异常, MerchantId: {MerchantId}", merchantId);
                throw new BusinessException("获取商家菜品统计信息失败");
            }
        }

    }

}