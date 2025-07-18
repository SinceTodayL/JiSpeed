using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Admin
{
    //公告实体
    //对应数据库表: Announcement
    [Table("ANNOUNCEMENT")]
    public class Announcement
    {
        [Key]
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string AnnounceId { get; set; } //公告ID pk

        [ForeignKey("AdminId")]
        public required string AdminId { get; set; } //管理员ID fk->Admin(adminId) 

        [StringLength(20)]
        public required string Title { get; set; } //标题

        [StringLength(65535)] // TEXT 类型
        public string? Content { get; set; } //正文

        [StringLength(20)]
        public string? TargetRole { get; set; } //目标角色 (可为空)

        public required DateTime StartAt { get; set; } //生效起

        public required DateTime EndAt { get; set; } //生效止

        [ForeignKey("AdminId")]
        public virtual required Admin Admin { get; set; } //关联到 Admin 实体

        public Announcement(string adminId, string title, string content, DateTime startAt, DateTime endAt, string? targetRole = null)
        {
            AnnounceId = Guid.NewGuid().ToString("N"); //生成唯一的公告编号
            AdminId = adminId;
            Title = title;
            Content = content;
            StartAt = startAt;
            EndAt = endAt;
            TargetRole = targetRole;
        }

        private Announcement() { }
    }
}