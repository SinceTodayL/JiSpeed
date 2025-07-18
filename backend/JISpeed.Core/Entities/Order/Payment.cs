using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Order
{
    //支付实体
    //对应数据库表: Payment
    [Table("PAYMENT")]
    public class Payment
    {
        [Key]
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string PayId { get; set; } //支付ID pk

        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string OrderId { get; set; } //订单ID fk->Order(orderId)

        [StringLength(20)]
        public required string Channel { get; set; } //支付渠道

        [Column(TypeName = "DECIMAL(10, 2)")]
        public required decimal PayAmount { get; set; } //应付金额

        public required int PayStatus { get; set; } //支付状态

        public DateTime? PayTime { get; set; } //支付完成时间 (可为空)

        //导航属性
        [ForeignKey("OrderId")]
        public virtual required Order Order { get; set; }

        public Payment(string orderId, string channel, decimal payAmount, int payStatus)
        {
            PayId = Guid.NewGuid().ToString("N");
            OrderId = orderId;
            Channel = channel;
            PayAmount = payAmount;
            PayStatus = payStatus;
            PayTime = null; //默认支付时间为空
        }

        private Payment() { }
    }
}