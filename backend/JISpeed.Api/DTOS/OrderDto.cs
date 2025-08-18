using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Entities.Junctions;
using JISpeed.Core.Entities.Merchant;
using JISpeed.Core.Interfaces.IServices;

namespace JISpeed.Api.DTOs
{
     public class OrderDetailDto
     { 
         public required string OrderId { get; set; } 
         public required string UserId { get; set; } 
         public required string MerchantId { get; set; }
         public required string AddressId { get; set; } 
         public required decimal OrderAmount { get; set; }
         public required DateTime CreateAt { get; set; }
         public required int OrderStatus { get; set; }
         public required string ReconId { get; set; } 
         public required string CouponId { get; set; } 
         public required string AssignId { get; set; } 
         
         public required List<OrderDishDto> OrderDishes { get; set; }= new List<OrderDishDto>();
         public required List<string> OrderLogIds { get; set; } = new List<string>();
         public required List<string> PaymentIds { get; set; } = new List<string>();
         public required List<string> RefundIds { get; set; } = new List<string>();
         public required List<string> ComplaintIds { get; set; } = new List<string>();
         public required List<string> ReviewIds { get; set; } = new List<string>();


         

     }

     public class OrderDishDto
     {
         public required int Quantity { get; set; } 
         public required string DishId { get; set; }
     }
     
     
     public class OrderDto
     { 
         public required string OrderId { get; set; } 
         public required string MerchantId { get; set; } 

         public required decimal OrderAmount { get; set; }
         public required DateTime CreateAt { get; set; }
         public required int OrderStatus { get; set; }
         public required string ReconId { get; set; } 
         public required string CouponId { get; set; } 
         public required string AssignId { get; set; } 
     }
     public class OrderRequestDto
     { 
         public required decimal OrderAmount { get; set; }
         public string? CouponId { get; set; } 
         public required string AddressId { get; set; } 
         public required string MerchantId { get; set; } 
         public required List<DishQuantityDto> DishQuantities { get; set; }
     }

     public class PaymentDto
     {
         public required string PayId { get; set; }
         public required string OrderId { get; set; }
         public required string Channel { get; set; }
         public required decimal PayAmount { get; set; } 
     }
     
     public class ReconciliationRequestDto
     {
         public required DateTime PeriodStart { get; set; } //账期起
         public required DateTime PeriodEnd { get; set; } //账期止
         public required List<string> OrderIds { get; set; } // 受影响的订单列表
         public required int ReconType { get; set; } //异常类型（1 = 金额不匹配、2 = 订单数量不一致、3 = 重复结算等）
     }
     public class ReconciliationResponseDto
     {
         public required string ReconId { get; set; } //对账异常ID
         public required DateTime PeriodStart { get; set; } //账期起
         public required DateTime PeriodEnd { get; set; } //账期止
         public required int ReconType { get; set; } //异常类型（1 = 金额不匹配、2 = 订单数量不一致、3 = 重复结算等）
         public required decimal DiffAmount { get; set; } //差额金额
         public required int AffectedOrders { get; set; } //受影响订单数
         public required bool IsResolved { get; set; } //是否解决 (BOOLEAN)
         public required List<string> OrderIdList { get; set; } // 受影响的订单列表
     }

     public class CouponResponseDto
     {
         public required string CouponId { get; set; } //优惠券ID pk
         public required string UserId { get; set; } //用户ID pk, fk->User(userId)
         public required decimal FaceValue { get; set; } //面额
         public required decimal Threshold { get; set; } //满减门槛
         public required DateTime StartTime { get; set; } //可用起始
         public required DateTime EndTime { get; set; } //可用截止
     }

     public class CouponRequestDto
     {        
         public string? UserId { get; set; } //用户ID pk, fk->User(userId)
         public decimal ?FaceValue { get; set; } //面额
         public decimal ?Threshold { get; set; } //满减门槛
         public required DateTime StartTime { get; set; } //可用起始
         public required DateTime EndTime { get; set; } //可用截止
     }
     public class OrderLogResponseDto
     {
         public required string LogId { get; set; } //记录ID pk
         public required int StatusCode { get; set; } //状态码
         public required DateTime LoggedAt { get; set; } //时间戳
         public required string Actor { get; set; } //触发方
         public string? Remark { get; set; } //备注
         public required string OrderId { get; set; } //订单ID pk,fk->Order(orderId)
     }

     public class PaymentResponseDto
     {
         public required string PayId { get; set; } //支付ID pk
         public required string OrderId { get; set; } //订单ID fk->Order(orderId)
         public required string Channel { get; set; } //支付渠道
         public required decimal PayAmount { get; set; } //应付金额
         public required int PayStatus { get; set; } //支付状态
         public DateTime? PayTime { get; set; } //支付完成时间 (可为空)
     }
     
     public class RefundRequestDto
     {
         public required string Reason { get; set; } //退款原因
         public required decimal RefundAmount { get; set; } //退款金额
     }

     public class RefundUpdateDto
     {
         public required int RefundStatus{ get; set; }
     }
}