using Dungeon_Dashboard.Event;
using Dungeon_Dashboard.Group;
using Dungeon_Dashboard.Group.Notes;
using Dungeon_Dashboard.Invitations;
using Dungeon_Dashboard.PlayerCharacters;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dungeon_Dashboard.Home {

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

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<RoomModel>()
                .HasMany(r => r.Notes)
                .WithOne(n => n.Room)
                .HasForeignKey(n => n.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}