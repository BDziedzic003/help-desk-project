using Helpdesk.Web.Contracts;

namespace Helpdesk.Web.Services;

/// <summary>
/// User management. Every operation requires an administrator actor.
/// </summary>
public interface IUserService
{
    Task<ServiceResult<IReadOnlyList<UserDto>>> GetAllAsync(AuthenticatedUser actor);

    Task<ServiceResult<UserDto>> GetByIdAsync(AuthenticatedUser actor, int userId);

    Task<ServiceResult<UserDto>> CreateAsync(AuthenticatedUser actor, CreateUserRequest request);

    Task<ServiceResult<UserDto>> UpdateAsync(AuthenticatedUser actor, int userId, UpdateUserRequest request);

    Task<ServiceResult> DeleteAsync(AuthenticatedUser actor, int userId);
}
