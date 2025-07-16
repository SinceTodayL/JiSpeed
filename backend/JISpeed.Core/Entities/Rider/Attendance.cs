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
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string AttendanceId { get; set; } //考勤编号 PK

        public required DateTime CheckDate { get; set; } //考勤日期 

        public DateTime? CheckInAt { get; set; } //签到时间 

        public DateTime? CheckoutAt { get; set; } //签退时间 

        public required int IsLate { get; set; } //是否迟到 

        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string RiderId { get; set; } //骑手Id fk->rider(riderId)

        public required int IsAbsent { get; set; } //是否缺勤

        //导航属性
        [ForeignKey("RiderId")]
        public virtual required Rider Rider { get; set; }

        //多对多联结表
        public virtual ICollection<ScheduleAttendance> ScheduleAttendances { get; set; } = new List<ScheduleAttendance>();

        public Attendance(string riderId, DateTime checkDate, int isLate, int isAbsent,
                          DateTime? checkInAt = null, DateTime? checkoutAt = null)
        {
            AttendanceId = Guid.NewGuid().ToString("N"); //生成唯一的考勤编号
            RiderId = riderId;
            CheckDate = checkDate;
            IsLate = isLate;
            IsAbsent = isAbsent;
            CheckInAt = checkInAt; //默认未签到
            CheckoutAt = checkoutAt; //默认未签退
        }

        private Attendance() { } 
    }
}