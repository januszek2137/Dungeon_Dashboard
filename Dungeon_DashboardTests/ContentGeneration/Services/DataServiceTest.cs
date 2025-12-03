using System.Text.Json;
using Dungeon_Dashboard.ContentGeneration.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dungeon_DashboardTests.ContentGeneration.Services {
    
    [TestClass]
    public class DataServiceTests {
        private string _testDataPath;
        private Mock<IHostEnvironment> _mockEnvironment;
        private Mock<ILogger<DataService>> _mockLogger;

        [TestInitialize]
        public void Setup() {
            // Create a temporary directory for test data
            _testDataPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDataPath);

            // Setup mock environment
            _mockEnvironment = new Mock<IHostEnvironment>();
            _mockEnvironment.Setup(e => e.ContentRootPath).Returns(_testDataPath);

            // Setup mock logger
            _mockLogger = new Mock<ILogger<DataService>>();
        }

        [TestCleanup]
        public void Cleanup() {
            // Clean up test directory
            if(Directory.Exists(_testDataPath)) {
                Directory.Delete(_testDataPath, true);
            }
        }

        private void CreateTestJsonFile(string fileName, List<string> data) {
            var filePath = Path.Combine(_testDataPath, "wwwroot", "data", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            
            var json = JsonSerializer.Serialize(data);
            File.WriteAllText(filePath, json);
        }

        [TestMethod]
        public async Task Constructor_LoadsAllJsonFiles_Successfully() {
            // Arrange
            var testData = new List<string> { "Item1", "Item2", "Item3" };
            CreateTestJsonFile("npcNames.json", testData);
            CreateTestJsonFile("npcRoles.json", testData);

            // Act
            var service = new DataService(_mockEnvironment.Object, _mockLogger.Object);
            var result = await service.GetNPCNamesAsync();

            // Assert
            Assert.AreEqual(testData.Count, result.Count);
            CollectionAssert.AreEqual(testData, result);
        }

        [TestMethod]
        public async Task GetNPCNamesAsync_ReturnsCorrectData() {
            // Arrange
            var expectedNames = new List<string> { "Gandalf", "Aragorn", "Legolas" };
            CreateTestJsonFile("npcNames.json", expectedNames);

            var service = new DataService(_mockEnvironment.Object, _mockLogger.Object);

            // Act
            var result = await service.GetNPCNamesAsync();

            // Assert
            CollectionAssert.AreEqual(expectedNames, result);
        }

        [TestMethod]
        public async Task GetNPCRolesAsync_ReturnsCorrectData() {
            // Arrange
            var expectedRoles = new List<string> { "Warrior", "Mage", "Rogue" };
            CreateTestJsonFile("npcRoles.json", expectedRoles);

            var service = new DataService(_mockEnvironment.Object, _mockLogger.Object);

            // Act
            var result = await service.GetNPCRolesAsync();

            // Assert
            CollectionAssert.AreEqual(expectedRoles, result);
        }

        [TestMethod]
        public async Task GetMonsterSpeciesAsync_ReturnsCorrectData() {
            // Arrange
            var expectedSpecies = new List<string> { "Dragon", "Goblin", "Orc" };
            CreateTestJsonFile("monsterSpecies.json", expectedSpecies);

            var service = new DataService(_mockEnvironment.Object, _mockLogger.Object);

            // Act
            var result = await service.GetMonsterSpeciesAsync();

            // Assert
            CollectionAssert.AreEqual(expectedSpecies, result);
        }

        [TestMethod]
        public async Task GetMonsterTypesAsync_ReturnsCorrectData() {
            // Arrange
            var expectedTypes = new List<string> { "Beast", "Undead", "Fiend" };
            CreateTestJsonFile("monsterTypes.json", expectedTypes);

            var service = new DataService(_mockEnvironment.Object, _mockLogger.Object);

            // Act
            var result = await service.GetMonsterTypesAsync();

            // Assert
            CollectionAssert.AreEqual(expectedTypes, result);
        }

        [TestMethod]
        public async Task GetMonsterAbilitiesAsync_ReturnsCorrectData() {
            // Arrange
            var expectedAbilities = new List<string> { "Fire Breath", "Fly", "Invisibility" };
            CreateTestJsonFile("monsterAbilities.json", expectedAbilities);

            var service = new DataService(_mockEnvironment.Object, _mockLogger.Object);

            // Act
            var result = await service.GetMonsterAbilitiesAsync();

            // Assert
            CollectionAssert.AreEqual(expectedAbilities, result);
        }

        [TestMethod]
        public async Task GetDescriptionsAsync_ReturnsCorrectData() {
            // Arrange
            var expectedDescriptions = new List<string> { "Dark and gloomy", "Bright and cheerful" };
            CreateTestJsonFile("descriptions.json", expectedDescriptions);

            var service = new DataService(_mockEnvironment.Object, _mockLogger.Object);

            // Act
            var result = await service.GetDescriptionsAsync();

            // Assert
            CollectionAssert.AreEqual(expectedDescriptions, result);
        }

        [TestMethod]
        public async Task GetEncounterNamesAsync_ReturnsCorrectData() {
            // Arrange
            var expectedNames = new List<string> { "Ambush at Dawn", "The Dragon's Lair" };
            CreateTestJsonFile("encounterNames.json", expectedNames);

            var service = new DataService(_mockEnvironment.Object, _mockLogger.Object);

            // Act
            var result = await service.GetEncounterNamesAsync();

            // Assert
            CollectionAssert.AreEqual(expectedNames, result);
        }

        [TestMethod]
        public async Task GetLocationsAsync_ReturnsCorrectData() {
            // Arrange
            var expectedLocations = new List<string> { "Forest", "Cave", "Castle" };
            CreateTestJsonFile("locations.json", expectedLocations);

            var service = new DataService(_mockEnvironment.Object, _mockLogger.Object);

            // Act
            var result = await service.GetLocationsAsync();

            // Assert
            CollectionAssert.AreEqual(expectedLocations, result);
        }

        [TestMethod]
        public async Task GetWeathersAsync_ReturnsCorrectData() {
            // Arrange
            var expectedWeathers = new List<string> { "Sunny", "Rainy", "Stormy" };
            CreateTestJsonFile("weathers.json", expectedWeathers);

            var service = new DataService(_mockEnvironment.Object, _mockLogger.Object);

            // Act
            var result = await service.GetWeathersAsync();

            // Assert
            CollectionAssert.AreEqual(expectedWeathers, result);
        }

        [TestMethod]
        public async Task GetTimesOfDayAsync_ReturnsCorrectData() {
            // Arrange
            var expectedTimes = new List<string> { "Dawn", "Noon", "Dusk", "Midnight" };
            CreateTestJsonFile("timesOfDay.json", expectedTimes);

            var service = new DataService(_mockEnvironment.Object, _mockLogger.Object);

            // Act
            var result = await service.GetTimesOfDayAsync();

            // Assert
            CollectionAssert.AreEqual(expectedTimes, result);
        }

        [TestMethod]
        public async Task GetTerrainsAsync_ReturnsCorrectData() {
            // Arrange
            var expectedTerrains = new List<string> { "Mountain", "Plains", "Swamp" };
            CreateTestJsonFile("terrains.json", expectedTerrains);

            var service = new DataService(_mockEnvironment.Object, _mockLogger.Object);

            // Act
            var result = await service.GetTerrainsAsync();

            // Assert
            CollectionAssert.AreEqual(expectedTerrains, result);
        }

        [TestMethod]
        public async Task GetDifficultiesAsync_ReturnsCorrectData() {
            // Arrange
            var expectedDifficulties = new List<string> { "Easy", "Medium", "Hard", "Deadly" };
            CreateTestJsonFile("difficulties.json", expectedDifficulties);

            var service = new DataService(_mockEnvironment.Object, _mockLogger.Object);

            // Act
            var result = await service.GetDifficultiesAsync();

            // Assert
            CollectionAssert.AreEqual(expectedDifficulties, result);
        }

        [TestMethod]
        public async Task GetRewardsAsync_ReturnsCorrectData() {
            // Arrange
            var expectedRewards = new List<string> { "Gold coins", "Magic sword", "Health potion" };
            CreateTestJsonFile("rewards.json", expectedRewards);

            var service = new DataService(_mockEnvironment.Object, _mockLogger.Object);

            // Act
            var result = await service.GetRewardsAsync();

            // Assert
            CollectionAssert.AreEqual(expectedRewards, result);
        }

        [TestMethod]
        public async Task GetNotesAsync_ReturnsCorrectData() {
            // Arrange
            var expectedNotes = new List<string> { "Note 1", "Note 2", "Note 3" };
            CreateTestJsonFile("notes.json", expectedNotes);

            var service = new DataService(_mockEnvironment.Object, _mockLogger.Object);

            // Act
            var result = await service.GetNotesAsync();

            // Assert
            CollectionAssert.AreEqual(expectedNotes, result);
        }
        
        [TestMethod]
        public async Task Constructor_InvalidJsonFile_LogsError() {
            // Arrange
            var filePath = Path.Combine(_testDataPath, "wwwroot", "data", "invalid.json");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            File.WriteAllText(filePath, "{ invalid json }");

            // Act
            var service = new DataService(_mockEnvironment.Object, _mockLogger.Object);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                Times.Once);
        }

        [TestMethod]
        public async Task Constructor_EmptyDataFolder_CreatesServiceSuccessfully() {
            // Arrange
            Directory.CreateDirectory(Path.Combine(_testDataPath, "wwwroot", "data"));

            // Act
            var service = new DataService(_mockEnvironment.Object, _mockLogger.Object);
            var result = await service.GetNPCNamesAsync();

            // Assert
            Assert.IsNotNull(service);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task Constructor_MultipleFiles_LoadsAllSuccessfully() {
            // Arrange
            CreateTestJsonFile("npcNames.json", new List<string> { "Name1", "Name2" });
            CreateTestJsonFile("npcRoles.json", new List<string> { "Role1", "Role2" });
            CreateTestJsonFile("locations.json", new List<string> { "Location1", "Location2" });

            // Act
            var service = new DataService(_mockEnvironment.Object, _mockLogger.Object);
            var names = await service.GetNPCNamesAsync();
            var roles = await service.GetNPCRolesAsync();
            var locations = await service.GetLocationsAsync();

            // Assert
            Assert.AreEqual(2, names.Count);
            Assert.AreEqual(2, roles.Count);
            Assert.AreEqual(2, locations.Count);
        }

        [TestMethod]
        public async Task GetDataPool_EmptyJsonArray_ReturnsEmptyList() {
            // Arrange
            CreateTestJsonFile("empty.json", new List<string>());
            CreateTestJsonFile("npcNames.json", new List<string>());

            var service = new DataService(_mockEnvironment.Object, _mockLogger.Object);

            // Act
            var result = await service.GetNPCNamesAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
    }
}