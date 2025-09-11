namespace JISpeed.Core.Interfaces.IServices
{
    public interface IPlatformService
    {
        Task<OperationalDataDto> GetOperationalDataByTimeRangeAsync(
            DateTime ?startTime, 
            DateTime ?endTime);
    }
    
    public class OperationalDataDto
    {
        public required int OrderQuantity { get; set; }
        public required int CancelledOrderQuantity { get; set; }
        public required int AftersalesCompletedOrderQuantity { get; set; }

        public required int UserQuantity { get; set; }
        public required int MerchantQuantity { get; set; }
        public required int RiderQuantity { get; set; }
    }
}