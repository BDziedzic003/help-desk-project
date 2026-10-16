using Helpdesk.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.Web.Controllers;

public static class ServiceErrorExtensions
{
    public static ActionResult ToActionResult(this ControllerBase controller, ServiceError error) => error switch
    {
        ServiceError.NotFound => controller.NotFound(),
        ServiceError.Forbidden => controller.Forbid(),
        ServiceError.Locked or ServiceError.UsernameTaken or ServiceError.LastAdminProtected or ServiceError.SelfDeletion
            => controller.Conflict(new { error = ServiceErrorMessages.For(error) }),
        _ => controller.StatusCode(StatusCodes.Status500InternalServerError)
    };
}
