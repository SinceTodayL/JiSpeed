using System.Runtime.InteropServices.JavaScript;
using JISpeed.Core.Constants;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IRepositories.Admin;
using JISpeed.Core.Interfaces.IRepositories.Common;
using JISpeed.Core.Interfaces.IRepositories.User;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.Extensions.Logging;

namespace JISpeed.Application.Services.Common
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAdminRepository _adminRepository;
        Random _random = new Random();

        private readonly ILogger<CouponService> _logger;
        
        public CouponService(
            ICouponRepository couponRepository,
            IUserRepository userRepository,
            IAdminRepository adminRepository,
            ILogger<CouponService> logger
        )
        {
            _couponRepository= couponRepository;
            _userRepository= userRepository;
            _adminRepository = adminRepository;
            _logger = logger;
        }

        public async Task<List<string>> GetCouponIdByFilterAsync(
            string userId,
            bool? isActive,
            decimal? amount,
            int? size,int? page)
        {
            try
            {
                var users = await _userRepository.GetByIdAsync(userId);
                if (users == null)
                {
                    throw new NotFoundException(ErrorCodes.UserNotFound, "无该用户");
                }
                _logger.LogInformation("开始获取用户优惠券统计信息");
                List<Coupon>? coupons;
                if (isActive.HasValue && isActive.Value)
                    //  获取目前可用的优惠券
                    coupons = await _couponRepository.GetValidByUserIdAsync(userId, size, page);
                else if (isActive.HasValue && !isActive.Value)
                    // 获取目前过期的优惠券
                    coupons = await _couponRepository.GetExpiredByUserIdAsync(userId,size,page);
                else if (amount.HasValue)
                    // 获取满足满减要求的可用优惠券
                    coupons = await _couponRepository.GetValidByUserIdAndAmountAsync(userId, amount.Value,size,page);
                else
                    // 不做筛选，获取该用户所有的优惠券
                    coupons = await _couponRepository.GetByUserIdAsync(userId, size,page);
                _logger.LogInformation("成功获取户优惠券统计信息");
                var couponIds = coupons.Select(c => c.CouponId).ToList();
                return couponIds;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取户优惠券统计信息时发生异常");
                throw new BusinessException("获取户优惠券统计信息失败");
            }
        }

        public async Task<Coupon> GetCouponDetailByCouponIdAsync(string couponId)
        {
            try
            {
                _logger.LogInformation("开始获取优惠券信息");
                var coupon = await _couponRepository.GetByIdAsync(couponId);
                if (coupon == null)
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, "无该优惠券");
                _logger.LogInformation("成功获取优惠券信息");
                return coupon;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取优惠券信息时发生异常");
                throw new BusinessException("获取优惠券信息失败");
            }
        }
        
        public async Task<bool> IssueCouponsAsync(
            string adminId, 
            DateTime startTime,DateTime endTime,
            string? userId,
            decimal? faceValue, decimal? threshold)
        {
            try
            {
                _logger.LogInformation("开始发放优惠券");
                var res = await _adminRepository.ExistsAsync(adminId);
                if (!res)
                {
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, "该管理员权限不足");
                }

                if (userId != null)
                {
                    var user = await _userRepository.GetByIdAsync(userId);
                    if (user==null)
                    {
                        throw new NotFoundException(ErrorCodes.UserNotFound, "无该用户");
                    }
                    var faceValueTemp = faceValue ?? _random.Next(5, 15);
                    var interval = threshold ?? _random.Next(5, 15);
                    var coupon = new Coupon()
                    {
                        CouponId = Guid.NewGuid().ToString("N"),
                        UserId = user.UserId,
                        FaceValue = faceValueTemp, //随机面额
                        Threshold = faceValueTemp + interval, // 随机门槛
                        TotalQty = 100,
                        IssuedQty = 0,
                        StartTime = startTime,
                        EndTime = endTime,
                        IsUsed = false,
                        User = user
                    };
                    await _couponRepository.CreateAsync(coupon);
                    await _couponRepository.SaveChangesAsync();
                }
                else
                {
                    var users = await _userRepository.GetAllAsync();
                    foreach (var user in users)
                    {
                        var faceValueTemp = faceValue ?? _random.Next(5, 15);
                        var interval = threshold ?? _random.Next(5, 15);
                        var coupon = new Coupon()
                        {
                            CouponId = Guid.NewGuid().ToString("N"),
                            UserId = user.UserId,
                            FaceValue = faceValueTemp, //随机面额
                            Threshold = faceValueTemp + interval, // 随机门槛
                            TotalQty = 100,
                            IssuedQty = 0,
                            StartTime = startTime,
                            EndTime = endTime,
                            IsUsed = false,
                            User = user
                        };
                        await _couponRepository.CreateAsync(coupon);
                        await _couponRepository.SaveChangesAsync();
                    }
                }

                _logger.LogInformation("成功发放优惠券信息");
                return true;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "发放优惠券时发生异常");
                throw new BusinessException("发放优惠券失败");
            }
        }


    }
}