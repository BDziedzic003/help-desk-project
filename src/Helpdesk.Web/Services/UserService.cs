using Helpdesk.Web.Contracts;
using Helpdesk.Web.Data;
using Helpdesk.Web.Domain;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Web.Services;

public class UserService(HelpdeskDbContext dbContext, IPasswordHasher passwordHasher) : IUserService
{
    public async Task<ServiceResult<IReadOnlyList<UserDto>>> GetAllAsync(AuthenticatedUser actor)
    {
        if (actor.Role != Role.Admin)
        {
            return ServiceResult<IReadOnlyList<UserDto>>.Failure(ServiceError.Forbidden);
        }

        var users = await dbContext.Users.AsNoTracking()
            .OrderBy(user => user.Username)
            .Select(user => new UserDto(user.Id, user.Username, user.Role, user.Tickets.Count))
            .ToListAsync();

        return ServiceResult<IReadOnlyList<UserDto>>.Success(users);
    }

    public async Task<ServiceResult<UserDto>> GetByIdAsync(AuthenticatedUser actor, int userId)
    {
        if (actor.Role != Role.Admin)
        {
            return ServiceResult<UserDto>.Failure(ServiceError.Forbidden);
        }

        var user = await FindDtoAsync(userId);
        return user is null
            ? ServiceResult<UserDto>.Failure(ServiceError.NotFound)
            : ServiceResult<UserDto>.Success(user);
    }

    public async Task<ServiceResult<UserDto>> CreateAsync(AuthenticatedUser actor, CreateUserRequest request)
    {
        if (actor.Role != Role.Admin)
        {
            return ServiceResult<UserDto>.Failure(ServiceError.Forbidden);
        }

        var username = request.Username.Trim();
        if (await IsUsernameTakenAsync(username, exceptUserId: null))
        {
            return ServiceResult<UserDto>.Failure(ServiceError.UsernameTaken);
        }

        var user = new User
        {
            Username = username,
            PasswordHash = passwordHasher.Hash(request.Password),
            Role = request.Role
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return ServiceResult<UserDto>.Success(new UserDto(user.Id, user.Username, user.Role, 0));
    }

    public async Task<ServiceResult<UserDto>> UpdateAsync(AuthenticatedUser actor, int userId, UpdateUserRequest request)
    {
        if (actor.Role != Role.Admin)
        {
            return ServiceResult<UserDto>.Failure(ServiceError.Forbidden);
        }

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            return ServiceResult<UserDto>.Failure(ServiceError.NotFound);
        }

        var username = request.Username.Trim();
        if (await IsUsernameTakenAsync(username, exceptUserId: userId))
        {
            return ServiceResult<UserDto>.Failure(ServiceError.UsernameTaken);
        }

        if (user.Role == Role.Admin && request.Role != Role.Admin && await IsLastAdminAsync())
        {
            return ServiceResult<UserDto>.Failure(ServiceError.LastAdminProtected);
        }

        user.Username = username;
        user.Role = request.Role;
        if (!string.IsNullOrEmpty(request.NewPassword))
        {
            user.PasswordHash = passwordHasher.Hash(request.NewPassword);
        }

        await dbContext.SaveChangesAsync();

        return ServiceResult<UserDto>.Success((await FindDtoAsync(userId))!);
    }

    public async Task<ServiceResult> DeleteAsync(AuthenticatedUser actor, int userId)
    {
        if (actor.Role != Role.Admin)
        {
            return ServiceResult.Failure(ServiceError.Forbidden);
        }

        if (actor.Id == userId)
        {
            return ServiceResult.Failure(ServiceError.SelfDeletion);
        }

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            return ServiceResult.Failure(ServiceError.NotFound);
        }

        if (user.Role == Role.Admin && await IsLastAdminAsync())
        {
            return ServiceResult.Failure(ServiceError.LastAdminProtected);
        }

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();

        return ServiceResult.Success();
    }

    private Task<UserDto?> FindDtoAsync(int userId) =>
        dbContext.Users.AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => new UserDto(user.Id, user.Username, user.Role, user.Tickets.Count))
            .FirstOrDefaultAsync();

    private Task<bool> IsUsernameTakenAsync(string username, int? exceptUserId)
    {
        var normalized = username.ToLower();
        return dbContext.Users.AnyAsync(user => user.Username.ToLower() == normalized && user.Id != exceptUserId);
    }

    private async Task<bool> IsLastAdminAsync() =>
        await dbContext.Users.CountAsync(user => user.Role == Role.Admin) <= 1;
}
