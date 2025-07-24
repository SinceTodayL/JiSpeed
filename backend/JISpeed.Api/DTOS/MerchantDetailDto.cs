using System;

namespace JISpeed.Api.DTOs
{

    // 用户详细信息DTO - 用于返回给前端的用户信息

    public class MerchantDetailDto
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
    
    public class SalesStatDto
    {

        // 商家ID
        public required string MerchantId { get; set; } 


        // 商家昵称
        public required DateTime StatDate { get; set; }


        // 商家状态
        public int SalesQty { get; set; } 
        
        // 联系方式
        public required decimal SalesAmount { get; set; }
        
    }
}