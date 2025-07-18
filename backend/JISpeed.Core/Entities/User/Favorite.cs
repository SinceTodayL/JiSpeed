using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore; 

namespace JISpeed.Core.Entities.User
{
    using System.Reflection.Metadata;
    using JISpeed.Core.Entities.Dish; // 引用 Dish 实体所在的命名空间

    //收藏实体
    //对应数据库表: Favorite
    [Table("FAVORITE")]
    [PrimaryKey(nameof(UserId), nameof(DishId))]  //复合主键
    public class Favorite
    {
        [StringLength(450)]
        [Column(TypeName = "VARCHAR(450)")]
        public required string UserId { get; set; } //用户ID pk, fk->User(userId)

        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string DishId { get; set; } //菜品ID pk, fk->Dish(dishId)

        public required DateTime FavorAt { get; set; } //收藏时间

        //导航属性
        [ForeignKey("UserId")]
        public virtual required User User { get; set; }

        [ForeignKey("DishId")]
        public virtual required Dish Dish { get; set; }

        public Favorite(string userId, string dishId)
        {
            UserId = userId;
            DishId = dishId;
            FavorAt = DateTime.UtcNow; //使用 UTC 时间
        }

        private Favorite() { } 
    }
}