namespace JISpeed.Api.DTOs
{
     public class OrderDetailDto
     { 
         public required string OrderId { get; set; } 
         public required string UserId { get; set; } 
         public required string AddressId { get; set; } 
         public required decimal OrderAmount { get; set; }
         public required DateTime CreateAt { get; set; }
         public required int OrderStatus { get; set; }
         public required string? ReconId { get; set; } 
         public required string? CouponId { get; set; } 
         public required string? AssignId { get; set; } 
         public required Dictionary<string, MerchantWithDishesDto> MerchantDishes { get; set; }

     }
     
     public class OrderDto
     { 
         public required string OrderId { get; set; } 
         public required decimal OrderAmount { get; set; }
         public required DateTime CreateAt { get; set; }
         public required int OrderStatus { get; set; }
         public required string? ReconId { get; set; } 
         public required string? CouponId { get; set; } 
         public required string? AssignId { get; set; } 
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
}