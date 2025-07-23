using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.User
{
    using JISpeed.Core.Entities.Common;
    using JISpeed.Core.Entities.Order;
    
    //用户实体 - 对应数据库表: CustomUser
    [Table("CUSTOMUSER")]
    public class User
    {
        [Key]
        [StringLength(450)]
        [Column(TypeName = "VARCHAR(450)")]
        public required string UserId { get; set; }
        
        [StringLength(50)]
        public required string Nickname { get; set; }
        
        [StringLength(200)]
        public string AvatarUrl { get; set; } = "default.jpg";
        
        public int Gender { get; set; } = 3; // 1: male, 2: female, 3: other
        
        public DateTime? Birthday { get; set; }
        
        public int Level { get; set; } = 0;
        
        // 默认地址ID
        [StringLength(32)]
        public string? DefaultAddrId { get; set; }
        
        //身份验证用户关联
        [StringLength(450)]
        public required string ApplicationUserId { get; set; }
        
        //导航属性
        [ForeignKey("DefaultAddrId")]
        public virtual Address? DefaultAddress { get; set; }
        
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser? ApplicationUser { get; set; }
        
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
        public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        //主构造函数
        public User(string userId, string nickname, string applicationUserId)
        {
            UserId = userId;
            Nickname = nickname;
            ApplicationUserId = applicationUserId;
        }
        
        //用于 EF Core 的无参构造函数
        public User() { }
    }
}
