using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Common
{
    using JISpeed.Core.Entities.Order; //引用 Order 实体所在的命名空间
    using JISpeed.Core.Entities.User; //引用 User 实体所在的命名空间

    //优惠券实体
    //对应数据库表: Coupon
    [Table("COUPON")]
    public class Coupon
    {
        [Key]
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string CouponId { get; set; } //优惠券ID pk

        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string UserId { get; set; } //用户ID pk, fk->User(userId)

        [Column(TypeName = "DECIMAL(10, 2)")]
        public required decimal FaceValue { get; set; } //面额

        [Column(TypeName = "DECIMAL(10, 2)")]
        public required decimal Threshold { get; set; } //满减门槛

        public required int TotalQty { get; set; } //总量
        public required int IssuedQty { get; set; } //已发量

        public required DateTime StartTime { get; set; } //可用起始

        public required DateTime EndTime { get; set; } //可用截止

        //导航属性
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>(); //一个优惠券可能被多个订单使用

        [ForeignKey("UserId")]
        public virtual required User User { get; set; } //关联到 User 实体

        public Coupon(string userId, decimal faceValue, decimal threshold, int totalQty,
                      DateTime startTime, DateTime endTime, int issuedQty = 0)
        {
            CouponId = Guid.NewGuid().ToString("N"); //生成唯一的优惠券编号
            UserId = userId;
            FaceValue = faceValue;
            Threshold = threshold;
            TotalQty = totalQty;
            IssuedQty = issuedQty;
            StartTime = startTime;
            EndTime = endTime;
        }

        private Coupon() { }
    }
}