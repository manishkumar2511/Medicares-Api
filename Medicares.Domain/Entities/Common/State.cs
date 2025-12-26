using Medicares.Domain.Base;
using System.ComponentModel.DataAnnotations;

namespace Medicares.Domain.Entities.Common
{
    public class State : BaseEntity
    {
        [Required]
        public required string Name { get; set; }
        public required string ShortName { get; set; }
    }
}
