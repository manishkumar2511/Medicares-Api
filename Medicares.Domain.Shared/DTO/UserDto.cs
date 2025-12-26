namespace Medicares.Domain.Shared.DTO;

public class UserDto
{
    public Guid? Id { get; set; }
    public Guid? OwnerId { get; set; }
    public Guid? RoleId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string ProfileImageUrl { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
