using Reminlo.Domain.Entities.Identity;
using MediatR;

namespace Reminlo.Domain.DomainEvents.Users;

/// <summary>
/// Triggered after a user is created.
/// </summary>
public record UserCreatedEvent(ApplicationUser User) : INotification;
