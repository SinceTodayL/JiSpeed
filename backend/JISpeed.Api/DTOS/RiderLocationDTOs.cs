using System;
using System.ComponentModel.DataAnnotations;

namespace JISpeed.Api.DTOS
{
    // 骑手位置DTO
    public class RiderLocationDTO
    {
        // 位置ID
        public string? LocationId { get; set; }

        // 骑手ID
        public string? RiderId { get; set; }

        // 经度
        public decimal Longitude { get; set; }

        // 纬度
        public decimal Latitude { get; set; }

        // 定位时间
        public DateTime LocationTime { get; set; }

        // 定位精度(米)
        public decimal? Accuracy { get; set; }

        // 速度(km/h)
        public decimal? Speed { get; set; }

        // 方向(0-360度)
        public decimal? Direction { get; set; }

        // 定位类型(GPS、网络等)
        public string? LocationType { get; set; }

        // 状态(1:在线, 0:离线)
        public int Status { get; set; }
    }

    // 更新骑手位置DTO
    public class UpdateRiderLocationDTO
    {
        // 骑手ID
        [Required(ErrorMessage = "骑手ID不能为空")]
        public string RiderId { get; set; } = string.Empty;

        // 经度
        [Required(ErrorMessage = "经度不能为空")]
        [Range(-180, 180, ErrorMessage = "经度必须在-180到180之间")]
        public decimal Longitude { get; set; }

        // 纬度
        [Required(ErrorMessage = "纬度不能为空")]
        [Range(-90, 90, ErrorMessage = "纬度必须在-90到90之间")]
        public decimal Latitude { get; set; }

        // 定位精度(米)
        [Range(0, 1000, ErrorMessage = "定位精度必须在0到1000之间")]
        public decimal? Accuracy { get; set; }

        // 速度(km/h)
        [Range(0, 200, ErrorMessage = "速度必须在0到200之间")]
        public decimal? Speed { get; set; }

        // 方向(0-360度)
        [Range(0, 360, ErrorMessage = "方向必须在0到360之间")]
        public decimal? Direction { get; set; }

        // 定位类型(GPS、网络等)
        [StringLength(20, ErrorMessage = "定位类型长度不能超过20个字符")]
        public string? LocationType { get; set; }
    }

    // 更新骑手状态DTO
    public class UpdateRiderStatusDTO
    {
        // 状态(1:在线, 0:离线)
        [Required(ErrorMessage = "状态不能为空")]
        [Range(0, 1, ErrorMessage = "状态必须为0(离线)或1(在线)")]
        public int Status { get; set; }
    }

    // 距离DTO
    public class DistanceDTO
    {
        // 骑手ID
        public string? RiderId { get; set; }

        // 目标经度
        public decimal TargetLongitude { get; set; }

        // 目标纬度
        public decimal TargetLatitude { get; set; }

        // 距离(米)
        public double Distance { get; set; }
    }

    // 地址DTO
    public class AddressDTO
    {
        // 骑手ID
        public string? RiderId { get; set; }

        // 地址信息
        public string? Address { get; set; }
    }
}
