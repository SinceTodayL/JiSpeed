using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Admin
{
    using JISpeed.Core.Entities.Merchant; // For Application entity

    //管理员实体
    //对应数据库表: Admin
    [Table("Admin")]
    public class Admin
    {
        [Key]
        [Column(TypeName = "CHAR(32)")]
        public required string AdminId { get; set; } //管理员ID pk 

        [Column(TypeName = "CHAR(32)")]
        public required string RoleId { get; set; } //角色ID 

        [Column(TypeName = "CHAR(32)")]
        public required string PermId { get; set; } //权限ID

        public required DateTime GrantedAt { get; set; } //授权时间

        public virtual ICollection<Application>? Applications { get; set; } = new List<Application>(); //管理员可以审批多个申请
    }
}