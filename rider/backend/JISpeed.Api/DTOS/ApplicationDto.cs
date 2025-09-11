namespace JISpeed.Api.DTOs
{
    // 管理员审批请求
    public class AuditRequest
    {
        public required int AuditStatus { get; set; }
        public string? RejectReason { get; set; }
    }
    // 商家发起请求
    public class ApplicationRequest
    {
        public required string CompanyName { get; set; } //店名称
        public required string ApplicationMaterials{ get; set; }
    }
    
    public class ApplicationResponse
    {
        public required string ApplyId { get; set; }
        public required string CompanyName { get; set; }
        public required string SubmittedAt { get; set; }
        public required string MerchantId { get; set; }
        public required string AuditStatus { get; set; }
        public string? AuditAt { get; set; }
        public string? RejectReason { get; set; }
        public string? AdminId { get; set; } 
        public string? ApplicationMaterials{ get; set; }
        
    }
}