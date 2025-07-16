using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Order
{
    using JISpeed.Core.Entities.User; //引用 User 实体所在的命名空间

    //退款实体
    //对应数据库表: Refund
    [Table("Refund")]
    public class Refund
    {
        [Key]
        [Column(TypeName = "CHAR(32)")]
        public required string RefundId { get; set; } //退款ID pk

        [Column(TypeName = "CHAR(32)")]
        public required string OrderId { get; set; } //订单ID fk->Order(orderId)

        [Column(TypeName = "CHAR(32)")]
        public required string ApplicationId { get; set; } //申请人ID fk->User(userId)

        [StringLength(65535)] //TEXT类型
        public string? Reason { get; set; } //退款原因

        public required int RefundAmount { get; set; } //退款金额

        public required DateTime ApplyAt { get; set; } //申请时间

        public required int AudisStatus { get; set; } //审核状态

        public DateTime? FinishAt { get; set; } //退款完成时间 (可为空)

        //导航属性
        [ForeignKey("OrderId")]
        public virtual required Order Order { get; set; }

        [ForeignKey("ApplicationId")]
        public virtual required User Applicant { get; set; } //关联到 User 实体
    }
}