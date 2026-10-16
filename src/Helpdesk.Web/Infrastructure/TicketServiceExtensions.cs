using Helpdesk.Web.Services;
using Helpdesk.Web.Services.Tickets;

namespace Helpdesk.Web.Infrastructure;

public static class TicketServiceExtensions
{
    public static IServiceCollection AddTicketServices(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<ITicketService, TicketService>();

        services.AddSingleton<ITicketFilter, StatusTicketFilter>();
        services.AddSingleton<ITicketFilter, PriorityTicketFilter>();
        services.AddSingleton<ITicketFilter, TitleSearchTicketFilter>();

        services.AddSingleton<ITicketSortStrategy, CreatedAtSortStrategy>();
        services.AddSingleton<ITicketSortStrategy, PrioritySortStrategy>();
        services.AddSingleton<ITicketSortStrategy, TitleSortStrategy>();

        return services;
    }
}
