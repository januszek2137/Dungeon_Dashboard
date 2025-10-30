using Dungeon_Dashboard.ContentGeneration.Models;

namespace Dungeon_Dashboard.ContentGeneration.Services {

    public class GenerationFailedException : Exception
    {
        public GenerationFailedException(string message) : base(message) { }
        public GenerationFailedException(string message, Exception innerException)
            : base(message, innerException) { }
    }
    
    public interface IContentGenerationService {

        Task<NPC> GenerateRandomNPC();

        Task<Monster> GenerateRandomMonster();

        Task<RandomEncounter> GenerateRandomEncounter();

        Task<List<NPC>> GenerateRandomNPCs(int count);

        Task<List<Monster>> GenerateRandomMonsters(int count);
    }

    public class ContentGenerationService : IContentGenerationService {
        private readonly IDataService _dataService;
        private readonly ILogger<ContentGenerationService> _logger;
        private readonly Random _random;

        public ContentGenerationService(IDataService dataService, ILogger<ContentGenerationService> logger) {
            _dataService = dataService;
            _logger = logger;
            _random = new Random();
        }

        public async Task<NPC> GenerateRandomNPC() {
            var npcNames = await _dataService.GetNPCNamesAsync();
            var npcRoles = await _dataService.GetNPCRolesAsync();
            var descriptions = await _dataService.GetDescriptionsAsync();

            if(!npcNames.Any() || !npcRoles.Any() || !descriptions.Any()) {
                _logger.LogWarning("One of the npc data pools is empty");
                throw new GenerationFailedException("Insufficient data to generate NPC");
            }

            var npc = new NPC {
                Id = GenerateUniqueId(),
                Name = await GetRandomItem(npcNames),
                Role = await GetRandomItem(npcRoles),
                Level = _random.Next(3, 21),
                Health = _random.Next(10, 251),
                ArmorClass = _random.Next(10, 21),
                Strength = _random.Next(3, 21),
                Dexterity = _random.Next(3, 21),
                Constitution = _random.Next(3, 21),
                Intelligence = _random.Next(3, 21),
                Wisdom = _random.Next(3, 21),
                Charisma = _random.Next(3, 21),
                Description = await GetRandomItem(descriptions)
            };

            return npc;
        }

        public async Task<Monster> GenerateRandomMonster() {
            var monsterSpecies = await _dataService.GetMonsterSpeciesAsync();
            var monsterTypes = await _dataService.GetMonsterTypesAsync();
            var monsterAbilities = await _dataService.GetMonsterAbilitiesAsync();
            var descriptions = await _dataService.GetDescriptionsAsync();

            if(!monsterSpecies.Any() || !monsterTypes.Any() || !monsterAbilities.Any() || !descriptions.Any()) {
                _logger.LogWarning("One of the monster data pools is empty");
                throw new GenerationFailedException("Insufficient data to generate Monster");
            }

            var monster = new Monster {
                Id = GenerateUniqueId(),
                Species = await GetRandomItem(monsterSpecies),
                Type = await GetRandomItem(monsterTypes),
                Level = _random.Next(3, 21),
                Health = _random.Next(10, 501),
                ArmorClass = _random.Next(10, 21),
                Damage = _random.Next(1, 51),
                Abilities = await GetRandomItem(monsterAbilities),
                Description = await GetRandomItem(descriptions)
            };
            return monster;
        }

        public async Task<RandomEncounter> GenerateRandomEncounter() {
            var encounterNames = await _dataService.GetEncounterNamesAsync();
            var descriptions   = await _dataService.GetDescriptionsAsync();
            var locations      = await _dataService.GetLocationsAsync();
            var weathers       = await _dataService.GetWeathersAsync();
            var timesOfDay     = await _dataService.GetTimesOfDayAsync();
            var terrains       = await _dataService.GetTerrainsAsync();
            var difficulties   = await _dataService.GetDifficultiesAsync();
            var rewards        = await _dataService.GetRewardsAsync();
            var notes          = await _dataService.GetNotesAsync();
            
            if(!encounterNames.Any() || !descriptions.Any() || !locations.Any() || !weathers.Any() || !timesOfDay.Any() || !terrains.Any() || !difficulties.Any() || !rewards.Any() || !notes.Any()) {
                _logger.LogWarning("One of the encounter data pools is empty");
                throw new GenerationFailedException("Insufficient data to generate Encounter");
            }

            var encounter = new RandomEncounter {
                Id = GenerateUniqueId(),
                Name = await GetRandomItem(encounterNames),
                Description = await GetRandomItem(descriptions),
                Location = await GetRandomItem(locations),
                Weather = await GetRandomItem(weathers),
                TimeOfDay = await GetRandomItem(timesOfDay),
                Terrain = await GetRandomItem(terrains),
                Difficulty = await GetRandomItem(difficulties),
                Reward = await GetRandomItem(rewards),
                Notes = await GetRandomItem(notes),
                InvolvedNPCs = new List<NPC> { await GenerateRandomNPC(), await GenerateRandomNPC() },
                InvolvedMonsters = new List<Monster> { await GenerateRandomMonster(), await GenerateRandomMonster() }
            };

            return encounter;
        }

        public async Task<List<NPC>> GenerateRandomNPCs(int count) {
            var npcs = new List<NPC>();

            for(int i = 0; i < count; i++) {
                var npc = await GenerateRandomNPC();
                if(npc != null) {
                    npcs.Add(npc);
                }
            }
            return npcs;
        }

        public async Task<List<Monster>> GenerateRandomMonsters(int count) {
            var monsters = new List<Monster>();

            for(int i = 0; i < count; i++) {
                var monster = await GenerateRandomMonster();
                if(monster != null) {
                    monsters.Add(monster);
                }
            }
            return monsters;
        }

        private async Task<T> GetRandomItem<T>(List<T> list) {
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