using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.User
{
    //地址实体
    //对应数据库表: Address
    [Table("Address")]
    public class Address
    {
        [Key]
        [Column(TypeName = "CHAR(32)")]
        public required string AddressId { get; set; } //地址ID pk

        [Column(TypeName = "CHAR(32)")]
        public required string UserId { get; set; } //用户ID fk->User(userId)

        [Required]
        [StringLength(50)]
        public required string ReceiverName { get; set; } //收货人姓名

        [Required]
        [StringLength(20)]
        public required string ReceiverPhone { get; set; } //收货人电话

        [Required]
        [StringLength(200)]
        public required string DetailedAddress { get; set; } //详细地址

        public required int IsDefault { get; set; } //是否默认地址 1: 是, 2: 否

        //导航属性
        [ForeignKey("UserId")]
        public virtual required User User { get; set; }
    }
}