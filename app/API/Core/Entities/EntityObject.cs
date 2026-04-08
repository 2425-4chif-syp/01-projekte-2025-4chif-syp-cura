using Core.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class EntityObject : IEntityObject
    {
        [Key]
        public int Id { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
