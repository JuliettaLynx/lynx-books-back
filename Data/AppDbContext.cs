using LynxBooks.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace LynxBooks.Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Session> Sessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Уникальный email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Каскадное удаление: при удалении пользователя удаляются его книги и сессии
        modelBuilder.Entity<Book>()
            .HasOne(b => b.User)
            .WithMany(u => u.Books)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Session>()
            .HasOne(s => s.User)
            .WithMany(u => u.Sessions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь сессии с книгой
        modelBuilder.Entity<Session>()
            .HasOne(s => s.Book)
            .WithMany(b => b.Sessions)
            .HasForeignKey(s => s.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        // Индексы для ускорения запросов
        modelBuilder.Entity<Session>()
            .HasIndex(s => s.Date);
        modelBuilder.Entity<Session>()
            .HasIndex(s => s.BookId);
        modelBuilder.Entity<Book>()
            .HasIndex(b => b.UserId);
    }
}