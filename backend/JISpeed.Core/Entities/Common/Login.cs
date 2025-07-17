using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Core.Entities.Common
{
    //登录实体
    //对应数据库表: Login
    [Table("Login")]
    [Index(nameof(Account), IsUnique = true)]
    public class Login
    {
        [Key]
        [StringLength(32)]
        public required string Account { get; set; } //账号名 pk

        //TODO:增加更多登录方式

        [StringLength(64)]
        public required string PasswordHash { get; set; } //密码哈希

        public required int LoginRole { get; set; } //登录类型 1: 用户, 2: 商家, 3：骑手, 4: 管理员

        public required int Status { get; set; } //账号状态 1: active, 2: deleted, 3: locked

        public required DateTime CreatedAt { get; set; } //注册时间

        public DateTime? DeletedAt { get; set; } //注销时间 (可为空)

        public DateTime? LastLoginAt { get; set; } //最后登录时间 (可为空)

        [StringLength(20)]
        public string? LastLoginIp { get; set; } //最后登录IP (可为空)

        [StringLength(32)]
        public required string RoleId { get; set; } //对应的角色ID(和userId, merchantId, riderId, adminId关联)

        public Login(string account, string passwordHash, int loginRole)
        {
            RoleId = Guid.NewGuid().ToString("N"); //生成唯一的角色ID
            Account = account;
            PasswordHash = passwordHash;
            LoginRole = loginRole;
            Status = 1; //默认状态为 active
            CreatedAt = DateTime.UtcNow; //设置注册时间为当前时间
        }

        private Login() { }
    }
}