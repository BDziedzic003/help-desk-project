using Helpdesk.Web.Domain;

namespace Helpdesk.Web.Infrastructure;

/// <summary>
/// Polish labels and Bootstrap styles used when ticket data is shown in the UI.
/// </summary>
public static class TicketDisplayExtensions
{
    private const string DateFormat = "yyyy-MM-dd HH:mm";

    public static string ToDisplayName(this TicketStatus status) => status switch
    {
        TicketStatus.Open => "Otwarte",
        TicketStatus.InProgress => "W trakcie",
        TicketStatus.Completed => "Zakończone",
        _ => status.ToString()
    };

    public static string ToDisplayName(this TicketPriority priority) => priority switch
    {
        TicketPriority.Low => "Niski",
        TicketPriority.Medium => "Średni",
        TicketPriority.High => "Wysoki",
        _ => priority.ToString()
    };

    public static string ToBadgeClass(this TicketStatus status) => status switch
    {
        TicketStatus.Open => "bg-primary",
        TicketStatus.InProgress => "bg-warning text-dark",
        TicketStatus.Completed => "bg-success",
        _ => "bg-secondary"
    };

    public static string ToBadgeClass(this TicketPriority priority) => priority switch
    {
        TicketPriority.Low => "bg-secondary",
        TicketPriority.Medium => "bg-info text-dark",
        TicketPriority.High => "bg-danger",
        _ => "bg-secondary"
    };

    /// <summary>
    /// Dates are stored in UTC (SQLite returns them without a kind); show them in the server's local time.
    /// </summary>
    public static string ToDisplayString(this DateTime utcDate) =>
        DateTime.SpecifyKind(utcDate, DateTimeKind.Utc).ToLocalTime().ToString(DateFormat);
}
