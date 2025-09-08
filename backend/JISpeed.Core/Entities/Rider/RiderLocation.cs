using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Rider
{
    // 骑手定位实体
    // 对应数据库表: RIDER_LOCATION
    [Table("RIDER_LOCATION")]
    public class RiderLocation
    {
        [Key]
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string LocationId { get; set; } // 位置ID pk

        [StringLength(450)]
        [Column(TypeName = "VARCHAR(450)")]
        public required string RiderId { get; set; } // 骑手ID fk->rider(riderId)

        [Column(TypeName = "DECIMAL(10, 6)")]
        public required decimal Longitude { get; set; } // 经度

        [Column(TypeName = "DECIMAL(10, 6)")]
        public required decimal Latitude { get; set; } // 纬度

        public required DateTime LocationTime { get; set; } // 定位时间

        [Column(TypeName = "DECIMAL(10, 2)")]
        public decimal? Accuracy { get; set; } // 定位精度(米)，可为空

        [Column(TypeName = "DECIMAL(10, 2)")]
        public decimal? Speed { get; set; } // 速度(km/h)，可为空

        [Column(TypeName = "DECIMAL(5, 2)")]
        public decimal? Direction { get; set; } // 方向(0-360度)，可为空

        [StringLength(20)]
        public string? LocationType { get; set; } // 定位类型(GPS、网络等)，可为空

        public int Status { get; set; } = 1; // 状态(1:在线, 0:离线)

        // 导航属性
        [ForeignKey("RiderId")]
        public virtual required Rider Rider { get; set; }

        // 构造函数
        public RiderLocation(string riderId, decimal longitude, decimal latitude, DateTime locationTime)
        {
            LocationId = Guid.NewGuid().ToString("N");
            RiderId = riderId;
            Longitude = longitude;
            Latitude = latitude;
            LocationTime = locationTime;
            Status = 1; // 默认在线
        }

        // 无参构造函数(供EF Core使用)
        public RiderLocation() { }
    }
}
