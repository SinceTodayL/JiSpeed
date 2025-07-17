using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Admin
{
    using JISpeed.Core.Entities.Merchant; //引用 Application 实体所在的命名空间

    //管理员实体
    //对应数据库表: Admin
    [Table("Admin")]
    public class Admin
    {
        [Key]
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string AdminId { get; set; } //管理员ID pk 

        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string RoleId { get; set; } //角色ID 

        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string PermId { get; set; } //权限ID

        public required DateTime GrantedAt { get; set; } //授权时间

        public virtual ICollection<Application>? Applications { get; set; } = new List<Application>(); //管理员可以审批多个申请
        public virtual ICollection<Announcement>? Announcements { get; set; } = new List<Announcement>(); //管理员可以发布多个公告

        public Admin(string adminId, string roleId, string permId)
        {
            AdminId = adminId;
            RoleId = roleId;
            PermId = permId;
            GrantedAt = DateTime.UtcNow; //设置授权时间为当前时间
        }

        private Admin() { }
    }
}