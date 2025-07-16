using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore; 

namespace JISpeed.Core.Entities.User
{
    using JISpeed.Core.Entities.Merchant; //引用 Merchant 实体所在的命名空间
    using JISpeed.Core.Entities.Dish; //引用 Dish 实体所在的命名空间

    //购物车项实体
    //对应数据库表: CartItem
    [Table("CartItem")]
    [PrimaryKey(nameof(CartItemId), nameof(UserId))] //复合主键
    public class CartItem
    {
        [Key] //CartItemId 仍然是主要的PK标识
        [Column(TypeName = "CHAR(32)")]
        public required string CartItemId { get; set; } //购物车ID pk

        [Column(TypeName = "CHAR(32)")]
        public required string UserId { get; set; } //用户ID pk,fk->User(userId)

        [Column(TypeName = "CHAR(32)")]
        public required string MerchantId { get; set; } //商家ID fk->Merchant()

        [Column(TypeName = "CHAR(32)")]
        public required string DishId { get; set; } //菜品ID fk->Dish(dishId)

        public required DateTime AddedAt { get; set; } //加入时间

        //导航属性
        [ForeignKey("UserId")]
        public virtual required User User { get; set; }

        [ForeignKey("MerchantId")]
        public virtual required Merchant Merchant { get; set; }

        [ForeignKey("DishId")]
        public virtual required Dish Dish { get; set; }
    }
}