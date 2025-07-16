using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Rider
{
    //考勤实体
    //对应数据库表: attendance
    [Table("attendance")]
    public class Attendance
    {
        [Key]
        [Column(TypeName = "CHAR(32)")]
        public required string AttendanceId { get; set; } //考勤编号 PK

        public required DateTime CheckDate { get; set; } //考勤日期 (DATE 类型映射为 DateTime)

        public TimeSpan? CheckInAt { get; set; } //签到时间 (可为空，TIME 类型映射为 TimeSpan)

        public TimeSpan? CheckoutAt { get; set; } //签退时间 (可为空，TIME 类型映射为 TimeSpan)

        public required bool IsLate { get; set; } //是否迟到 (BOOLEAN)

        [Column(TypeName = "CHAR(32)")]
        public required string RiderId { get; set; } //骑手Id fk->rider(riderId)

        public required bool IsAbsent { get; set; } //是否缺勤 (BOOLEAN)

        //导航属性
        [ForeignKey("RiderId")]
        public virtual required Rider Rider { get; set; }

        //多对多联结表
        public virtual ICollection<ScheduleAttendance> ScheduleAttendances { get; set; } = new List<ScheduleAttendance>();
    }
}