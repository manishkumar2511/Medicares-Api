namespace Medicares.Application.Contracts.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? OwnerId { get; }
}
