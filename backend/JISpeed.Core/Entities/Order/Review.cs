using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities.Order
{
    using JISpeed.Core.Entities.User; //引用 User 实体所在的命名空间
    using JISpeed.Core.Entities.Junctions; //引用联结表命名空间

    //评价实体
    //对应数据库表: Review
    [Table("REVIEW")]
    public class Review
    {
        [Key]
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string ReviewId { get; set; } //评价ID pk

        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string OrderId { get; set; } //订单ID fk->Order(orderId)

        [StringLength(450)]
        [Column(TypeName = "VARCHAR(450)")]
        public required string UserId { get; set; } //用户ID fk->User(userId)

        public required int Rating { get; set; } //评分 1-5分

        [StringLength(65535)] //TEXT类型
        public string? Content { get; set; } //评价内容

        public required int IsAnonymous { get; set; } //是否匿名 1: 是, 2: 否

        public required DateTime ReviewAt { get; set; } //评价时间

        //导航属性
        [ForeignKey("OrderId")]
        public virtual required Order Order { get; set; }

        [ForeignKey("UserId")]
        public virtual required User User { get; set; }

        // 多对多联结表
        public virtual ICollection<DishReview> DishReviews { get; set; } = new List<DishReview>();

        public Review(string orderId, string userId, int rating, string? content = null, int isAnonymous = 2)
        {
            ReviewId = Guid.NewGuid().ToString("N");
            OrderId = orderId;
            UserId = userId;
            Rating = rating;
            Content = content;
            IsAnonymous = isAnonymous;
            ReviewAt = DateTime.UtcNow; //默认评价时间为当前时间
        }

        private Review() { }
    }
}