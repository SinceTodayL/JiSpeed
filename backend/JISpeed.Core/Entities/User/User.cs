using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Core.Entities.User
{
    using JISpeed.Core.Entities.Order; //引用 Order 命名空间下的 Complaint 和 Review
    using JISpeed.Core.Entities.Common; //引用 Common 命名空间下的 Coupon

    //用户实体
    //对应数据库表: User
    [Table("User")]
    public class User
    {
        [Key]
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string UserId { get; set; } //用户ID pk

        //导航属性
        public required virtual UserProfile UserProfile { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
        public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();
        public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>(); //用户拥有的优惠券

        public User(string userId)
        {
            UserId = userId;
        }

        private User() { }
    }
}