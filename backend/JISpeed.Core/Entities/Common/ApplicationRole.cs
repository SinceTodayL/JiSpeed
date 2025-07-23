using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JISpeed.Core.Entities.Common
{
    //应用角色类，继承自 IdentityRole，用于管理系统角色
    public class ApplicationRole : IdentityRole
    {
        // 角色描述
        [StringLength(256)]
        public string? Description { get; set; }
        
        // 角色类型: 1-普通用户角色, 2-商家角色, 3-骑手角色, 4-管理员角色
        public int RoleType { get; set; }
        
        // 带角色名的构造函数
        public ApplicationRole(string roleName) : base(roleName)
        {
        }
        
        // 带角色名和角色类型的构造函数
        public ApplicationRole(string roleName, int roleType) : base(roleName)
        {
            RoleType = roleType;
            Description = $"Default role for type {roleType}";
        }
        
        // 用于 EF Core 的私有构造函数
        private ApplicationRole() : base() { }
    }
}
