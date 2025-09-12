using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Rider
{
    using JISpeed.Core.Entities.Common;
    
    /// 骑手实体 - 对应数据库表: Rider
    [Table("RIDER")]
    public class Rider
    {
        [Key]
        [StringLength(450)]
        [Column(TypeName = "VARCHAR(450)")]
        public required string RiderId { get; set; }
        
        [StringLength(50)]
        public required string Name { get; set; }
        
        [StringLength(20)]
        public required string PhoneNumber { get; set; }
        
        [StringLength(20)]
        public string? VehicleNumber { get; set; }
        
        //身份验证用户关联
        public string? ApplicationUserId { get; set; }

        //导航属性
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser? ApplicationUser { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public virtual ICollection<Performance> Performances { get; set; } = new List<Performance>();
        public virtual ICollection<RiderSchedule> RiderSchedules { get; set; } = new List<RiderSchedule>();
        public virtual ICollection<RiderLocation> Locations { get; set; } = new List<RiderLocation>();

        //主构造函数
        public Rider(string riderId, string name, string phoneNumber, string? applicationUserId = null)
        {
            RiderId = riderId;
            Name = name;
            PhoneNumber = phoneNumber;
            ApplicationUserId = applicationUserId;
        }
        
        //用于 EF Core 的无参构造函数
        public Rider() { }
    }
}
