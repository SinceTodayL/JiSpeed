using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Merchant
{
    using JISpeed.Core.Entities.Admin; //引用 Admin 实体所在的命名空间

    //申请实体
    //对应数据库表: Application
    [Table("APPLICATION")]
    public class Application
    {
        [Key]
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string ApplyId { get; set; } //申请ID pk

        [StringLength(255)]
        public required string CompanyName { get; set; } //店名称

        public required DateTime SubmittedAt { get; set; } //提交时间

        public required int AuditStatus { get; set; } //审核状态

        public DateTime? AuditAt { get; set; } //审核时间 (可为空)

        [StringLength(65535)] // TEXT 类型
        public string? RejectReason { get; set; } //驳回原因

        [StringLength(450)]
        [Column(TypeName = "VARCHAR(450)")]
        public string? AdminId { get; set; } //管理员ID fk->Admin(adminId) (可为空)

        [StringLength(450)]
        [Column(TypeName = "VARCHAR(450)")]
        public required string MerchantId { get; set; } //商家ID fk->Merchant(merchantId)

        //导航属性
        [ForeignKey("AdminId")]
        public virtual Admin? Admin { get; set; } // 关联到 Admin 实体，可为空

        [ForeignKey("MerchantId")]
        public virtual required Merchant Merchant { get; set; } // 关联到 Merchant 实体

        public Application(string companyName, string merchantId)
        {
            ApplyId = Guid.NewGuid().ToString("N"); //生成唯一的申请ID
            CompanyName = companyName;
            MerchantId = merchantId;
            SubmittedAt = DateTime.UtcNow; //设置提交时间为当前时间
            AuditStatus = 0; //默认审核状态为0（待审核）
        }

        private Application() { }
    }
}