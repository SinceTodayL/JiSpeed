using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Order
{
    using JISpeed.Core.Entities.User; //引用 User 和 Address 实体所在的命名空间
    using JISpeed.Core.Entities.Reconciliation; //引用 Reconciliation 实体所在的命名空间
    using JISpeed.Core.Entities.Common; //引用 Coupon 实体所在的命名空间
    using JISpeed.Core.Entities.Rider; //引用 Assignment 实体所在的命名空间
    using JISpeed.Core.Entities.Junctions; //引用 OrderDish 联结表所在的命名空间

    //订单实体
    //对应数据库表: Orders
    [Table("ORDERS")]
    public class Order
    {
        [Key]
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string OrderId { get; set; } //订单ID pk

        [StringLength(450)]
        [Column(TypeName = "VARCHAR(450)")]
        public required string UserId { get; set; } //用户ID fk->User(userId)

        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string AddressId { get; set; } //地址ID fk->Address(addrId)

        [Column(TypeName = "DECIMAL(10, 2)")]
        public required decimal OrderAmount { get; set; } //订单金额

        public required DateTime CreateAt { get; set; } //创建时间

        public required int OrderStatus { get; set; } //订单状态 (TINYINT)

        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public string? ReconId { get; set; } //对账异常ID fk->Reconciliation(reconId) (可为空)
    
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public string? CouponId { get; set; } //优惠券ID fk->Coupon(couponId) (可为空)

        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public string? AssignId { get; set; } //分配编号 fk->Assignment(assignID) (可为空)

        //导航属性
        [ForeignKey("UserId")]
        public virtual required User User { get; set; }

        [ForeignKey("AddressId")]
        public virtual required Address Address { get; set; }

        [ForeignKey("ReconId")]
        public virtual Reconciliation? Reconciliation { get; set; } //可为空

        [ForeignKey("CouponId")]
        public virtual Coupon? Coupon { get; set; } //可为空

        [ForeignKey("AssignId")]
        public virtual Assignment? Assignment { get; set; } //关联到 Rider 命名空间下的 Assignment，可为空

        // 一对多集合导航属性
        public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public required virtual ICollection<OrderLog> OrderLogs { get; set; }
        public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>(); //订单可以有多个投诉
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<OrderDish> OrderDishes { get; set; } = new List<OrderDish>(); //多对多联结表
    }
}