using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore; 

namespace JISpeed.Core.Entities.Rider
{
    //骑手绩效实体
    //对应数据库表: performance
    [Table("PERFORMANCE")]
    [PrimaryKey(nameof(RiderId), nameof(StatsMonth))] //复合主键
    public class Performance
    {
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string RiderId { get; set; } //骑手编号 pk,fk->rider(riderId)

        public required DateTime StatsMonth { get; set; } //统计月份 pk (DATE 类型映射为 DateTime)

        public required int TotalOrders { get; set; } //总订单量

        [Column(TypeName = "DECIMAL(5, 2)")]
        public required decimal OnTimeRate { get; set; } //准时率

        [Column(TypeName = "DECIMAL(5, 2)")]
        public required decimal GoodReviewRate { get; set; } //好评率

        [Column(TypeName = "DECIMAL(5, 2)")]
        public required decimal BadReviewRate { get; set; } //差评率

        [Column(TypeName = "DECIMAL(10, 2)")]
        public required decimal Income { get; set; } //总收入

        //导航属性
        [ForeignKey("RiderId")]
        public virtual required Rider Rider { get; set; }

        public Performance(string riderId, DateTime statsMonth, int totalOrders, decimal onTimeRate,
                           decimal goodReviewRate, decimal badReviewRate, decimal income)
        {
            RiderId = riderId;
            StatsMonth = statsMonth;
            TotalOrders = totalOrders;
            OnTimeRate = onTimeRate;
            GoodReviewRate = goodReviewRate;
            BadReviewRate = badReviewRate;
            Income = income;
        }

        private Performance() { } 
    }
}