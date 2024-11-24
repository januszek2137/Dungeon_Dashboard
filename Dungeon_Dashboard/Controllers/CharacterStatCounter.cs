namespace Dungeon_Dashboard.Controllers {

    public class CharacterStatCounter {

        private Dictionary<int, int> proficiencyBonus = new Dictionary<int, int> {
            { 1, 2 },
            { 2, 2 },
            { 3, 2 },
            { 4, 2 },
            { 5, 3 },
            { 6, 3 },
            { 7, 3 },
            { 8, 3 },
            { 9, 4 },
            { 10, 4 },
            { 11, 4 },
            { 12, 4 },
            { 13, 5 },
            { 14, 5 },
            { 15, 5 },
            { 16, 5 },
            { 17, 6 },
            { 18, 6 },
            { 19, 6 },
            { 20, 6 }
        };

        public CharacterStatCounter() {
        }

        public int CalculateProficiencyBonus(int level) {
            int proficiency = 0;
            for(int i = 1; i <= level; i++) {
                if(proficiencyBonus.TryGetValue(i, out int bonus)) {
                    proficiency += bonus;
                }
            }
            return proficiency;
        }

        public int CalculateStatModifier(int stat) {
            if(stat >= 3 && stat <= 20)
                return (stat - 10) / 2;
            else
                throw new ArgumentOutOfRangeException("Stat must be between 3 and 20");
        }

        public int CalculatePassiveWisdom(int wisdom) {
            if(wisdom >= 3 && wisdom <= 20) {
                return 10 + CalculateStatModifier(wisdom);
            } else {
                throw new ArgumentOutOfRangeException("Wisdom must be between 3 and 20");
            }
        }
    }
}