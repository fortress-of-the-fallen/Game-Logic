using System.ComponentModel.DataAnnotations.Schema;

namespace Game.Application.Domain.Entity
{
    [Table("Samples")]
    public class Sample : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}