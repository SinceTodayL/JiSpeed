using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore; 

namespace JISpeed.Core.Entities.Dish
{
    using JISpeed.Core.Entities.Merchant; //引用 Merchant 实体所在的命名空间
    using JISpeed.Core.Entities.Junctions; //引用联结表命名空间

    //菜品实体
    //对应数据库表: Dish
    [Table("Dish")]
    public class Dish
    {
        [Key]
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string DishId { get; set; } //菜品ID pk

        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string CategoryId { get; set; } //分类ID fk->Category(categoryId)

        [StringLength(20)]
        public required string DishName { get; set; } //菜品名

        [Column(TypeName = "DECIMAL(10, 2)")]
        public required decimal Price { get; set; } //售价

        [Column(TypeName = "DECIMAL(10, 2)")]
        public required decimal OriginPrice { get; set; } //原价

        [StringLength(200)]
        public string? CoverUrl { get; set; } //封面图URL

        public required int MonthlySales { get; set; } //月销量

        [Column(TypeName = "DECIMAL(5, 2)")]
        public required decimal Rating { get; set; } //好评率

        public required int OnSale { get; set; } //上架标志

        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string MerchantId { get; set; } //商家ID fk->Merchant(merchantId)

        public required int ReviewQuantity { get; set; } //评论数目

        //导航属性
        [ForeignKey("CategoryId")]
        public virtual required Category Category { get; set; }

        [ForeignKey("MerchantId")]
        public virtual required Merchant Merchant { get; set; }

        // 多对多联结表
        public virtual ICollection<DishReview> DishReviews { get; set; } = new List<DishReview>();
        public virtual ICollection<OrderDish> OrderDishes { get; set; } = new List<OrderDish>();

        public Dish(string categoryId, string dishName, decimal price, decimal originPrice, string? coverUrl = null,
                    int monthlySales = 0, decimal rating = 0.0m, int onSale = 1, string merchantId = "", int reviewQuantity = 0)
        {
            DishId = Guid.NewGuid().ToString("N"); //生成唯一的DishId
            CategoryId = categoryId;
            DishName = dishName;
            Price = price;
            OriginPrice = originPrice;
            CoverUrl = coverUrl;
            MonthlySales = monthlySales;
            Rating = rating;
            OnSale = onSale;
            MerchantId = merchantId;
            ReviewQuantity = reviewQuantity;
        }
        
        private Dish() { } // EF Core 需要一个无参构造函数
    }
}