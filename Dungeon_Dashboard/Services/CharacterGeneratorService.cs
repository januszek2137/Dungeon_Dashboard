using Dungeon_Dashboard.Models;

namespace Dungeon_Dashboard.Services {

    public interface ICharacterGeneratorService {

        NPC GenerateRandomNPC();

        Monster GenerateRandomMonster();

        RandomEncounter GenerateRandomEncounter();

        List<NPC> GenerateRandomNPCs(int count);

        List<Monster> GenerateRandomMonsters(int count);
    }

    public class CharacterGeneratorService : ICharacterGeneratorService {
        private readonly IDataService _dataService;
        private readonly ILogger<CharacterGeneratorService> _logger;
        private readonly Random _random;

        public CharacterGeneratorService(IDataService dataService, ILogger<CharacterGeneratorService> logger) {
            _dataService = dataService;
            _logger = logger;
            _random = new Random();
        }

        public NPC GenerateRandomNPC() {
            var npcNames = _dataService.GetNPCNamesAsync().Result;
            var npcRoles = _dataService.GetNPCRolesAsync().Result;
            var descriptions = _dataService.GetDescriptionsAsync().Result;

            if(!npcNames.Any() || !npcRoles.Any() || !descriptions.Any()) {
                _logger.LogWarning("One of the npc data pools is empty");
                return null;
            }

            var npc = new NPC {
                Id = GenerateUniqueId(),
                Name = GetRandomItem(npcNames),
                Role = GetRandomItem(npcRoles),
                Level = _random.Next(3, 21),
                Health = _random.Next(10, 251),
                ArmorClass = _random.Next(10, 21),
                Strength = _random.Next(3, 21),
                Dexterity = _random.Next(3, 21),
                Constitution = _random.Next(3, 21),
                Intelligence = _random.Next(3, 21),
                Wisdom = _random.Next(3, 21),
                Charisma = _random.Next(3, 21),
                Description = GetRandomItem(descriptions)
            };

            return npc;
        }

        public Monster GenerateRandomMonster() {
            var monsterSpecies = _dataService.GetMonsterSpeciesAsync().Result;
            var monsterTypes = _dataService.GetMonsterTypesAsync().Result;
            var monsterAbilities = _dataService.GetMonsterAbilitiesAsync().Result;
            var descriptions = _dataService.GetDescriptionsAsync().Result;

            if(!monsterSpecies.Any() || !monsterTypes.Any() || !monsterAbilities.Any() || !descriptions.Any()) {
                _logger.LogWarning("One of the monster data pools is empty");
                return null;
            }

            var monster = new Monster {
                Id = GenerateUniqueId(),
                Species = GetRandomItem(monsterSpecies),
                Type = GetRandomItem(monsterTypes),
                Level = _random.Next(3, 21),
                Health = _random.Next(10, 501),
                ArmorClass = _random.Next(10, 21),
                Damage = _random.Next(1, 51),
                Abilities = GetRandomItem(monsterAbilities),
                Description = GetRandomItem(descriptions)
            };
            return monster;
        }

        public RandomEncounter GenerateRandomEncounter() {
            var encounterNames = _dataService.GetEncounterNamesAsync().Result;
            var descriptions = _dataService.GetDescriptionsAsync().Result;
            var locations = _dataService.GetLocationsAsync().Result;
            var weathers = _dataService.GetWeathersAsync().Result;
            var timesOfDay = _dataService.GetTimesOfDayAsync().Result;
            var terrains = _dataService.GetTerrainsAsync().Result;
            var difficulties = _dataService.GetDifficultiesAsync().Result;
            var rewards = _dataService.GetRewardsAsync().Result;
            var notes = _dataService.GetNotesAsync().Result;

            if(!encounterNames.Any() || !descriptions.Any() || !locations.Any() || !weathers.Any() || !timesOfDay.Any() || !terrains.Any() || !difficulties.Any() || !rewards.Any() || !notes.Any()) {
                _logger.LogWarning("One of the encounter data pools is empty");
                return null;
            }

            var encounter = new RandomEncounter {
                Id = GenerateUniqueId(),
                Name = GetRandomItem(encounterNames),
                Description = GetRandomItem(descriptions),
                Location = GetRandomItem(locations),
                Weather = GetRandomItem(weathers),
                TimeOfDay = GetRandomItem(timesOfDay),
                Terrain = GetRandomItem(terrains),
                Difficulty = GetRandomItem(difficulties),
                Reward = GetRandomItem(rewards),
                Notes = GetRandomItem(notes),
                InvolvedNPCs = new List<NPC> { GenerateRandomNPC(), GenerateRandomNPC() },
                InvolvedMonsters = new List<Monster> { GenerateRandomMonster(), GenerateRandomMonster() }
            };

            return encounter;
        }

        public List<NPC> GenerateRandomNPCs(int count) {
            var npcs = new List<NPC>();

            for(int i = 0; i < count; i++) {
                var npc = GenerateRandomNPC();
                if(npc != null) {
                    npcs.Add(npc);
                }
            }
            return npcs;
        }

        public List<Monster> GenerateRandomMonsters(int count) {
            var monsters = new List<Monster>();

            for(int i = 0; i < count; i++) {
                var monster = GenerateRandomMonster();
                if(monster != null) {
                    monsters.Add(monster);
                }
            }
            return monsters;
        }

        private T GetRandomItem<T>(List<T> list) {
            if(list == null || list.Count == 0)
                throw new ArgumentException("The list cannot be empty");

            int index = _random.Next(list.Count);
            return list[index];
        }

        private int GenerateUniqueId() {
            return Guid.NewGuid().GetHashCode();
        }
    }
}