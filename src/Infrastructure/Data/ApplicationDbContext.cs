using Domain.Entities;
using Domain.Entities.Chat;
using Domain.Entities.Chats;
using Domain.Entities.Logs;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<LogType> LogTypesSet => Set<LogType>();
        public DbSet<Log> Logs => Set<Log>();
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatParticipant> ChatParticipants { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatMessageView> ChatMessageViews { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<LogType>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasMaxLength(255);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            builder.Entity<Log>(entity =>
            {
                entity.Property(e => e.Action).IsRequired().HasMaxLength(150);
                entity.Property(e => e.IpAddress).HasMaxLength(45);

                entity.HasOne(e => e.LogType)
                      .WithMany(t => t.Logs)
                      .HasForeignKey(e => e.LogTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<ChatParticipant>(entity =>
            {
                entity.HasKey(x => x.Id);

                // Composite primary key ensures each user can only participate once per chat
                entity.HasIndex(x => new { x.ChatId, x.UserId }).IsUnique();

                // Relationship: Participant belongs to a Chat
                // A Chat can have many participants
                entity.HasOne(x => x.Chat).WithMany(c => c.Participants).HasForeignKey(x => x.ChatId).OnDelete(DeleteBehavior.NoAction);

                // Relationship: Participant references an ApplicationUser
                // A user can belong to multiple chats
                entity.HasOne(x => x.User).WithMany(u => u.ChatParticipants).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<ChatMessage>(entity =>
            {
                // Relationship: Message belongs to a Chat
                // A chat can contain many messages
                entity.HasOne(x => x.Chat).WithMany(c => c.Messages).HasForeignKey(x => x.ChatId).OnDelete(DeleteBehavior.NoAction);

                // Relationship: Message sender is an ApplicationUser
                // Restrict delete prevents deleting a user if they have sent messages,
                // preserving chat history integrity (might be removed later as deleted users would still be fetched but being shown as 'unknown user')
                entity.HasOne(x => x.Sender).WithMany(u => u.SentMessages).HasForeignKey(x => x.SenderId).OnDelete(DeleteBehavior.Restrict);

                // Index to optimize retrieval of chat history ordered by creation time
                entity.HasIndex(x => new { x.ChatId, x.CreatedAt });
            });

            builder.Entity<ChatMessageView>(entity =>
            {
                entity.ToTable("ChatMessageView");

                entity.HasKey(x => x.Id);

                // Prevent duplicate views per user per message
                entity.HasIndex(x => new { x.ChatMessageId, x.UserId })
                      .IsUnique();

                entity.HasOne(x => x.ChatMessage)
                      .WithMany(m => m.Views)
                      .HasForeignKey(x => x.ChatMessageId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.User)
                      .WithMany()
                      .HasForeignKey(x => x.UserId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(x => x.UserId);
            });
        }
    }
}
