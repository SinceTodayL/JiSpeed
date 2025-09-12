using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore; 

namespace JISpeed.Core.Entities.Dish
{
    //菜品分类实体
    //对应数据库表: Category (隐含)
    [Table("CATEGORY")]
    public class Category
    {
        [Key]
        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public required string CategoryId { get; set; } //分类ID pk

        [StringLength(50)]
        public required string CategoryName { get; set; } //分类名称

        [StringLength(32)]
        [Column(TypeName = "CHAR(32)")]
        public string? ParentId { get; set; } //父级分类ID (可为空，表示顶级分类)

        public required int SortOrder { get; set; } //排序顺序

        //导航属性
        //自引用导航属性：父级分类
        [ForeignKey("ParentId")]
        public virtual Category? ParentCategory { get; set; } //关联到自身，表示父级分类

        //自引用导航属性：子级分类集合
        public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>(); //关联到自身，表示所有子级分类

        //该分类下的菜品集合
        public virtual ICollection<Dish> Dishes { get; set; } = new List<Dish>(); //一个分类可以有多个菜品

        public Category(string categoryName, string? parentId = null, int sortOrder = 0)
        {
            CategoryId = Guid.NewGuid().ToString("N"); //生成唯一的分类ID
            CategoryName = categoryName;
            ParentId = parentId;
            SortOrder = sortOrder;
        }
        
        private Category() { } 
    }
}