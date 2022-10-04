using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.TypeConfiguration;
internal class MovieTypeConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.ToTable("Movies");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .HasMaxLength(500);

        builder.Property(e => e.Plot)
            .HasMaxLength(2000);

        builder.Property(e => e.ImdbRating)
            .HasMaxLength(5);

        builder.Property(e => e.Poster);

        builder.HasIndex(e => e.Title);
    }
}
