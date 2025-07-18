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
    [Table("CARTITEM")]
    [PrimaryKey(nameof(CartItemId), nameof(UserId))] //复合主键
    public class CartItem
    {
        [Key] //CartItemId 仍然是主要的PK标识
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string CartItemId { get; set; } //购物车ID pk

        [StringLength(450)]
        [Column(TypeName = "VARCHAR(450)")]
        public required string UserId { get; set; } //用户ID pk,fk->User(userId)

        [StringLength(450)]
        [Column(TypeName = "VARCHAR(450)")]
        public required string MerchantId { get; set; } //商家ID fk->Merchant()

        [StringLength(32)]
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

        public CartItem(string userId, string merchantId, string dishId)
        {
            UserId = userId;
            MerchantId = merchantId;
            DishId = dishId;
            CartItemId = Guid.NewGuid().ToString("N"); //生成唯一的CartItemId
            AddedAt = DateTime.UtcNow; //使用 UTC 时间
        }

        private CartItem(){ }
    }
}