using System.ComponentModel.DataAnnotations.Schema;

namespace JISpeed.Core.Entities
{
    [Table("USERS", Schema = "BACKEND_WRITER_01")]
    public class UUser
    {
        [Column("ID")]
        public string Id { get; set; }

        [Column("PASSWORD")]
        public string Password { get; set; }
    }
}
