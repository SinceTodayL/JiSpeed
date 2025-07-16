using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Rider
{
    //骑手实体
    //对应数据库表: rider
    [Table("rider")]
    public class Rider
    {
        [Key]
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string RiderId { get; set; } //骑手编号 PK

        [StringLength(20)]
        public required string Name { get; set; } //骑手姓名

        [StringLength(11)]
        [Column(TypeName = "CHAR(11)")]
        public required string PhoneNumber { get; set; } //联系电话

        [StringLength(20)]
        public string? VehicleNumber { get; set; } //车牌号 (可为空)

        //导航属性
        public virtual ICollection<RiderSchedule> RiderSchedules { get; set; } = new List<RiderSchedule>(); //一个骑手可以有多个排班
        public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>(); //一个骑手可以有多个派单
        public virtual ICollection<Performance> Performances { get; set; } = new List<Performance>(); //一个骑手可以有多条绩效记录
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>(); //一个骑手可以有多条考勤记录

        public Rider(string name, string phoneNumber, string? vehicleNumber = null)
        {
            RiderId = Guid.NewGuid().ToString("N"); //生成唯一的骑手编号
            Name = name;
            PhoneNumber = phoneNumber;
            VehicleNumber = vehicleNumber; 
        }

        private Rider() { }
    }
}