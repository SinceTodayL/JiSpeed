using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Rider
{
    using JISpeed.Core.Entities.Order; //引用 Order 实体所在的命名空间

    //分配实体 (派单)
    //对应数据库表: assignment
    [Table("assignment")]
    public class Assignment
    {
        [Key]
        [Column(TypeName = "CHAR(32)")]
        public required string AssignId { get; set; } //分配编号 PK

        [Column(TypeName = "CHAR(32)")]
        public required string RiderId { get; set; } //骑手编号 fk->rider(riderId)

        public required DateTime AssignedAt { get; set; } //派单时间

        public required int AcceptedStatus { get; set; } //接单状态

        public DateTime? AcceptedAt { get; set; } //接单时间 (可为空)

        public int? TimeOut { get; set; } //超时分钟数 (可为空)

        //导航属性
        [ForeignKey("RiderId")]
        public virtual required Rider Rider { get; set; }

        // Assignment 与 Order 是一对一关系，Order 的 AssignId 是外键
        public required virtual Order Order { get; set; }// 反向导航属性
    }
}