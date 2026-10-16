using Helpdesk.Web.Domain;

namespace Helpdesk.Web.Contracts;

public class ChangeStatusRequest
{
    public TicketStatus Status { get; set; }
}
