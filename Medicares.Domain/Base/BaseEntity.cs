using System.ComponentModel.DataAnnotations;
namespace Medicares.Domain.Base
{
    public abstract class BaseEntity : IEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
