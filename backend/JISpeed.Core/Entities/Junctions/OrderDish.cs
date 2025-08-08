using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Core.Entities.Junctions 
{
    using JISpeed.Core.Entities.Order; //引用 Order 实体所在的命名空间
    using JISpeed.Core.Entities.Dish; //引用 Dish 实体所在的命名空间

    //订单菜品联结实体 (多对多)
    //对应数据库表: order_dish
    [Table("ORDER_DISH")]
    [PrimaryKey(nameof(OrderId), nameof(DishId))] //复合主键
    public class OrderDish
    {
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string OrderId { get; set; } //订单ID pk,fk->Order(orderId)

        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string DishId { get; set; } //菜品ID pk,fk->Dish(dishId)
        
        public required int Quantity { get; set; } // 订单中该菜品的数量
        //导航属性
        [ForeignKey("OrderId")]
        public virtual required Order Order { get; set; }

        [ForeignKey("DishId")]
        public virtual required Dish Dish { get; set; }

        public OrderDish(string orderId, string dishId,int quantity)
        {
            OrderId = orderId;
            DishId = dishId;
            Quantity = quantity;
        }

        private OrderDish() { }

    }
}