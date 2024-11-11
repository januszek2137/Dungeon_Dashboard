using System.ComponentModel.DataAnnotations;

namespace Dungeon_Dashboard.Models {

    public enum Classes {
        Barbarian,
        Bard,
        Cleric,
        Druid,
        Fighter,
        Monk,
        Paladin,
        Ranger,
        Rogue,
        Sorcerer,
        Warlock,
        Wizard
    }

    public enum Races {
        Dragonborn,
        Dwarf,
        Elf,
        Gnome,
        HalfElf,
        Halfling,
        HalfOrc,
        Human,
        Tiefling
    }

    public class CharacterModel {

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Class is required")]
        public Classes Class { get; set; }

        [Required(ErrorMessage = "Race is required")]
        public Races Race { get; set; }

        [Required(ErrorMessage = "Level is required")]
        [Range(1, 20, ErrorMessage = "Level must be between 1 and 20")]
        public int Level { get; set; }

        [Required(ErrorMessage = "Speed is required")]
        [Range(0, float.MaxValue, ErrorMessage = "Speed must be a non-negative number")]
        public float Speed { get; set; }

        [Required(ErrorMessage = "Armor Class is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Armor Class must be a non-negative number")]
        public int ArmorClass { get; set; }

        [Required(ErrorMessage = "Hit Points is required")]
        [Range(1, 1000, ErrorMessage = "Hit Points must be a positive number")]
        public int HitPoints { get; set; }

        [Required(ErrorMessage = "Strength is required")]
        [Range(3, 20, ErrorMessage = "Strength must be between 3 and 20")]
        public int Strength { get; set; }

        [Required(ErrorMessage = "Dexterity is required")]
        [Range(3, 20, ErrorMessage = "Dexterity must be between 3 and 20")]
        public int Dexterity { get; set; }

        [Required(ErrorMessage = "Constitution is required")]
        [Range(3, 20, ErrorMessage = "Constitution must be between 3 and 20")]
        public int Constitution { get; set; }

        [Required(ErrorMessage = "Intelligence is required")]
        [Range(3, 20, ErrorMessage = "Intelligence must be between 3 and 20")]
        public int Intelligence { get; set; }

        [Required(ErrorMessage = "Wisdom is required")]
        [Range(3, 20, ErrorMessage = "Wisdom must be between 3 and 20")]
        public int Wisdom { get; set; }

        [Required(ErrorMessage = "Charisma is required")]
        [Range(3, 20, ErrorMessage = "Charisma must be between 3 and 20")]
        public int Charisma { get; set; }

        public string[]? Skills { get; set; }
        public string[]? Equipment { get; set; }
        public string[]? Inventory { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Copper must be a non-negative number")]
        public int Copper { get; set; } = 0;

        [Range(0, int.MaxValue, ErrorMessage = "Silver must be a non-negative number")]
        public int Silver { get; set; } = 0;

        [Range(0, int.MaxValue, ErrorMessage = "Electrum must be a non-negative number")]
        public int Electrum { get; set; } = 0;

        [Range(0, int.MaxValue, ErrorMessage = "Gold must be a non-negative number")]
        public int Gold { get; set; } = 0;

        [Range(0, int.MaxValue, ErrorMessage = "Platinum must be a non-negative number")]
        public int Platinum { get; set; } = 0;

        public string CreatedBy { get; set; } = "admin";
    }
}