using Medicares.Domain.Base;
using Medicares.Domain.Entities.Personnel;

namespace Medicares.Domain.Entities.Auth
{
    public class Owner : BaseEntity, IAuditableEntity, IDeleteEntity
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? LogoUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? DeactivatedAt { get; set; }

        public bool IsSubscriptionActive { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? DeletedBy { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        public ICollection<Store> Stores { get; set; } = new List<Store>();
        public ICollection<ApplicationUser> ApplicationUser { get; set; } = new List<ApplicationUser>();

    }
}
