using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Core.Entities.Merchant
{
    //销售统计实体
    //对应数据库表: SalesStat
    [Table("SALESSTAT")]
    [PrimaryKey(nameof(StatDate), nameof(MerchantId))] //复合主键
    public class SalesStat
    {
        public required DateTime StatDate { get; set; } //统计日期 pk

        public required int SalesQty { get; set; } //销量

        [Column(TypeName = "DECIMAL(10, 2)")]
        public required decimal SalesAmount { get; set; } //销售额

        [StringLength(450)]
        [Column(TypeName = "VARCHAR(450)")]
        public required string MerchantId { get; set; } //商家ID pk,fk->Merchant(merchantId)

        //导航属性
        [ForeignKey("MerchantId")]
        public virtual required Merchant Merchant { get; set; }

        public SalesStat(DateTime statDate, string merchantId, int salesQty, decimal salesAmount)
        {
            StatDate = statDate;
            MerchantId = merchantId;
            SalesQty = salesQty;
            SalesAmount = salesAmount;
        }

        private SalesStat() { }
    }
}