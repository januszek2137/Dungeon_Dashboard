using System.ComponentModel.DataAnnotations.Schema;

namespace Dungeon_Dashboard.ContentGeneration.Models {

    [NotMapped]
    public class NPC {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public int Level { get; set; }
        public int Health { get; set; }
        public int ArmorClass { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }
        public string Description { get; set; }
    }

    [NotMapped]
    public class Monster {
        public int Id { get; set; }
        public string Species { get; set; }
        public string Type { get; set; }
        public int Level { get; set; }
        public int Health { get; set; }
        public int ArmorClass { get; set; }
        public int Damage { get; set; }
        public string Abilities { get; set; }
        public string Description { get; set; }
    }

    [NotMapped]
    public class RandomEncounter {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Weather { get; set; }
        public string TimeOfDay { get; set; }
        public string Terrain { get; set; }
        public string Difficulty { get; set; }
        public string Reward { get; set; }
        public string Notes { get; set; }
        public List<NPC> InvolvedNPCs { get; set; }
        public List<Monster> InvolvedMonsters { get; set; }
    }
}