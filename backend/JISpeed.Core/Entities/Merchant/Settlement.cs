using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Merchant
{
    //结算实体
    //对应数据库表: Settlement
    [Table("Settlement")]
    public class Settlement
    {
        [Key]
        [Column(TypeName = "CHAR(32)")]
        public required string SettleId { get; set; } //结算单ID pk

        public required DateTime PeriodStart { get; set; } //周期起

        public required DateTime PeriodEnd { get; set; } //周期止

        [Required]
        [Column(TypeName = "DECIMAL(10, 2)")]
        public required decimal GrossAmount { get; set; } //毛收入

        [Required]
        [Column(TypeName = "DECIMAL(10, 2)")]
        public required decimal CommissionFee { get; set; } //抽佣

        [Required]
        [Column(TypeName = "DECIMAL(10, 2)")]
        public required decimal NetAmount { get; set; } //应结金额

        public DateTime? SettledAt { get; set; } //结算完成时间 (可为空)

        [Column(TypeName = "CHAR(32)")]
        public required string MerchantId { get; set; } //商家ID fk->Merchant(merchantId)

        //导航属性
        [ForeignKey("MerchantId")]
        public virtual required Merchant Merchant { get; set; }
    }
}