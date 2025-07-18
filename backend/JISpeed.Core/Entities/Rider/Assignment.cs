using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Rider
{
    using JISpeed.Core.Entities.Order; //引用 Order 实体所在的命名空间

    //分配实体 (派单)
    //对应数据库表: assignment
    [Table("ASSIGNMENT")]
    public class Assignment
    {
        [Key]
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string AssignId { get; set; } //分配编号 PK

        [StringLength(32)]
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

        public Assignment(string riderId, DateTime assignedAt, int acceptedStatus,
                          DateTime? acceptedAt = null, int? timeOut = null)
        {
            AssignId = Guid.NewGuid().ToString("N"); //生成唯一的分配编号
            RiderId = riderId;
            AssignedAt = assignedAt;
            AcceptedStatus = acceptedStatus;
            AcceptedAt = acceptedAt; //默认未接单
            TimeOut = timeOut; //默认无超时
        }
    }
}