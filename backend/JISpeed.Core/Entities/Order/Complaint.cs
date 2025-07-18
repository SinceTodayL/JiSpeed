using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Order
{
    using JISpeed.Core.Entities.User; //引用 User 实体所在的命名空间

    //投诉实体
    //对应数据库表: Complaint
    [Table("COMPLAINT")]
    public class Complaint
    {
        [Key]
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string ComplaintId { get; set; } //投诉ID pk

        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string OrderId { get; set; } //订单ID fk->Order(orderId)

        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string ComplainantId { get; set; } //投诉人ID fk->User(userId)

        public required int CmplRole { get; set; } //投诉人角色 1: 用户, 2: 商家, 3: 骑手

        [StringLength(65535)] //TEXT类型
        public string? CmplDescription { get; set; } //投诉描述

        public required int CmplStatus { get; set; } //投诉状态 1: 待处理, 2: 处理中, 3: 已解决, 4: 已关闭

        public required DateTime CreatedAt { get; set; } //投诉创建时间

        //导航属性
        [ForeignKey("OrderId")]
        public virtual required Order Order { get; set; }

        [ForeignKey("ComplainantId")]
        public virtual required User Complainant { get; set; } // 关联到 User 实体

        public Complaint(string orderId, int role, string? description = null)
        {
            OrderId = orderId;
            ComplainantId = Guid.NewGuid().ToString("N");
            CmplRole = role;
            CmplDescription = description;
            CmplStatus = 1; //默认状态为待处理
            CreatedAt = DateTime.UtcNow; //设置创建时间为当前时间
        }

        private Complaint() { }
    }
}