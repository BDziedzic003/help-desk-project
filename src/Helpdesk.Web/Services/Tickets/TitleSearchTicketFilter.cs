using Helpdesk.Web.Contracts;
using Helpdesk.Web.Domain;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Web.Services.Tickets;

public class TitleSearchTicketFilter : ITicketFilter
{
    private const string EscapeCharacter = "\\";

    public IQueryable<Ticket> Apply(IQueryable<Ticket> tickets, TicketQuery query)
    {
        var search = query.Search?.Trim();
        if (string.IsNullOrEmpty(search))
        {
            return tickets;
        }

        var pattern = $"%{EscapeLikeWildcards(search)}%";
        return tickets.Where(ticket => EF.Functions.Like(ticket.Title, pattern, EscapeCharacter));
    }

    private static string EscapeLikeWildcards(string value) =>
        value
            .Replace(EscapeCharacter, EscapeCharacter + EscapeCharacter)
            .Replace("%", EscapeCharacter + "%")
            .Replace("_", EscapeCharacter + "_");
}
