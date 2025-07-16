using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Common
{
    //公告实体
    //对应数据库表: Announcement
    [Table("Announcement")]
    public class Announcement
    {
        [Key]
        [Column(TypeName = "CHAR(32)")]
        public required string AnnounceId { get; set; } //公告ID pk

        [Required]
        [StringLength(20)]
        public required string Title { get; set; } //标题

        [StringLength(65535)] // TEXT 类型
        public string? Content { get; set; } //正文

        [StringLength(20)]
        public string? TargetRole { get; set; } //目标角色 (可为空)

        public required DateTime StartAt { get; set; } //生效起

        public required DateTime EndAt { get; set; } //生效止
    }
}