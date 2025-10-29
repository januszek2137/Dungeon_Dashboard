using Dungeon_Dashboard.Event.Models;
using Dungeon_Dashboard.Invitations.Models;
using Dungeon_Dashboard.PlayerCharacters.Models;
using Dungeon_Dashboard.Room.Models;
using Dungeon_Dashboard.Room.Notes.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dungeon_Dashboard.Home.Data {

    public class AppDBContext : IdentityDbContext {

        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) {
            /* TODO : Add DbSet properties for each model class
            public DbSet<Monster> Monsters { get; set; }
            public DbSet<Hero> Heroes { get; set; }
            public DbSet<Quest> Quests { get; set; }
            public DbSet<Party> Parties { get; set; }
            public DbSet<PartyMember> PartyMembers { get; set; }
            public DbSet<PartyQuest> PartyQuests { get; set; }
            public DbSet<PartyMonster> PartyMonsters { get; set; }
            */
        }

        public DbSet<EventModel> EventModel { get; set; } = default!;
        public DbSet<CharacterModel> CharacterModel { get; set; } = default!;
        public DbSet<InvitationModel> InvitationModel { get; set; } = default!;
        public DbSet<RoomModel> RoomModel { get; set; } = default!;
        public DbSet<NoteModel> NoteModel { get; set; } = default!;
     
        public DbSet<MarkerModel> MarkerModel { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<RoomModel>()
                .HasMany(r => r.Notes)
                .WithOne(n => n.Room)
                .HasForeignKey(n => n.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RoomModel>()
                .HasMany(r=> r.Markers)
                .WithOne(m => m.Room!)
                .HasForeignKey(m=> m.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<MarkerModel>(entity => {
                entity.Property(m => m.UserId).IsRequired();
            });
        }
    }
}