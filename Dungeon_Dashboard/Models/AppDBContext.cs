using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dungeon_Dashboard.Models {

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

        public DbSet<Dungeon_Dashboard.Models.EventModel> EventModel { get; set; } = default!;
        public DbSet<Dungeon_Dashboard.Models.CharacterModel> CharacterModel { get; set; } = default!;
    }
}