using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Admin
{
    using JISpeed.Core.Entities.Common;
    using JISpeed.Core.Entities.Merchant;
    
    //管理员实体 - 对应数据库表: Admin
    [Table("ADMIN")]
    public class Admin
    {
        [Key]
        [StringLength(450)]
        [Column(TypeName = "VARCHAR(450)")]
        public required string AdminId { get; set; }
        
        [StringLength(32)]
        public required string PermissionLevel { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // 身份验证用户关联
        public string? ApplicationUserId { get; set; }
        
        // 导航属性
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser? ApplicationUser { get; set; }

        public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
        public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

        // 主构造函数
        public Admin(string adminId, string permissionLevel, string? applicationUserId = null)
        {
            AdminId = adminId;
            PermissionLevel = permissionLevel;
            ApplicationUserId = applicationUserId;
            CreatedAt = DateTime.Now;
        }
        
        // 用于 EF Core 的私有构造函数
        public Admin() { }
    }
}
