using Helpdesk.Web.Domain;

namespace Helpdesk.Web.Contracts;

public class TicketQuery
{
    /// <summary>
    /// Pokazuje tylko zgłoszenia o tym statusie.
    /// </summary>
    public TicketStatus? Status { get; set; }

    /// <summary>
    /// Pokazuje tylko zgłoszenia o tym priorytecie.
    /// </summary>
    public TicketPriority? Priority { get; set; }

    /// <summary>
    /// Fragment tytułu (wielkość liter nie ma znaczenia).
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Pole sortowania: CreatedAt, Priority lub Title.
    /// </summary>
    public TicketSortField SortBy { get; set; } = TicketSortField.CreatedAt;

    /// <summary>
    /// Sortowanie malejące (domyślnie true).
    /// </summary>
    public bool Descending { get; set; } = true;
}
