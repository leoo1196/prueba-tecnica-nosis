using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;
public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    public DbSet<Movie> Movies { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly, type => type.Namespace is not null && type.Namespace.Contains("TypeConfiguration"));
    }
}
