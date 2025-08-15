using System;

namespace JISpeed.Api.DTOs
{
    // 商家信息
    public class MerchantDto
    {

        // 商家ID
        public required string MerchantId { get; set; } 
        // 商家昵称
        public required string MerchantName { get; set; }
        // 商家状态
        public required int Status { get; set; } 
        // 联系方式
        public required string ContactInfo { get; set; }
        // 地址
        public string? Location { get; set; }
        public string? Description { get; set; }
        
    }
    public class UpdateMerchantDto
    {
        
        // 商家昵称
        public string? MerchantName { get; set; }
        // 商家状态
        public int? Status { get; set; } 
        // 联系方式
        public string? ContactInfo { get; set; }
        // 地址
        public string? Location { get; set; }
        
        public string? Description { get; set; }
        
    }
    
    // 商家销量排行查询
    public class SalesStatDto
    {
        // 商家ID
        public required string MerchantId { get; set; } 
        // 统计日期
        public required DateTime StatDate { get; set; }
        // 销售量
        public int SalesQty { get; set; } 
        // 销售额
        public required decimal SalesAmount { get; set; }
    }

    // 菜品浏览功能
    public class DishesDto
    {
        // 菜品ID
        public string? DishId{ get; set; } 
        // 分类ID
        public required string CategoryId{ get; set; } 
        // 菜品名
        public required string DishName{ get; set; } 
        // 现价
        public required decimal Price{ get; set; } 
        // 原价
        public required decimal OriginPrice{ get; set; } 
        // 封面
        public string? CoverUrl { get; set; } 
        // 月销售
        public int? MonthlySales{ get; set; } 
        // 好评率
        public decimal? Rating{ get; set; } 
        // 上架标志
        public int? OnSale{ get; set; } 
        // 商家ID
        public string? MerchantId{ get; set; }
        // 评论数目
        public int? ReviewQuantity{ get; set; } 
        public string? CategoryName { get; set; }
        public string? Description { get; set; } //菜品描述
        public int? StockQuantity { get; set; }
        
    }
    
    public class UpdateDishesDto
    {
        // 分类ID
        public string? CategoryId{ get; set; } 
        // 菜品名
        public string? DishName{ get; set; } 
        // 现价
        public decimal? Price{ get; set; } 
        // 原价
        public decimal? OriginPrice{ get; set; } 
        // 封面
        public string? CoverUrl { get; set; }
        public string? Description { get; set; } //菜品描述
        public int? StockQuantity { get; set; }
        // 上架标志
        public int? OnSale{ get; set; } 
    }
    
    public class CreateDishesDto
    {
        // 分类ID
        public required string CategoryId{ get; set; } 
        // 菜品名
        public required string DishName{ get; set; } 
        // 现价
        public decimal? Price{ get; set; } 
        // 原价
        public required decimal OriginPrice{ get; set; } 
        // 封面
        public string? CoverUrl { get; set; } 
        
        public string? Description { get; set; } //菜品描述
        public int? StockQuantity { get; set; }
        
    }
    
    // 用于订单的内嵌DishDto
    public class DishItemDto
    {
        public required string DishId { get; set; }
        public required string DishName { get; set; }
        public required int Quantity { get; set; } // 订单中的菜品数量（来自 OrderDish）
        public required decimal UnitPrice { get; set; } // 下单时的单价（来自 OrderDish）
        public required decimal Price{ get; set; } 
        public required string CoverUrl { get; set; } 
    }

    public class CategoryWithDishesDto
    {
        // 分类ID
        public required string CategoryId { get; set; }
        // 分类名称
        public required string CategoryName { get; set; }
        // 该分类下的菜品列表（内层DTO）
        public List<DishesDto> Dishes { get; set; } = new(); 
    }
    
    public class MerchantWithDishesDto
    {
        // 商家ID
        public required string MerchantId { get; set; }
        // 商家名称
        public required string MerchantName { get; set; }
        // 该分类下的菜品列表（内层DTO）
        public List<DishItemDto> Dishes { get; set; } = new(); 
    }
    public class CategoryDto
    {
        // 分类ID
        public required string CategoryId { get; set; }
        // 分类名称
        public required string CategoryName { get; set; }
        // 父级分类ID (可为空，表示顶级分类)
        public string? ParentId { get; set; }
        //排序顺序
        public required int SortOrder { get; set; }
    }

    public class SettlementDetailDto
    {
        public required string SettleId { get; set; } //结算单ID pk

        public required DateTime PeriodStart { get; set; } //周期起

        public required DateTime PeriodEnd { get; set; } //周期止
        
        public required decimal GrossAmount { get; set; } //毛收入
        
        public required decimal CommissionFee { get; set; } //抽佣
        
        public required decimal NetAmount { get; set; } //应结金额

        public DateTime? SettledAt { get; set; } //结算完成时间 (可为空)
    }
    public class SettlementDto
    {
        public required string SettleId { get; set; } //结算单ID pk

        public required DateTime PeriodStart { get; set; } //周期起

        public required DateTime PeriodEnd { get; set; } //周期止

        public DateTime? SettledAt { get; set; } //结算完成时间 (可为空)
    }
}
