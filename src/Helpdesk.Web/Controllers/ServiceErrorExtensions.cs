using Helpdesk.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.Web.Controllers;

public static class ServiceErrorExtensions
{
    public const string TicketLockedMessage = "Zakończone zgłoszenie nie może być edytowane ani usunięte.";

    public static ActionResult ToActionResult(this ControllerBase controller, ServiceError error) => error switch
    {
        ServiceError.NotFound => controller.NotFound(),
        ServiceError.Forbidden => controller.Forbid(),
        ServiceError.Locked => controller.Conflict(new { error = TicketLockedMessage }),
        _ => controller.StatusCode(StatusCodes.Status500InternalServerError)
    };
}
