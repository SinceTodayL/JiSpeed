using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Core.Entities.Rider
{
    //排班考勤联结实体 (多对多)
    //对应数据库表: schedule_attendance
    [Table("SCHEDULE_ATTENDANCE")]
    [PrimaryKey(nameof(ScheduleId), nameof(AttendanceId))] //复合主键
    public class ScheduleAttendance
    {
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string ScheduleId { get; set; } //排班编号 PK,fk->schedule(scheduleId)

        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string AttendanceId { get; set; } //考勤编号 PK ,fk->attendance(attendanceId)

        //导航属性
        [ForeignKey("ScheduleId")]
        public virtual required Schedule Schedule { get; set; }

        [ForeignKey("AttendanceId")]
        public virtual required Attendance Attendance { get; set; }

        public ScheduleAttendance(string scheduleId, string attendanceId)
        {
            ScheduleId = scheduleId;
            AttendanceId = attendanceId;
        }

        public ScheduleAttendance() { }
    }
}