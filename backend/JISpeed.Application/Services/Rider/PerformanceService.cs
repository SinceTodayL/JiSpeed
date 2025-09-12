using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Interfaces.IRepositories.Rider;
using JISpeed.Core.Interfaces.IRepositories.Order;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Constants;
using Microsoft.Extensions.Logging;

namespace JISpeed.Application.Services.Rider
{
    public class PerformanceService : IPerformanceService
    {
        private readonly IPerformanceRepository _performanceRepository;
        private readonly IRiderRepository _riderRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly ILogger<PerformanceService> _logger;

        public PerformanceService(
            IPerformanceRepository performanceRepository,
            IRiderRepository riderRepository,
            IAssignmentRepository assignmentRepository,
            IReviewRepository reviewRepository,
            ILogger<PerformanceService> logger)
        {
            _performanceRepository = performanceRepository;
            _riderRepository = riderRepository;
            _assignmentRepository = assignmentRepository;
            _reviewRepository = reviewRepository;
            _logger = logger;
        }

        // 获取骑手月度绩效
        public async Task<Performance?> GetRiderMonthlyPerformanceAsync(string riderId, DateTime month)
        {
            try
            {
                _logger.LogInformation("开始获取骑手月度绩效, RiderId: {RiderId}, Month: {Month}",
                    riderId, month.ToString("yyyy-MM"));

                // 参数验证
                if (string.IsNullOrWhiteSpace(riderId))
                {
                    throw CommonExceptions.ValidationFailed("riderId", "骑手ID不能为空");
                }

                // 检查骑手是否存在
                var riderExists = await _riderRepository.GetByIdAsync(riderId) != null;
                if (!riderExists)
                {
                    _logger.LogWarning("骑手不存在, RiderId: {RiderId}", riderId);
                    throw RiderExceptions.RiderNotFound(riderId);
                }

                // 标准化月份（只保留年月，日设为1）
                var standardizedMonth = new DateTime(month.Year, month.Month, 1);

                // 直接获取Performance表中的数据
                var performance = await _performanceRepository.GetByCompositeKeyAsync(riderId, standardizedMonth);

                _logger.LogInformation("成功获取骑手月度绩效, RiderId: {RiderId}, Month: {Month}, Found: {Found}",
                    riderId, standardizedMonth.ToString("yyyy-MM"), performance != null);

                return performance;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取骑手月度绩效时发生异常, RiderId: {RiderId}, Month: {Month}",
                    riderId, month.ToString("yyyy-MM"));
                throw CommonExceptions.GeneralError($"获取骑手月度绩效失败: {ex.Message}");
            }
        }

