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
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<SharedLink> SharedLinks { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

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

        modelBuilder.Entity<Wishlist>()
            .HasOne(w => w.User)
            .WithMany(u => u.Wishlists)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Session>()
            .HasOne(s => s.Book)
            .WithMany(b => b.Sessions)
            .HasForeignKey(s => s.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Session>()
            .HasIndex(s => s.Date);
        modelBuilder.Entity<Session>()
            .HasIndex(s => s.BookId);
        modelBuilder.Entity<Book>()
            .HasIndex(b => b.UserId);

        modelBuilder.Entity<Wishlist>()
            .HasIndex(w => new { w.UserId, w.Priority });
        modelBuilder.Entity<Wishlist>()
            .HasIndex(w => w.UserId);
            
        modelBuilder.Entity<SharedLink>()
            .HasIndex(s => s.Token)
            .IsUnique();
        modelBuilder.Entity<SharedLink>()
            .HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Subscription>()
            .HasIndex(s => new { s.SubscriberUserId, s.TargetUserId, s.ListType })
            .IsUnique();
        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.Subscriber)
            .WithMany(u => u.SubscriptionsAsSubscriber)
            .HasForeignKey(s => s.SubscriberUserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.TargetUser)
            .WithMany(u => u.SubscriptionsAsTarget)
            .HasForeignKey(s => s.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}