using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Merchant
{
    using JISpeed.Core.Entities.Common;
    using JISpeed.Core.Entities.Dish;
    using JISpeed.Core.Entities.Order;

    //商家实体 - 对应数据库表: Merchant
    [Table("MERCHANT")]
    public class Merchant
    {
        [Key]
        [StringLength(450)]
        [Column(TypeName = "VARCHAR(450)")]
        public required string MerchantId { get; set; }
        
        [StringLength(50)]
        public required string MerchantName { get; set; }
        
        public int Status { get; set; } = 0;
        
        [StringLength(50)]
        public string? ContactInfo { get; set; }
        
        [StringLength(65535)]
        public string? Description { get; set; } // 商家描述

        
        [StringLength(200)]
        public string? Location { get; set; }
        
        [Column(TypeName = "DECIMAL(10, 6)")]
        public decimal? Longitude { get; set; } // 经度

        [Column(TypeName = "DECIMAL(10, 6)")]
        public decimal? Latitude { get; set; } // 纬度

        // 身份验证用户关联
        [StringLength(450)]
        [Column(TypeName = "VARCHAR(450)")]
        public string? ApplicationUserId { get; set; }
        
        // 导航属性
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser? ApplicationUser { get; set; }

        public virtual ICollection<SalesStat> SalesStats { get; set; } = new List<SalesStat>();
        public virtual ICollection<Settlement> Settlements { get; set; } = new List<Settlement>();
        public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
        public virtual ICollection<Dish> Dishes { get; set; } = new List<Dish>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        // 主构造函数
        public Merchant(string merchantId, string merchantName, string? applicationUserId = null)
        {
            MerchantId = merchantId;
            MerchantName = merchantName;
            ApplicationUserId = applicationUserId;
            Longitude = 116.397128m;
            Latitude = 39.916527m;
        }
        
        // 用于 EF Core 的私有构造函数
        public Merchant() { }
    }
}
