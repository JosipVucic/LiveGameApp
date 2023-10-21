using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.Extensions.Options;
using Duende.IdentityServer.EntityFramework.Options;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LiveGameApp.Models
{
    public partial class LiveGameAppContext : MyApiAuthorizationDbContext<Appuser, Role, Hasrole, int>
    {
        public object this[string propertyName]
        {
            get {
                return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }
        public LiveGameAppContext(DbContextOptions<LiveGameAppContext> options, IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {
        }

        public virtual DbSet<Appuser> Appuser { get; set; }
        public virtual DbSet<Author> Author { get; set; }
        public virtual DbSet<Directmessage> Directmessage { get; set; }
        public virtual DbSet<Friend> Friend { get; set; }
        public virtual DbSet<Friendrequest> Friendrequest { get; set; }
        public virtual DbSet<Game> Game { get; set; }
        public virtual DbSet<Gamegenre> Gamegenre { get; set; }
        public virtual DbSet<Hasrole> Hasrole { get; set; }
        public virtual DbSet<Invitation> Invitation { get; set; }
        public virtual DbSet<Inviterequesttype> Inviterequesttype { get; set; }
        public virtual DbSet<Isgenre> Isgenre { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<Owns> Owns { get; set; }
        public virtual DbSet<Participant> Participant { get; set; }
        public virtual DbSet<Participationrequest> Participationrequest { get; set; }
        public virtual DbSet<Plan> Plan { get; set; }
        public virtual DbSet<Plantype> Plantype { get; set; }
        public virtual DbSet<Player> Player { get; set; }
        public virtual DbSet<Privacytype> Privacytype { get; set; }
        public virtual DbSet<Reviewable> Reviewable { get; set; }
        public virtual DbSet<Reviews> Reviews { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Room> Room { get; set; }
        public virtual DbSet<Spectator> Spectator { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Appuser>(entity =>
            {
                entity.ToTable("appuser");

                entity.HasIndex(e => e.Email)
                    .HasName("appuser_email_key")
                    .IsUnique();

                entity.HasIndex(e => e.UserName)
                    .HasName("appuser_username_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnName("date_of_birth")
                    .HasColumnType("date");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email");

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasColumnName("password");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("username");
            });

            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.GameId })
                    .HasName("author_pkey");

                entity.ToTable("author");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.GameId).HasColumnName("game_id");

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.Author)
                    .HasForeignKey(d => d.GameId)
                    .HasConstraintName("author_game_id_fkey");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Author)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("author_user_id_fkey");
            });

            modelBuilder.Entity<Directmessage>(entity =>
            {
                entity.ToTable("directmessage");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("content");

                entity.Property(e => e.Datetime).HasColumnName("datetime");

                entity.Property(e => e.RecipientId).HasColumnName("recipient_id");

                entity.Property(e => e.SenderId).HasColumnName("sender_id");

                entity.HasOne(d => d.Recipient)
                    .WithMany(p => p.DirectmessageRecipient)
                    .HasForeignKey(d => d.RecipientId)
                    .HasConstraintName("directmessage_recipient_id_fkey");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.DirectmessageSender)
                    .HasForeignKey(d => d.SenderId)
                    .HasConstraintName("directmessage_sender_id_fkey");
            });

            modelBuilder.Entity<Friend>(entity =>
            {
                entity.HasKey(e => new { e.UserLowId, e.UserHighId })
                    .HasName("friend_pkey");

                entity.ToTable("friend");

                entity.Property(e => e.UserLowId).HasColumnName("user_low_id");

                entity.Property(e => e.UserHighId).HasColumnName("user_high_id");

                entity.HasOne(d => d.UserHigh)
                    .WithMany(p => p.FriendUserHigh)
                    .HasForeignKey(d => d.UserHighId)
                    .HasConstraintName("friend_user_high_id_fkey");

                entity.HasOne(d => d.UserLow)
                    .WithMany(p => p.FriendUserLow)
                    .HasForeignKey(d => d.UserLowId)
                    .HasConstraintName("friend_user_low_id_fkey");
            });

            modelBuilder.Entity<Friendrequest>(entity =>
            {
                entity.ToTable("friendrequest");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnName("message");

                entity.Property(e => e.RecipientId).HasColumnName("recipient_id");

                entity.Property(e => e.SenderId).HasColumnName("sender_id");

                entity.HasOne(d => d.Recipient)
                    .WithMany(p => p.FriendrequestRecipient)
                    .HasForeignKey(d => d.RecipientId)
                    .HasConstraintName("friendrequest_recipient_id_fkey");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.FriendrequestSender)
                    .HasForeignKey(d => d.SenderId)
                    .HasConstraintName("friendrequest_sender_id_fkey");
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.ToTable("game");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description");

                entity.Property(e => e.MaxPlayers).HasColumnName("max_players");

                entity.Property(e => e.MinPlayers).HasColumnName("min_players");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.Rules)
                    .IsRequired()
                    .HasColumnName("rules");

                entity.Property(e => e.ImageUrl)
                    .HasColumnName("image_url");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Game)
                    .HasForeignKey<Game>(d => d.Id)
                    .HasConstraintName("game_id_fkey");
            });

            modelBuilder.Entity<Gamegenre>(entity =>
            {
                entity.ToTable("gamegenre");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Hasrole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId })
                    .HasName("hasrole_pkey");

                entity.ToTable("hasrole");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Hasrole)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("hasrole_role_id_fkey");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Hasrole)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("hasrole_user_id_fkey");
            });

            modelBuilder.Entity<Invitation>(entity =>
            {
                entity.ToTable("invitation");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnName("message");

                entity.Property(e => e.PlanId).HasColumnName("plan_id");

                entity.Property(e => e.RecipientId).HasColumnName("recipient_id");

                entity.Property(e => e.TypeId).HasColumnName("type_id");

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.Invitation)
                    .HasForeignKey(d => d.PlanId)
                    .HasConstraintName("invitation_plan_id_fkey");

                entity.HasOne(d => d.Recipient)
                    .WithMany(p => p.Invitation)
                    .HasForeignKey(d => d.RecipientId)
                    .HasConstraintName("invitation_recipient_id_fkey");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Invitation)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("invitation_type_id_fkey");
            });

            modelBuilder.Entity<Inviterequesttype>(entity =>
            {
                entity.ToTable("inviterequesttype");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Isgenre>(entity =>
            {
                entity.HasKey(e => new { e.GameId, e.GenreId })
                    .HasName("isgenre_pkey");

                entity.ToTable("isgenre");

                entity.Property(e => e.GameId).HasColumnName("game_id");

                entity.Property(e => e.GenreId).HasColumnName("genre_id");

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.Isgenre)
                    .HasForeignKey(d => d.GameId)
                    .HasConstraintName("isgenre_game_id_fkey");

                entity.HasOne(d => d.Genre)
                    .WithMany(p => p.Isgenre)
                    .HasForeignKey(d => d.GenreId)
                    .HasConstraintName("isgenre_genre_id_fkey");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("message");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("content");

                entity.Property(e => e.Datetime).HasColumnName("datetime");

                entity.Property(e => e.RoomId).HasColumnName("room_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Message)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("message_room_id_fkey");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Message)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("message_user_id_fkey");
            });

            modelBuilder.Entity<Owns>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.GameId })
                    .HasName("owns_pkey");

                entity.ToTable("owns");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.GameId).HasColumnName("game_id");

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.Owns)
                    .HasForeignKey(d => d.GameId)
                    .HasConstraintName("owns_game_id_fkey");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Owns)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("owns_user_id_fkey");
            });

            modelBuilder.Entity<Participant>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoomId })
                    .HasName("participant_pkey");

                entity.ToTable("participant");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.RoomId).HasColumnName("room_id");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Participant)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("participant_room_id_fkey");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Participant)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("participant_user_id_fkey");
            });

            modelBuilder.Entity<Participationrequest>(entity =>
            {
                entity.ToTable("participationrequest");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnName("message");

                entity.Property(e => e.PlanId).HasColumnName("plan_id");

                entity.Property(e => e.SenderId).HasColumnName("sender_id");

                entity.Property(e => e.TypeId).HasColumnName("type_id");

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.Participationrequest)
                    .HasForeignKey(d => d.PlanId)
                    .HasConstraintName("participationrequest_plan_id_fkey");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.Participationrequest)
                    .HasForeignKey(d => d.SenderId)
                    .HasConstraintName("participationrequest_sender_id_fkey");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Participationrequest)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("participationrequest_type_id_fkey");
            });

            modelBuilder.Entity<Plan>(entity =>
            {
                entity.ToTable("plan");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Datetime).HasColumnName("datetime");

                entity.Property(e => e.GameId).HasColumnName("game_id");

                entity.Property(e => e.HostUserId).HasColumnName("host_user_id");

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasColumnName("location");

                entity.Property(e => e.MaxPlayers).HasColumnName("max_players");

                entity.Property(e => e.MaxSpectators).HasColumnName("max_spectators");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.PrivacyTypeId).HasColumnName("privacy_type_id");

                entity.Property(e => e.TypeId).HasColumnName("type_id");

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.Plan)
                    .HasForeignKey(d => d.GameId)
                    .HasConstraintName("plan_game_id_fkey");

                entity.HasOne(d => d.HostUser)
                    .WithMany(p => p.Plan)
                    .HasForeignKey(d => d.HostUserId)
                    .HasConstraintName("plan_host_user_id_fkey");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Plan)
                    .HasForeignKey<Plan>(d => d.Id)
                    .HasConstraintName("plan_id_fkey");

                entity.HasOne(d => d.PrivacyType)
                    .WithMany(p => p.Plan)
                    .HasForeignKey(d => d.PrivacyTypeId)
                    .HasConstraintName("plan_privacy_type_id_fkey");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Plan)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("plan_type_id_fkey");
            });

            modelBuilder.Entity<Plantype>(entity =>
            {
                entity.ToTable("plantype");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.PlanId })
                    .HasName("player_pkey");

                entity.ToTable("player");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.PlanId).HasColumnName("plan_id");

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.Player)
                    .HasForeignKey(d => d.PlanId)
                    .HasConstraintName("player_plan_id_fkey");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Player)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("player_user_id_fkey");
            });

            modelBuilder.Entity<Privacytype>(entity =>
            {
                entity.ToTable("privacytype");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Reviewable>(entity =>
            {
                entity.ToTable("reviewable");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AverageRating).HasColumnName("average_rating");
            });

            modelBuilder.Entity<Reviews>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.ReviewableId })
                    .HasName("reviews_pkey");

                entity.ToTable("reviews");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.ReviewableId).HasColumnName("reviewable_id");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("content");

                entity.Property(e => e.Rating).HasColumnName("rating");

                entity.HasOne(d => d.Reviewable)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.ReviewableId)
                    .HasConstraintName("reviews_reviewable_id_fkey");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("reviews_user_id_fkey");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("room");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Spectator>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.PlanId })
                    .HasName("spectator_pkey");

                entity.ToTable("spectator");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.PlanId).HasColumnName("plan_id");

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.Spectator)
                    .HasForeignKey(d => d.PlanId)
                    .HasConstraintName("spectator_plan_id_fkey");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Spectator)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("spectator_user_id_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
