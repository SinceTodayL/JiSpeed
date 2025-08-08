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
}