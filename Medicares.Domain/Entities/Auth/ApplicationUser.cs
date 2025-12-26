using Medicares.Domain.Entities.Common;
using Microsoft.AspNetCore.Identity;

namespace Medicares.Domain.Entities.Auth
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public Guid OwnerId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public Guid? AddressId { get; set; }
        public Address? Address { get; set; }
        public Owner? Owner { get; set; }
    }
}
