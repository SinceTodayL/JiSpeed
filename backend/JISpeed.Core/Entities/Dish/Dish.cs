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
        [Column(TypeName = "CHAR(32)")]
        public required string DishId { get; set; } //菜品ID pk

        [Column(TypeName = "CHAR(32)")]
        public required string CategoryId { get; set; } //分类ID fk->Category(categoryId)

        [Required]
        [StringLength(20)]
        public required string DishName { get; set; } //菜品名

        [Required]
        [Column(TypeName = "DECIMAL(10, 2)")]
        public required decimal Price { get; set; } //售价

        [Required]
        [Column(TypeName = "DECIMAL(10, 2)")]
        public required decimal OriginPrice { get; set; } //原价

        [StringLength(200)]
        public string? CoverUrl { get; set; } //封面图

        public required int MonthlySales { get; set; } //月销量

        [Required]
        [Column(TypeName = "DECIMAL(5, 2)")]
        public required decimal Rating { get; set; } //好评率

        public required int OnSale { get; set; } //上架标志

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
    }
}