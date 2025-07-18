using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Order
{
    using JISpeed.Core.Entities.User; //引用 User 实体所在的命名空间

    //退款实体
    //对应数据库表: Refund
    [Table("REFUND")]
    public class Refund
    {
        [Key]
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string RefundId { get; set; } //退款ID pk

        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string OrderId { get; set; } //订单ID fk->Order(orderId)

        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string ApplicationId { get; set; } //申请人ID fk->User(userId)

        [StringLength(65535)] //TEXT类型
        public string? Reason { get; set; } //退款原因

        [Column(TypeName = "DECIMAL(10, 2)")]
        public required decimal RefundAmount { get; set; } //退款金额

        public required DateTime ApplyAt { get; set; } //申请时间

        public required int AuditStatus { get; set; } //审核状态

        public DateTime? FinishAt { get; set; } //退款完成时间 (可为空)

        //导航属性
        [ForeignKey("OrderId")]
        public virtual required Order Order { get; set; }

        [ForeignKey("ApplicationId")]
        public virtual required User Applicant { get; set; } //关联到 User 实体
        public Refund(string orderId, string applicationId, decimal refundAmount, string? reason = null)
        {
            RefundId = Guid.NewGuid().ToString("N");
            OrderId = orderId;
            ApplicationId = applicationId;
            RefundAmount = refundAmount;
            Reason = reason;
            ApplyAt = DateTime.UtcNow; //默认申请时间为当前时间
            AuditStatus = 0; //默认审核状态为未审核
            FinishAt = null; //默认退款完成时间为空
        }

        private Refund() { }
    }
}