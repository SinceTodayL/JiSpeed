using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Merchant
{
    using JISpeed.Core.Entities.Dish; //引用 Dish 实体所在的命名空间
    using JISpeed.Core.Entities.User; //引用 CartItem 实体所在的命名空间

    //商家实体
    //对应数据库表: Merchant
    [Table("Merchant")]
    public class Merchant
    {
        [Key]
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string MerchantId { get; set; } //商家ID pk

        [StringLength(20)]
        public required string MerchantName { get; set; } //商家名

        public required int Status { get; set; } //商家状态

        [StringLength(20)]
        public string? ContactInfo { get; set; } //联系方式

        [StringLength(20)]
        public string? Location { get; set; } //地址

        //导航属性
        public virtual ICollection<Application> Applications { get; set; } = new List<Application>(); //商家可以有多个申请
        public virtual ICollection<Dish> Dishes { get; set; } = new List<Dish>();
        public virtual ICollection<SalesStat> SalesStats { get; set; } = new List<SalesStat>();
        public virtual ICollection<Settlement> Settlements { get; set; } = new List<Settlement>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>(); //Merchant 可以出现在多个购物车项中

        public Merchant(string merchantName, int status = 1, string? contactInfo = null, string? location = null)
        {
            MerchantId = Guid.NewGuid().ToString("N"); //生成唯一的MerchantId
            MerchantName = merchantName;
            Status = status;
            ContactInfo = contactInfo;
            Location = location;
        }

        private Merchant() { }
    }
}