using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;
using SchedulePlannerBack.Domain.Entity;
using SchedulePlannerBack.Domain.Infrastructure;

namespace SchedulePlannerBack.Domain;

public class SchedulePlannerDbContext : DbContext
{
    public SchedulePlannerDbContext(IConfigurationDatabase configurationDatabase)
    {
        _configurationDatabase = configurationDatabase ?? throw new ArgumentNullException(nameof(configurationDatabase));
    }

    public SchedulePlannerDbContext()
    {
        _configurationDatabase = new DefaultConfigurationDatabase();
    }
    private readonly IConfigurationDatabase _configurationDatabase;
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configurationDatabase?.ConnectionString);
        optionsBuilder.UseExceptionProcessor();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<User>()
            .HasIndex(x => x.Login)
            .IsUnique();
        modelBuilder.Entity<User>()
            .HasMany(x => x.Events)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<User>()
            .HasMany(x => x.Participants)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Participant>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<Event>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<EventDate>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
    }

    public DbSet<Event> Events {get; set;}
    public DbSet<EventDate> EventDates {get; set;}
    public DbSet<Participant> Participants {get; set;}
    public DbSet<User> Users {get; set;}
}