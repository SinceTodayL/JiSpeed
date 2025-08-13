using JISpeed.Core.Entities.Admin;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Constants;
using JISpeed.Core.Entities.Reconciliation;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IRepositories.Admin;
using JISpeed.Core.Interfaces.IRepositories.Order;
using JISpeed.Core.Interfaces.IRepositories.Reconciliation;
using JISpeed.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace JISpeed.Application.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly IReconciliationRepository _reconciliationRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<AdminService> _logger;
        private readonly OracleDbContext _context;

        public AdminService(
            IAdminRepository adminRepository, 
            ILogger<AdminService> logger,
            IAnnouncementRepository announcementRepository,
            IReconciliationRepository reconciliationRepository,
            IOrderRepository orderRepository,
            OracleDbContext context)
        {
            _adminRepository = adminRepository;
            _announcementRepository = announcementRepository;
            _reconciliationRepository = reconciliationRepository;
            _orderRepository = orderRepository;
            _logger = logger;
            _context = context;
        }

        public async Task<bool> CreateAnnouncementEntityAsync(string adminId, string title, string? content,
            string? targetRole, DateTime startAt, DateTime endAt)
        {
            try
            {
                _logger.LogInformation("开始新增公告的请求, AdminId: {AdminId}", adminId);

                var adminEntity = await _adminRepository.GetByIdAsync(adminId);

                if (adminEntity == null)
                {
                    _logger.LogWarning("管理员不存在, AdminId: {AdminId}", adminId);
                    throw new NotFoundException(ErrorCodes.MerchantNotFound, $"无相关数据，ID: {adminId}");
                }

                var announcement = new Announcement()
                {
                    AnnounceId = Guid.NewGuid().ToString("N"),
                    Title = title,
                    Content = content,
                    TargetRole = targetRole,
                    StartAt = startAt,
                    EndAt = endAt,
                    AdminId = adminId,
                    Admin = adminEntity
                };
                var res = await _announcementRepository.CreateAsync(announcement);
                if (res==null)
                {
                    _logger.LogWarning("创建失败");
                    throw new BusinessException("创建新公告失败");
                }
                await _announcementRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "新增公告时发生异常, AdminId: {AdminId}", adminId);
                throw new BusinessException("新增公告失败");
            }
        }

        public async Task<Announcement> GetAnnouncementDetailAsync(string announcementId)
        {
            try
            {
                _logger.LogInformation("开始获取公告的请求");
                var announcements = await _announcementRepository.GetByIdAsync(announcementId);
                if (announcements==null)
                {
                    _logger.LogWarning("获取失败");
                    throw new NotFoundException(ErrorCodes.ResourceNotFound,"无相关公告");
                }
                return announcements;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取公告时发生异常");
                throw new BusinessException("获取公告失败");
            }
        }

        public async Task<List<Announcement>> GetActiveAnnouncementByUserTypeAsync(
            string targetRole,
            int? size,int? page)
        {
            try
            {
                _logger.LogInformation("开始获取有效公告的请求");
                List<Announcement> ?announcements;
                if (targetRole=="All")
                    announcements = await _announcementRepository.GetAllActiveAnnouncementsAsync(size, page);
                else 
                    announcements = await _announcementRepository.GetActiveAnnouncementsByUserTypeAsync(targetRole, size, page);
                if (announcements==null)
                {
                    _logger.LogWarning("获取失败");
                    throw new BusinessException("获取公告失败");
                }
                return announcements;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取公告时发生异常");
                throw new BusinessException("获取公告失败");
            }
        }

        public async Task<List<Announcement>> GetAllAnnouncementByUserTypeAsync(
            string targetRole,
            int? size, int? page)
        {
            try
            {
                _logger.LogInformation("开始获取全部公告的请求");
                List<Announcement> ?announcements;
                if (targetRole=="All")
                    announcements = await _announcementRepository.GetAllAnnouncementsAsync(size, page);
                else 
                    announcements = await _announcementRepository.GetAllAnnouncementsByUserTypeAsync(targetRole, size, page);
                if (announcements==null)
                {
                    _logger.LogWarning("获取失败");
                    throw new BusinessException("获取公告失败");
                }
                return announcements;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取公告时发生异常");
                throw new BusinessException("获取公告失败");
            }
        }

        public async Task<bool> ModifyAnnouncementAsync(
            string announcementId,
            string? title, string? content, string? targetRole,
            DateTime? startAt, DateTime? endAt)
        {
            try
            {
                _logger.LogInformation("开始修改公告的请求");
                var entity = await _announcementRepository.GetByIdAsync(announcementId);
                if (entity == null)
                {
                    _logger.LogWarning("公告不存在, AnnouncementId: {AnnouncementId}", announcementId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据，ID: {announcementId}");
                }
                entity.Title = title ?? entity.Title;
                entity.Content = content ?? entity.Content;
                entity.TargetRole = targetRole ?? entity.TargetRole;
                entity.StartAt = startAt ?? entity.StartAt;
                entity.EndAt = endAt ?? entity.EndAt;
                await _announcementRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取公告时发生异常");
                throw new BusinessException("获取公告失败");
            }
        }

        public async Task<bool> CreateReconciliationEntityAsync(
            DateTime periodStart, DateTime periodEnd,
            int reconType, List<string> orderIds)
        {
            try
            {
                _logger.LogInformation("开始新增对账异常的请求");
                if (!orderIds.Any())
                {
                    _logger.LogWarning("至少要填入一个orderId");
                    throw new BusinessException(ErrorCodes.ResourceNotFound, "至少要填入一个orderId");
                }

                decimal diffAmount = 0;
                // 计算order和payment的差值，TODO：后续要加上cupon的折扣
                foreach (var orderId in orderIds)
                {
                    var orderEntity = await _orderRepository.GetWithDetailsAsync(orderId);
                    if (orderEntity == null)
                    {
                        _logger.LogInformation($"无该订单,orderId: {orderIds}");
                        continue;
                    }
                    decimal amount = 0;
                    foreach (var payment in orderEntity.Payments)
                    {
                        int temp = 0;
                        if(payment.PayStatus == (int)PayStatus.Paid)
                        {
                            amount += payment.PayAmount;
                            temp++;
                        }
                        if(temp > 1)
                            _logger.LogWarning($"该order多次支付，ID：{orderId}");
                    }
                    diffAmount += amount -orderEntity.OrderAmount;
                }
                
                
                var reconciliation = new Reconciliation()
                {
                    ReconId = Guid.NewGuid().ToString("N"),
                    PeriodStart = periodStart,
                    PeriodEnd = periodEnd,
                    ReconType = reconType,
                    FoundAt = DateTime.Now,
                    DiffAmount = diffAmount,
                    AffectedOrders = orderIds.Count,
                    IsResolved = false
                };
                var res = await _reconciliationRepository.CreateAsync(reconciliation);
                if (res==null)
                {
                    _logger.LogWarning("创建失败");
                    throw new BusinessException("创建新对账异常实体失败");
                }
                _context.Reconciliations.Add(reconciliation);
                await _reconciliationRepository.SaveChangesAsync();

                foreach (var orderId in orderIds)
                {
                    var orderEntity = await _orderRepository.GetWithDetailsAsync(orderId);
                    if (orderEntity == null)
                    {
                        _logger.LogInformation($"无该订单,orderId: {orderId}");
                        continue;
                    }
                    orderEntity.ReconId = reconciliation.ReconId;
                    reconciliation.Orders.Add(orderEntity); // 双向关联

                }

                await _orderRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException||ex is BusinessException))
            {
                _logger.LogError(ex, "新增对账异常实体时发生异常");
                throw new BusinessException("新增对账异常实体失败");
            }
        }

        public async Task<Reconciliation> GetReconciliationDetailAsync(string reconId)
        {
            try
            {
                _logger.LogInformation("开始获取对账异常主体的请求");
                var entity = await _reconciliationRepository.GetByIdAsync(reconId);
               if (entity==null)
                {
                    _logger.LogWarning("获取失败");
                    throw new NotFoundException(ErrorCodes.ResourceNotFound,"获取对账异常主体失败,对象不存在");
                }
                return entity;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException||ex is BusinessException))
            {
                _logger.LogError(ex, "获取对账异常主体时发生异常");
                throw new BusinessException("获取对账异常主体失败");
            }
        }

        public async Task<List<Reconciliation>> GetReconciliationByFilterAsync(
            bool? isResolved, 
            int? reconType,
            int? size, int? page)
        {
            try
            {
                List<Reconciliation>? reconciliations;
                _logger.LogInformation("开始获取对账异常主体列表的请求");
                if (isResolved.HasValue)
                {
                    reconciliations = await _reconciliationRepository.GetByResolvedStatusAsync(isResolved.Value,size,page);
                }
                else if (reconType != null)
                {
                    reconciliations = await _reconciliationRepository.GetByReconTypeAsync(reconType.Value,size,page);
                }
                else
                    reconciliations = await _reconciliationRepository.GetAllAsync(size,page);
                if (reconciliations==null)
                {
                    _logger.LogWarning("获取失败");
                    throw new BusinessException("获取对账异常主体列表失败");
                }
                return reconciliations;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException||ex is BusinessException))
            {
                _logger.LogError(ex, "获取对账异常主体列表时发生异常");
                throw new BusinessException("获取对账异常主体列表失败");
            }
        }

        public async Task<bool> ResolveReconciliationAsync(string reconId)
        {
            try
            {
                _logger.LogInformation("开始解决对账异常主体列表的请求");
                var  entity = await _reconciliationRepository.GetByIdAsync(reconId);
                if (entity==null)
                {
                    _logger.LogWarning("获取失败");
                    throw new NotFoundException(ErrorCodes.ResourceNotFound,"获取对账异常主体列表失败");
                }
                entity.IsResolved = true;
                await _reconciliationRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException||ex is BusinessException))
            {
                _logger.LogError(ex, "解决对账异常主体列表时发生异常");
                throw new BusinessException("解决对账异常主体列表失败");
            }
        }


    }

}