        // 获取骑手绩效趋势（最近几个月）
        public async Task<IEnumerable<Performance>> GetRiderPerformanceTrendAsync(string riderId, int monthCount = 6)
        {
            try
            {
                _logger.LogInformation("开始获取骑手绩效趋势, RiderId: {RiderId}, MonthCount: {MonthCount}",
                    riderId, monthCount);

                // 参数验证
                if (string.IsNullOrWhiteSpace(riderId))
                {
                    throw CommonExceptions.ValidationFailed("riderId", "骑手ID不能为空");
                }

                if (monthCount <= 0)
                {
                    throw CommonExceptions.ValidationFailed("monthCount", "月份数量必须大于0");
                }

                // 检查骑手是否存在
                var riderExists = await _riderRepository.GetByIdAsync(riderId) != null;
                if (!riderExists)
                {
                    _logger.LogWarning("骑手不存在, RiderId: {RiderId}", riderId);
                    throw RiderExceptions.RiderNotFound(riderId);
                }

                // 计算开始月份（当前月份往前推monthCount个月）
                var endMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var startMonth = endMonth.AddMonths(-monthCount + 1);

                // 直接查询Performance表中的数据
                var performances = new List<Performance>();

                for (var month = startMonth; month <= endMonth; month = month.AddMonths(1))
                {
                    var performance = await _performanceRepository.GetByCompositeKeyAsync(riderId, month);
                    if (performance != null)
                    {
                        performances.Add(performance);
                    }
                }

                _logger.LogInformation("成功获取骑手绩效趋势, RiderId: {RiderId}, MonthCount: {MonthCount}, RecordCount: {RecordCount}",
                    riderId, monthCount, performances.Count());

                return performances.OrderByDescending(p => p.StatsMonth);
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取骑手绩效趋势时发生异常, RiderId: {RiderId}, MonthCount: {MonthCount}",
                    riderId, monthCount);
                throw CommonExceptions.GeneralError($"获取骑手绩效趋势失败: {ex.Message}");
            }
        }

        // 获取骑手绩效排名
        public async Task<Dictionary<string, int>> GetRiderRankingAsync(string riderId, DateTime month)
        {
            try
            {
                _logger.LogInformation("开始获取骑手绩效排名, RiderId: {RiderId}, Month: {Month}",
                    riderId, month.ToString("yyyy-MM"));

                // 参数验证
                if (string.IsNullOrWhiteSpace(riderId))
                {
                    throw CommonExceptions.ValidationFailed("riderId", "骑手ID不能为空");
                }

                // 检查骑手是否存在
                var riderExists = await _riderRepository.GetByIdAsync(riderId) != null;
                if (!riderExists)
                {
                    _logger.LogWarning("骑手不存在, RiderId: {RiderId}", riderId);
                    throw RiderExceptions.RiderNotFound(riderId);
                }

                // 标准化月份（只保留年月，日设为1）
                var standardizedMonth = new DateTime(month.Year, month.Month, 1);

                // 获取排名数据
                var rankings = await _performanceRepository.GetRiderRankingAsync(riderId, standardizedMonth);

                _logger.LogInformation("成功获取骑手绩效排名, RiderId: {RiderId}, Month: {Month}",
                    riderId, standardizedMonth.ToString("yyyy-MM"));

                return rankings;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取骑手绩效排名时发生异常, RiderId: {RiderId}, Month: {Month}",
                    riderId, month.ToString("yyyy-MM"));
                throw CommonExceptions.GeneralError($"获取骑手绩效排名失败: {ex.Message}");
            }
        }

        // 获取绩效优秀骑手列表
        public async Task<IEnumerable<Performance>> GetTopPerformersAsync(DateTime month, int topCount = 10)
        {
            try
            {
                _logger.LogInformation("开始获取绩效优秀骑手列表, Month: {Month}, TopCount: {TopCount}",
                    month.ToString("yyyy-MM"), topCount);

                // 参数验证
                if (topCount <= 0)
                {
                    throw CommonExceptions.ValidationFailed("topCount", "返回数量必须大于0");
                }

                // 标准化月份（只保留年月，日设为1）
                var standardizedMonth = new DateTime(month.Year, month.Month, 1);

                // 直接获取Performance表中的数据
                var topPerformers = await _performanceRepository.GetTopPerformersByMonthAsync(standardizedMonth, topCount);

                _logger.LogInformation("成功获取绩效优秀骑手列表, Month: {Month}, TopCount: {TopCount}, RecordCount: {RecordCount}",
                    standardizedMonth.ToString("yyyy-MM"), topCount, topPerformers.Count());

                return topPerformers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取绩效优秀骑手列表时发生异常, Month: {Month}, TopCount: {TopCount}",
                    month.ToString("yyyy-MM"), topCount);
                throw CommonExceptions.GeneralError($"获取绩效优秀骑手列表失败: {ex.Message}");
            }
        }

        // 获取月度绩效概览
        public async Task<Dictionary<string, object>> GetMonthlyPerformanceOverviewAsync(DateTime month)
        {
            try
            {
                _logger.LogInformation("开始获取月度绩效概览, Month: {Month}", month.ToString("yyyy-MM"));

                // 标准化月份（只保留年月，日设为1）
                var standardizedMonth = new DateTime(month.Year, month.Month, 1);

                // 直接获取Performance表中的概览数据
                var overview = await _performanceRepository.GetMonthlyPerformanceOverviewAsync(standardizedMonth);

                _logger.LogInformation("成功获取月度绩效概览, Month: {Month}", standardizedMonth.ToString("yyyy-MM"));

                return overview;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取月度绩效概览时发生异常, Month: {Month}", month.ToString("yyyy-MM"));
                throw CommonExceptions.GeneralError($"获取月度绩效概览失败: {ex.Message}");
            }
        }

        // 计算并生成月度绩效数据
        public async Task<Performance> GenerateMonthlyPerformanceAsync(string riderId, DateTime month)
        {
            try
            {
                _logger.LogInformation("开始计算并生成月度绩效数据, RiderId: {RiderId}, Month: {Month}",
                    riderId, month.ToString("yyyy-MM"));

                // 参数验证
                if (string.IsNullOrWhiteSpace(riderId))
                {
                    throw CommonExceptions.ValidationFailed("riderId", "骑手ID不能为空");
                }

                // 检查骑手是否存在
                var rider = await _riderRepository.GetByIdAsync(riderId);
                if (rider == null)
                {
                    _logger.LogWarning("骑手不存在, RiderId: {RiderId}", riderId);
                    throw RiderExceptions.RiderNotFound(riderId);
                }

                // 标准化月份（只保留年月，日设为1）
                var standardizedMonth = new DateTime(month.Year, month.Month, 1);

                // 检查是否已存在绩效记录
                var existingPerformance = await _performanceRepository.GetByCompositeKeyAsync(riderId, standardizedMonth);

                // 计算月份的开始和结束日期
                var startDate = standardizedMonth;
                var endDate = standardizedMonth.AddMonths(1).AddDays(-1);

                // 获取该月骑手的所有订单分配
                var allAssignments = await _assignmentRepository.GetByRiderIdAsync(riderId);
                var monthlyAssignments = allAssignments
                     .Where(a => a.AssignedAt >= startDate && a.AssignedAt <= endDate)
                     .Where(a => a.AcceptedStatus == 1) // 只计算已接单的订单
                     .ToList();

                // 计算绩效指标
                int totalOrders = monthlyAssignments.Count;
                decimal onTimeRate = 0;
                decimal goodReviewRate = 0;
                decimal badReviewRate = 0;
                decimal income = 0;

                if (totalOrders > 0)
                {
                    // 计算准时率 - 基于实际超时数据
                    var onTimeOrders = monthlyAssignments.Count(a => a.TimeOut == null || a.TimeOut <= 0);
                    onTimeRate = (decimal)onTimeOrders / totalOrders * 100;

                    // 计算收入 - 从订单中获取实际金额，骑手获得订单金额的10%
                    income = monthlyAssignments.Sum(a => a.Order?.OrderAmount ?? 0) * 0.1m;

                    // 获取订单评价数据 - 优化：使用批量查询替代N+1查询
                    var orderIds = monthlyAssignments
                        .Select(a => a.Order?.OrderId)
                        .Where(id => id != null)
                        .Cast<string>()
                        .ToList();

                    if (orderIds.Any())
                    {
                        // 优化：批量查询所有评价，避免N+1查询
                        var reviews = await _reviewRepository.GetByOrderIdsAsync(orderIds);

                        // 计算好评率和差评率
                        if (reviews.Any())
                        {
                            // 4-5星为好评，1-2星为差评，3星为中性评价
                            var goodReviews = reviews.Count(r => r.Rating >= 4);
                            var badReviews = reviews.Count(r => r.Rating <= 2);
                            var totalReviews = reviews.Count;

                            goodReviewRate = (decimal)goodReviews / totalReviews * 100;
                            badReviewRate = (decimal)badReviews / totalReviews * 100;

                            _logger.LogInformation("骑手 {RiderId} 在 {Month} 的评价统计: 总评价数={TotalReviews}, 好评数={GoodReviews}, 差评数={BadReviews}, 好评率={GoodReviewRate}%, 差评率={BadReviewRate}%",
                                riderId, standardizedMonth.ToString("yyyy-MM"), totalReviews, goodReviews, badReviews, goodReviewRate, badReviewRate);
                        }
                        else
                        {
                            _logger.LogInformation("骑手 {RiderId} 在 {Month} 没有找到任何评价数据", riderId, standardizedMonth.ToString("yyyy-MM"));
                            goodReviewRate = 0;
                            badReviewRate = 0;
                        }
                    }
                    else
                    {
                        _logger.LogInformation("骑手 {RiderId} 在 {Month} 没有关联的订单", riderId, standardizedMonth.ToString("yyyy-MM"));
                        goodReviewRate = 0;
                        badReviewRate = 0;
                    }
                }

                // 创建或更新绩效记录
                var performance = existingPerformance ?? new Performance()
                {
                    RiderId = riderId,
                    StatsMonth = standardizedMonth,
                    TotalOrders = totalOrders,
                    OnTimeRate = onTimeRate,
                    GoodReviewRate = goodReviewRate,
                    BadReviewRate = badReviewRate,
                    Income = income,
                    Rider = rider
                };

                if (existingPerformance == null)
                {
                    await _performanceRepository.CreateAsync(performance);
                }
                else
                {
                    // 更新现有记录
                    existingPerformance.TotalOrders = totalOrders;
                    existingPerformance.OnTimeRate = onTimeRate;
                    existingPerformance.GoodReviewRate = goodReviewRate;
                    existingPerformance.BadReviewRate = badReviewRate;
                    existingPerformance.Income = income;
                    await _performanceRepository.UpdateAsync(existingPerformance);
                    performance = existingPerformance;
                }

                _logger.LogInformation("成功计算并生成月度绩效数据, RiderId: {RiderId}, Month: {Month}, TotalOrders: {TotalOrders}, Income: {Income}",
                    riderId, standardizedMonth.ToString("yyyy-MM"), totalOrders, income);

                return performance;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "计算并生成月度绩效数据时发生异常, RiderId: {RiderId}, Month: {Month}",
                    riderId, month.ToString("yyyy-MM"));
                throw CommonExceptions.GeneralError($"计算并生成月度绩效数据失败: {ex.Message}");
            }
        }
    }
}
