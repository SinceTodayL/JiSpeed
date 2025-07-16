using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Core.Entities.Order
{
    //订单日志实体
    //对应数据库表: OrderLog
    [Table("OrderLog")]
    [PrimaryKey(nameof(LogId), nameof(OrderId))] //复合主键
    public class OrderLog
    {
        [Key] //LogId仍然是主要的PK标识
        [Column(TypeName = "CHAR(32)")]
        public required string LogId { get; set; } //记录ID pk

        public required int StatusCode { get; set; } //状态码

        public required DateTime LoggedAt { get; set; } //时间戳

        [Required]
        [StringLength(20)]
        public required string Actor { get; set; } //触发方

        [StringLength(65535)] //TEXT类型
        public string? Remark { get; set; } //备注

        [Column(TypeName = "CHAR(32)")]
        public required string OrderId { get; set; } //订单ID pk,fk->Order(orderId)

        //导航属性
        [ForeignKey("OrderId")]
        public virtual required Order Order { get; set; }
    }
}