using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore; 

namespace JISpeed.Core.Entities.Rider
{
    //骑手排班联结实体 (多对多)
    //对应数据库表: rider_schedule
    [Table("rider_schedule")]
    [PrimaryKey(nameof(RiderId), nameof(ScheduleId))] //复合主键
    public class RiderSchedule
    {
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string RiderId { get; set; } //骑手编号 pk,fk->rider(riderId)

        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string ScheduleId { get; set; } //排班编号 pk,fk->schedule(schduleId)

        //导航属性
        [ForeignKey("RiderId")]
        public virtual required Rider Rider { get; set; }

        [ForeignKey("ScheduleId")]
        public virtual required Schedule Schedule { get; set; }

        public RiderSchedule(string riderId, string scheduleId)
        {
            RiderId = riderId;
            ScheduleId = scheduleId;
        }

        private RiderSchedule() { } 
    }
}