using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.User
{
    //地址实体
    //对应数据库表: Address
    [Table("ADDRESS")]
    public class Address
    {
        [Key]
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string AddressId { get; set; } //地址ID pk

        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string UserId { get; set; } //用户ID fk->User(userId)

        [StringLength(50)]
        public required string ReceiverName { get; set; } //收货人姓名

        [StringLength(20)]
        public required string ReceiverPhone { get; set; } //收货人电话

        [StringLength(200)]
        public required string DetailedAddress { get; set; } //详细地址

        public required int IsDefault { get; set; } //是否默认地址 1: 是, 2: 否

        //导航属性
        [ForeignKey("UserId")]
        public virtual required User User { get; set; }

        //构造函数用于创建新的地址实例
        public Address(string userId, string receiverName, string receiverPhone, string detailedAddress, int isDefault)
        {
            AddressId = Guid.NewGuid().ToString("N"); //生成32位的唯一地址ID
            UserId = userId;
            ReceiverName = receiverName;
            ReceiverPhone = receiverPhone;
            DetailedAddress = detailedAddress;
            IsDefault = isDefault;
        }

        //EF Core 需要一个无参构造函数用于实体创建
        private Address() { } 
    }
}