using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Reconciliation
{
    using JISpeed.Core.Entities.Order; //引用 Order 实体所在的命名空间

    //对账实体
    //对应数据库表: Reconciliation
    [Table("Reconciliation")]
    public class Reconciliation
    {
        [Key]
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string ReconId { get; set; } //对账异常ID pk

        public required DateTime PeriodStart { get; set; } //账期起

        public required DateTime PeriodEnd { get; set; } //账期止

        public required DateTime FoundAt { get; set; } //发现时间

        public required int ReconType { get; set; } //异常类型 (TINYINT)

        [Required]
        [Column(TypeName = "DECIMAL(10, 2)")]
        public required decimal DiffAmount { get; set; } //差额金额

        public required int AffectedOrders { get; set; } //受影响订单数

        public required bool IsResolved { get; set; } //是否解决 (BOOLEAN)

        //导航属性
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>(); //一个对账异常可能涉及多个订单

        public Reconciliation(DateTime periodStart, DateTime periodEnd, DateTime foundAt,
                              int reconType, decimal diffAmount, int affectedOrders, bool isResolved)
        {
            ReconId = Guid.NewGuid().ToString("N");
            PeriodStart = periodStart;
            PeriodEnd = periodEnd;
            FoundAt = foundAt;
            ReconType = reconType;
            DiffAmount = diffAmount;
            AffectedOrders = affectedOrders;
            IsResolved = isResolved;
        }

        private Reconciliation() { }
    }
}