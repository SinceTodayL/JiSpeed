using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Order
{
    //支付实体
    //对应数据库表: Payment
    [Table("Payment")]
    public class Payment
    {
        [Key]
        [Column(TypeName = "CHAR(32)")]
        public required string PayId { get; set; } //支付ID pk

        [Column(TypeName = "CHAR(32)")]
        public required string OrderId { get; set; } //订单ID fk->Order(orderId)

        [Required]
        [StringLength(20)]
        public required string Channel { get; set; } //支付渠道

        public required int PayAmount { get; set; } //应付金额

        public required int PayStatus { get; set; } //支付状态

        public DateTime? PayTime { get; set; } //支付完成时间 (可为空)

        //导航属性
        [ForeignKey("OrderId")]
        public virtual required Order Order { get; set; }
    }
}