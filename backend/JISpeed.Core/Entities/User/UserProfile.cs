using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.User
{
    //用户档案实体
    //对应数据库表: UserProfile
    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [StringLength(64)]
        [Column(TypeName = "CHAR(32)")]
        public required string UserId { get; set; } //用户ID pk, fk->User(userId)

        [StringLength(50)]
        public required string Nickname { get; set; } //昵称

        [StringLength(200)]
        public required string AvatarUrl { get; set; } //头像地址

        public required int Gender { get; set; } //性别 1: male, 2: female, 3: other

        public DateTime? Birthday { get; set; } //生日 (可为空，DATE 类型映射)

        public required int Level { get; set; } //会员等级 (默认为0)

        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public string? DefaultAddrId { get; set; } //默认地址ID fk->Address(addrId) (可为空)

        //导航属性
        [ForeignKey("UserId")]
        public virtual required User User { get; set; }

        [ForeignKey("DefaultAddrId")]
        public virtual Address? DefaultAddress { get; set; } //关联到 Address 实体，可为空

        public UserProfile(string userId, string nickname = "Default Nickname",
                   string avatarUrl = "Default Avatar URL", int gender = 3, 
                   DateTime? birthday = null, int level = 0)
        {
            UserId = userId;
            Nickname = nickname;
            AvatarUrl = avatarUrl;
            Gender = gender;
            Birthday = birthday;  
            Level = level;
        }

        private UserProfile() { } 
    }
}