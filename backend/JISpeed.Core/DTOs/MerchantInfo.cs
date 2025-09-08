namespace JISpeed.Core.DTOs
{
    public class MerchantDetailDto
    {
        // 商家ID
        public required string MerchantId { get; set; }
        public required decimal Distance { get; set; }
        public required decimal Rating { get; set; }
        public required int SalesQty { get; set; }
    }
}