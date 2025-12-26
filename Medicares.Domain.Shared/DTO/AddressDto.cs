namespace Medicares.Domain.Shared.DTO;

public class AddressDto
{
    public Guid Id { get; set; }
    public string AddressLine { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public Guid? StateId { get; set; }
    public string StateName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
