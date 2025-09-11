namespace JISpeed.Api.DTOs
{
    // 公告发布请求
    public class CreateAnnouncementDto
    {
        public required string Title { get; set; } //标题
        public string? Content { get; set; } //正文
        public string? TargetRole { get; set; } //目标角色 (可为空)
        public required DateTime StartAt { get; set; } //生效起
        public required DateTime EndAt { get; set; } //生效止
    }
    
    public class AnnouncementResponseDto
    {
        public required string AnnounceId { get; set; } //公告ID pk
        public required string Title { get; set; } //标题
        public string? Content { get; set; } //正文
        public string? TargetRole { get; set; } //目标角色 (可为空)
        public required DateTime StartAt { get; set; } //生效起
        public required DateTime EndAt { get; set; } //生效止
        
    }
}