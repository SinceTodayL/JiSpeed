using JISpeed.Api.DTOs;
using JISpeed.Core.Entities.Merchant;
using JISpeed.Core.Interfaces.IRepositories;

namespace JISpeed.Api.Mappers
{

    // 用户信息映射器 - 负责将实体对象转换为DTO对象

    public static class MerchantMapper
    {

        // 将Merchant实体转换为MerchantDetailDto

        // <param name="user">用户实体</param>
        // <param name="stats">用户统计信息</param>
        // <returns>用户详情DTO</returns>
        public static MerchantDetailDto ToMerchantDetailDto(Merchant user)
        {
            return new MerchantDetailDto
            {
                MerchantId = user.MerchantId,
                MerchantName = user.MerchantName,
                Status = user.Status,
                ContactInfo = user.ContactInfo??"",
                Location = user.Location??"",
            };
        }

        public static List<SalesStatDto> ToSalesStatDto(List<SalesStat> dataList)
        {
            return dataList?.Select(data => new SalesStatDto
            {
                MerchantId = data.MerchantId,
                SalesAmount = data.SalesAmount,
                StatDate = data.StatDate,
                SalesQty = data.SalesQty
            }).ToList() ?? new List<SalesStatDto>();;
        }

    }
}
