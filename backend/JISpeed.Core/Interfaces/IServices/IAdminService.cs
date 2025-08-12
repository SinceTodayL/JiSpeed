using JISpeed.Core.Entities.Admin;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories;

namespace JISpeed.Core.Interfaces.IServices
{
    public interface IAdminService
    {
        
        Task<bool> CreateAnnouncementEntityAsync(string adminId, string title,string? content,string? targetRole,DateTime startAt,DateTime endAt);
        Task<Announcement> GetAnnouncementDetailAsync(string announcementId);

        Task<List<Announcement>> GetActiveAnnouncementByUserTypeAsync(
            string targetRole,
            int? size, int? page);
        Task<List<Announcement>> GetAllAnnouncementByUserTypeAsync(
            string targetRole,
            int? size, int? page);
        Task<bool> ModifyAnnouncementAsync(
            string announcementId,
            string? title, string? content, string? targetRole,
            DateTime? startAt,DateTime? endAt);


    }
}