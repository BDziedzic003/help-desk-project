using Helpdesk.Web.Domain;

namespace Helpdesk.Web.Contracts;

public class TicketQuery
{
    public TicketStatus? Status { get; set; }

    public TicketPriority? Priority { get; set; }

    public string? Search { get; set; }

    public TicketSortField SortBy { get; set; } = TicketSortField.CreatedAt;

    public bool Descending { get; set; } = true;
}
