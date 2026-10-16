using Helpdesk.Web.Domain;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Web.Data;

public class HelpdeskDbContext(DbContextOptions<HelpdeskDbContext> options) : DbContext(options)
{
    public const int UsernameMaxLength = 50;
    public const int TicketTitleMaxLength = 200;
    public const int TicketDescriptionMaxLength = 4000;

    public DbSet<User> Users => Set<User>();

    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUser(modelBuilder);
        ConfigureTicket(modelBuilder);
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(user => user.Username).IsUnique();
            entity.Property(user => user.Username).IsRequired().HasMaxLength(UsernameMaxLength);
            entity.Property(user => user.PasswordHash).IsRequired();
            entity.Property(user => user.Role).HasConversion<string>().HasMaxLength(20);
        });
    }

    private static void ConfigureTicket(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.Property(ticket => ticket.Title).IsRequired().HasMaxLength(TicketTitleMaxLength);
            entity.Property(ticket => ticket.Description).IsRequired().HasMaxLength(TicketDescriptionMaxLength);
            entity.Property(ticket => ticket.Priority).HasConversion<string>().HasMaxLength(20);
            entity.Property(ticket => ticket.Status).HasConversion<string>().HasMaxLength(20);
            entity.HasOne(ticket => ticket.User)
                .WithMany(user => user.Tickets)
                .HasForeignKey(ticket => ticket.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
