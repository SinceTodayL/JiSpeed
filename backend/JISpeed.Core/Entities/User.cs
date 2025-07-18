using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

// 继承于IndentityUser，后续考虑增加属性
namespace JISpeed.Core.Entities
{
    [Table("USERS", Schema = "BACKEND_WRITER_01")]
    public class UUser: IdentityUser { }
}
