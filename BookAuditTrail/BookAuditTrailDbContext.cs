using Microsoft.EntityFrameworkCore;

namespace BookAuditTrail;

public class BookAuditTrailDbContext(DbContextOptions<BookAuditTrailDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<BookAuditLog> BookAuditLogs => Set<BookAuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Title).HasMaxLength(500).IsRequired();
            entity.Property(b => b.ShortDescription).HasMaxLength(2000);
            entity.Property(b => b.PublishDate).IsRequired();

            entity.HasMany(b => b.Authors)
                  .WithMany(a => a.Books)
                  .UsingEntity(j => j.ToTable("BookAuthors"));

            entity.HasMany(b => b.AuditLogs)
                  .WithOne(a => a.Book)
                  .HasForeignKey(a => a.BookId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Name).HasMaxLength(300).IsRequired();
            entity.HasIndex(a => a.Name).IsUnique();
        });

        modelBuilder.Entity<BookAuditLog>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.ChangeType).HasMaxLength(50).IsRequired();
            entity.Property(a => a.FieldName).HasMaxLength(100).IsRequired();
            entity.Property(a => a.OldValue).HasMaxLength(2000);
            entity.Property(a => a.NewValue).HasMaxLength(2000);
            entity.Property(a => a.Description).HasMaxLength(2000).IsRequired();
            entity.Property(a => a.ChangedAt).IsRequired();

            entity.HasIndex(a => a.BookId);
            entity.HasIndex(a => a.ChangedAt);
            entity.HasIndex(a => a.ChangeType);
        });
    }
}
