namespace Helpdesk.Web.Services;

public enum ServiceError
{
    None = 0,
    NotFound = 1,
    Forbidden = 2,
    Locked = 3,
    UsernameTaken = 4,
    LastAdminProtected = 5,
    SelfDeletion = 6
}

public record ServiceResult(ServiceError Error)
{
    public bool IsSuccess => Error == ServiceError.None;

    public static ServiceResult Success() => new(ServiceError.None);

    public static ServiceResult Failure(ServiceError error) => new(error);
}

public record ServiceResult<T>(T? Value, ServiceError Error)
{
    public bool IsSuccess => Error == ServiceError.None;

    public static ServiceResult<T> Success(T value) => new(value, ServiceError.None);

    public static ServiceResult<T> Failure(ServiceError error) => new(default, error);
}
