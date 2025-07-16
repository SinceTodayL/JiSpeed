using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Core.Entities.User
{
    using JISpeed.Core.Entities.Order; //引用 Order 命名空间下的 Complaint 和 Review

    //用户实体
    //对应数据库表: User
    [Table("User")]
    [Index(nameof(Account), IsUnique = true)]
    public class User
    {
        [Key]
        [Column(TypeName = "CHAR(32)")]
        public required string UserId { get; set; } //用户ID pk

        [Required]
        [StringLength(50)]
        public required string Account { get; set; } //账号 unique

        [Required]
        [StringLength(64)]
        [Column(TypeName = "CHAR(64)")]
        public required string PasswordHash { get; set; } //密码哈希

        public required int Status { get; set; } //账号状态 1: active, 2: deleted

        public required DateTime CreatedAt { get; set; } //注册时间

        public DateTime? DeletedAt { get; set; } //注销时间 (可为空)

        public DateTime? LastLoginAt { get; set; } //最后登录时间 (可为空)

        [StringLength(20)]
        public string? LastLoginIp { get; set; } //最后登录IP (可为空)

        //导航属性
        public required virtual UserProfile UserProfile { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>(); 
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
        public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();
    }
}