using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public abstract class EntityObject
    {
        [Key]
        public int Id { get; set; }
    }
}
