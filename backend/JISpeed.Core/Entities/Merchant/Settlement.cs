using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Merchant
{
    //结算实体
    //对应数据库表: Settlement
    [Table("SETTLEMENT")]
    public class Settlement
    {
        [Key]
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string SettleId { get; set; } //结算单ID pk

        public required DateTime PeriodStart { get; set; } //周期起

        public required DateTime PeriodEnd { get; set; } //周期止

        [Column(TypeName = "DECIMAL(10, 2)")]
        public required decimal GrossAmount { get; set; } //毛收入

        [Column(TypeName = "DECIMAL(10, 2)")]
        public required decimal CommissionFee { get; set; } //抽佣

        [Column(TypeName = "DECIMAL(10, 2)")]
        public required decimal NetAmount { get; set; } //应结金额

        public DateTime? SettledAt { get; set; } //结算完成时间 (可为空)

        [StringLength(450)]
        [Column(TypeName = "VARCHAR(450)")]
        public required string MerchantId { get; set; } //商家ID fk->Merchant(merchantId)

        //导航属性
        [ForeignKey("MerchantId")]
        public virtual required Merchant Merchant { get; set; }

        public Settlement(DateTime periodStart, DateTime periodEnd, decimal grossAmount, decimal commissionFee, decimal netAmount, string merchantId, DateTime? settledAt = null)
        {
            PeriodStart = periodStart;
            PeriodEnd = periodEnd;
            GrossAmount = grossAmount;
            CommissionFee = commissionFee;
            NetAmount = netAmount;
            MerchantId = merchantId;
            SettleId = Guid.NewGuid().ToString("N"); //生成唯一的SettleId
            SettledAt = settledAt;
        }

        private Settlement() { }
    }
}