using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace JISpeed.Core.Entities
{
    [Table("USERS", Schema = "BACKEND_WRITER_01")]
    public class UUser: IdentityUser { }
}
