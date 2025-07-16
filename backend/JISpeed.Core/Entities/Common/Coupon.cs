using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Common
{
    using JISpeed.Core.Entities.Order; //引用 Order 实体所在的命名空间

    //优惠券实体
    //对应数据库表: Coupon
    [Table("Coupon")]
    public class Coupon
    {
        [Key]
        [Column(TypeName = "CHAR(32)")]
        public required string CouponId { get; set; } //优惠券ID pk

        [Required]
        [Column(TypeName = "DECIMAL(10, 2)")]
        public required decimal FaceValue { get; set; } //面额

        [Required]
        [Column(TypeName = "DECIMAL(10, 2)")]
        public required decimal Threshold { get; set; } //满减门槛

        public required int TotalQty { get; set; } //总量ze
        public required int IssuedQty { get; set; } //已发量

        public required DateTime StartTime { get; set; } //可用起始

        public required DateTime EndTime { get; set; } //可用截止

        //导航属性
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>(); //一个优惠券可能被多个订单使用
    }
}