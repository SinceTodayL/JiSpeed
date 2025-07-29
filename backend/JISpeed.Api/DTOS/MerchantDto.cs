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
        public int Status { get; set; } 
        // 联系方式
        public required string ContactInfo { get; set; }
        // 地址
        public string Location { get; set; } =string.Empty;
        
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
    }


}