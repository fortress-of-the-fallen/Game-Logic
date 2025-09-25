using System.ComponentModel.DataAnnotations.Schema;

namespace Game.Application.Domain.Entity
{
    [Table("Settings")]
    public class Setting : BaseEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}