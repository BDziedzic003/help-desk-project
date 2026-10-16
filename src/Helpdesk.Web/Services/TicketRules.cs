using Helpdesk.Web.Domain;

namespace Helpdesk.Web.Services;

public static class TicketRules
{
    /// <summary>
    /// A completed ticket can no longer be edited or deleted by a regular user; admins are not affected.
    /// </summary>
    public static bool IsLockedFor(Role role, TicketStatus status) =>
        role != Role.Admin && status == TicketStatus.Completed;
}
