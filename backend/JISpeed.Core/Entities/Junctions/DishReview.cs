using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore; 

namespace JISpeed.Core.Entities.Junctions 
{
    using JISpeed.Core.Entities.Dish; //引用 Dish 实体所在的命名空间
    using JISpeed.Core.Entities.Order; //引用 Review 实体所在的命名空间

    //菜品评论联结实体 (多对多)
    //对应数据库表: dish_review
    [Table("dish_review")]
    [PrimaryKey(nameof(DishId), nameof(ReviewId))] //复合主键
    public class DishReview
    {
        [Column(TypeName = "CHAR(32)")]
        public required string DishId { get; set; } //菜品ID pk,fk->Dish(dishId)

        [Column(TypeName = "CHAR(32)")]
        public required string ReviewId { get; set; } //评论ID pk,fk->Review(reviewId)

        //导航属性
        [ForeignKey("DishId")]
        public virtual required Dish Dish { get; set; }

        [ForeignKey("ReviewId")]
        public virtual required Review Review { get; set; }
    }
}