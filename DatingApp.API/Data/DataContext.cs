using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Like>()
                        .HasKey(x => new { x.LikerId, x.LikeeId });

            modelBuilder.Entity<Like>()
                        .HasOne(x => x.Likee)
                        .WithMany(x => x.Likers)
                        .HasForeignKey(x => x.LikeeId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Like>()
                        .HasOne(x => x.Liker)
                        .WithMany(x => x.Likees)
                        .HasForeignKey(x => x.LikerId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                        .HasOne(x => x.Sender)
                        .WithMany(x => x.MessagesSent)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                        .HasOne(x => x.Recipient)
                        .WithMany(x => x.MessagesReceived)
                        .OnDelete(DeleteBehavior.Restrict);
        }
    }
}