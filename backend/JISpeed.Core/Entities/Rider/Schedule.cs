using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Rider
{
    //排班实体
    //对应数据库表: schedule
    [Table("schedule")]
    public class Schedule
    {
        [Key]
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string ScheduleId { get; set; } //排班编号 PK

        public required DateTime WorkDate { get; set; } //工作日期

        public required DateTime ShiftStart { get; set; } //班次开始

        public required DateTime ShiftEnd { get; set; } //班次结束

        //导航属性
        public virtual ICollection<RiderSchedule> RiderSchedules { get; set; } = new List<RiderSchedule>(); // 一个排班可以有多个骑手
        public virtual ICollection<ScheduleAttendance> ScheduleAttendances { get; set; } = new List<ScheduleAttendance>(); // 多对多联结表

        public Schedule(DateTime workDate, DateTime shiftStart, DateTime shiftEnd)
        {
            ScheduleId = Guid.NewGuid().ToString("N"); //生成唯一的排班编号
            WorkDate = workDate;
            ShiftStart = shiftStart;
            ShiftEnd = shiftEnd;
        }

        private Schedule() { }
    }
}