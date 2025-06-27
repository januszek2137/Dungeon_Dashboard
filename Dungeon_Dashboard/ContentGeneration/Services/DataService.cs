using System.Text.Json;

namespace Dungeon_Dashboard.ContentGeneration.Services {

    public interface IDataService {

        Task<List<string>> GetNPCNamesAsync();

        Task<List<string>> GetNPCRolesAsync();

        Task<List<string>> GetMonsterSpeciesAsync();

        Task<List<string>> GetMonsterTypesAsync();

        Task<List<string>> GetMonsterAbilitiesAsync();

        Task<List<string>> GetDescriptionsAsync();

        Task<List<string>> GetEncounterNamesAsync();

        Task<List<string>> GetLocationsAsync();

        Task<List<string>> GetWeathersAsync();

        Task<List<string>> GetTimesOfDayAsync();

        Task<List<string>> GetTerrainsAsync();

        Task<List<string>> GetDifficultiesAsync();

        Task<List<string>> GetRewardsAsync();

        Task<List<string>> GetNotesAsync();
    }

    public class DataService : IDataService {
        private readonly string _dataFolderPath;
        private readonly ILogger<DataService> _logger;

        private readonly Dictionary<string, List<string>> _dataPools;

        //loads all data pools from json files in the data folder
        public DataService(IHostEnvironment environment, ILogger<DataService> logger) {
            _logger = logger;
            _dataFolderPath = Path.Combine(environment.ContentRootPath, "wwwroot", "data");
            _dataPools = new Dictionary<string, List<string>>();
            LoadAllDataPools().Wait();
        }

        //loads all data pools from json files in the data folder
        private async Task LoadAllDataPools() {
            var files = Directory.GetFiles(_dataFolderPath, "*.json");

            foreach(var file in files) {
                var key = Path.GetFileNameWithoutExtension(file);

                try {
                    var json = await File.ReadAllTextAsync(file);
                    var list = JsonSerializer.Deserialize<List<string>>(json, new JsonSerializerOptions {
                        PropertyNameCaseInsensitive = true
                    });

                    if(list != null) {
                        _dataPools[key] = list;
                        _logger.LogInformation($"Loaded data pool: {key} from file {file}");
                    }
                } catch(Exception ex) {
                    _logger.LogError(ex, $"Failed to load data pool: {key} from file {file}");
                }
            }
        }

        public Task<List<string>> GetNPCNamesAsync() => GetDataPoolAsync("npcNames");

        public Task<List<string>> GetNPCRolesAsync() => GetDataPoolAsync("npcRoles");

        public Task<List<string>> GetMonsterSpeciesAsync() => GetDataPoolAsync("monsterSpecies");

        public Task<List<string>> GetMonsterTypesAsync() => GetDataPoolAsync("monsterTypes");

        public Task<List<string>> GetMonsterAbilitiesAsync() => GetDataPoolAsync("monsterAbilities");

        public Task<List<string>> GetDescriptionsAsync() => GetDataPoolAsync("descriptions");

        public Task<List<string>> GetEncounterNamesAsync() => GetDataPoolAsync("encounterNames");

        public Task<List<string>> GetLocationsAsync() => GetDataPoolAsync("locations");

        public Task<List<string>> GetWeathersAsync() => GetDataPoolAsync("weathers");

        public Task<List<string>> GetTimesOfDayAsync() => GetDataPoolAsync("timesOfDay");

        public Task<List<string>> GetTerrainsAsync() => GetDataPoolAsync("terrains");

        public Task<List<string>> GetDifficultiesAsync() => GetDataPoolAsync("difficulties");

        public Task<List<string>> GetRewardsAsync() => GetDataPoolAsync("rewards");

        public Task<List<string>> GetNotesAsync() => GetDataPoolAsync("notes");

        //returns the data pool for the given key
        private Task<List<string>> GetDataPoolAsync(string key) {
            if(_dataPools.ContainsKey(key)) {
                return Task.FromResult(_dataPools[key]);
            } else {
                return Task.FromResult(new List<string>());
            }
        }
    }
}