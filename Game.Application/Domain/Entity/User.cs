using System.ComponentModel.DataAnnotations.Schema;

namespace Game.Application.Domain.Entity
{
    [Table("User")]
    public class User : BaseEntity
    {
        // public string Username { get; set; }
        public string SessionId { get; set; }
    }
}