using JISpeed.Core.Entities.Admin;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Constants;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IRepositories.Admin;
using Microsoft.Extensions.Logging;

namespace JISpeed.Application.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
            IAdminRepository adminRepository, 
            ILogger<AdminService> logger,
            IAnnouncementRepository announcementRepository)
        {
            _adminRepository = adminRepository;
            _announcementRepository = announcementRepository;
            _logger = logger;
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

    }

}