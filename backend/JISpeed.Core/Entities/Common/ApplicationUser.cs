using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Common
{
    //应用用户类，继承自 IdentityUser，用于管理用户认证信息
    public class ApplicationUser : IdentityUser
    {
        // 用户类型: 1-普通用户, 2-商家, 3-骑手, 4-管理员
        public int UserType { get; set; }
        
        // 账号状态: 1-active, 2-deleted, 3-locked
        public int Status { get; set; } = 1;
        
        // 创建时间
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        
        // 导航属性 - 各类型的具体实体
        [InverseProperty("ApplicationUser")]
        public virtual User.User? UserEntity { get; set; }
        
        [InverseProperty("ApplicationUser")]
        public virtual Merchant.Merchant? MerchantEntity { get; set; }
        
        [InverseProperty("ApplicationUser")]
        public virtual Rider.Rider? RiderEntity { get; set; }
        
        [InverseProperty("ApplicationUser")]
        public virtual Admin.Admin? AdminEntity { get; set; }
        
        // 主构造函数
        public ApplicationUser(string userName, int userType, string ?email = null) : base(userName)
        {
            Id = Guid.NewGuid().ToString();
            Email = email;
            UserType = userType;
            Status = 1;
            CreatedAt = DateTime.UtcNow;
            EmailConfirmed = false;
        }
        
        // 用于 EF Core 的私有构造函数
        private ApplicationUser() : base() { }
    }
}
