using Dungeon_Dashboard.ContentGeneration.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dungeon_DashboardTests.ContentGeneration.Services;

[TestClass]
public class ContentGenerationServiceTest {
    private Mock<IDataService>                      _mockDataService;
    private Mock<ILogger<ContentGenerationService>> _mockLogger;
    private ContentGenerationService                _contentGenerationService;

    [TestInitialize]
    public void Setup() {
        _mockDataService          = new Mock<IDataService>();
        _mockLogger               = new Mock<ILogger<ContentGenerationService>>();
        _contentGenerationService = new ContentGenerationService(_mockDataService.Object, _mockLogger.Object);
    }
    
    #region GenerateRandomNPC Tests
    [TestMethod]
    public async Task GenerateRandomNPC_WithValidData_ReturnsNPC() {
        // Arrange
        var npcNames     = new List<string> { "Thorin", "Gandalf", "Aragorn" };
        var npcRoles     = new List<string> { "Warrior", "Wizard", "Ranger" };
        var descriptions = new List<string> { "Brave hero", "Wise sage", "Skilled tracker" };

        _mockDataService.Setup(x => x.GetNPCNamesAsync()).ReturnsAsync(npcNames);
        _mockDataService.Setup(x => x.GetNPCRolesAsync()).ReturnsAsync(npcRoles);
        _mockDataService.Setup(x => x.GetDescriptionsAsync()).ReturnsAsync(descriptions);

        // Act
        var result = await _contentGenerationService.GenerateRandomNPC();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Id);
        Assert.IsTrue(npcNames.Contains(result.Name));
        Assert.IsTrue(npcRoles.Contains(result.Role));
        Assert.IsTrue(descriptions.Contains(result.Description));
    }

    [TestMethod]
    public async Task GenerateRandomNPC_WithValidData_SetsPropertiesInValidRange() {
        // Arrange
        var npcNames     = new List<string> { "Thorin" };
        var npcRoles     = new List<string> { "Warrior" };
        var descriptions = new List<string> { "Brave hero" };

        _mockDataService.Setup(x => x.GetNPCNamesAsync()).ReturnsAsync(npcNames);
        _mockDataService.Setup(x => x.GetNPCRolesAsync()).ReturnsAsync(npcRoles);
        _mockDataService.Setup(x => x.GetDescriptionsAsync()).ReturnsAsync(descriptions);

        // Act
        var result = await _contentGenerationService.GenerateRandomNPC();

        // Assert
        Assert.IsTrue(result.Level        >= 3  && result.Level        <= 20);
        Assert.IsTrue(result.Health       >= 10 && result.Health       <= 250);
        Assert.IsTrue(result.ArmorClass   >= 10 && result.ArmorClass   <= 20);
        Assert.IsTrue(result.Strength     >= 3  && result.Strength     <= 20);
        Assert.IsTrue(result.Dexterity    >= 3  && result.Dexterity    <= 20);
        Assert.IsTrue(result.Constitution >= 3  && result.Constitution <= 20);
        Assert.IsTrue(result.Intelligence >= 3  && result.Intelligence <= 20);
        Assert.IsTrue(result.Wisdom       >= 3  && result.Wisdom       <= 20);
        Assert.IsTrue(result.Charisma     >= 3  && result.Charisma     <= 20);
    }

    [TestMethod]
    [ExpectedException(typeof(GenerationFailedException))]
    public async Task GenerateRandomNPC_WithEmptyNames_ThrowsException() {
        // Arrange
        var emptyNames   = new List<string>();
        var npcRoles     = new List<string> { "Warrior" };
        var descriptions = new List<string> { "Brave hero" };

        _mockDataService.Setup(x => x.GetNPCNamesAsync()).ReturnsAsync(emptyNames);
        _mockDataService.Setup(x => x.GetNPCRolesAsync()).ReturnsAsync(npcRoles);
        _mockDataService.Setup(x => x.GetDescriptionsAsync()).ReturnsAsync(descriptions);

        // Act
        await _contentGenerationService.GenerateRandomNPC();

        // Assert - ExpectedException handles this
    }

    [TestMethod]
    [ExpectedException(typeof(GenerationFailedException))]
    public async Task GenerateRandomNPC_WithEmptyRoles_ThrowsException() {
        // Arrange
        var npcNames     = new List<string> { "Thorin" };
        var emptyRoles   = new List<string>();
        var descriptions = new List<string> { "Brave hero" };

        _mockDataService.Setup(x => x.GetNPCNamesAsync()).ReturnsAsync(npcNames);
        _mockDataService.Setup(x => x.GetNPCRolesAsync()).ReturnsAsync(emptyRoles);
        _mockDataService.Setup(x => x.GetDescriptionsAsync()).ReturnsAsync(descriptions);

        // Act
        await _contentGenerationService.GenerateRandomNPC();

        // Assert - ExpectedException handles this
    }

    [TestMethod]
    [ExpectedException(typeof(GenerationFailedException))]
    public async Task GenerateRandomNPC_WithEmptyDescriptions_ThrowsException() {
        // Arrange
        var npcNames          = new List<string> { "Thorin" };
        var npcRoles          = new List<string> { "Warrior" };
        var emptyDescriptions = new List<string>();

        _mockDataService.Setup(x => x.GetNPCNamesAsync()).ReturnsAsync(npcNames);
        _mockDataService.Setup(x => x.GetNPCRolesAsync()).ReturnsAsync(npcRoles);
        _mockDataService.Setup(x => x.GetDescriptionsAsync()).ReturnsAsync(emptyDescriptions);

        // Act
        await _contentGenerationService.GenerateRandomNPC();

        // Assert - ExpectedException handles this
    }

    [TestMethod]
    public async Task GenerateRandomNPC_WithEmptyData_LogsWarning() {
        // Arrange
        var emptyNames   = new List<string>();
        var npcRoles     = new List<string> { "Warrior" };
        var descriptions = new List<string> { "Brave hero" };

        _mockDataService.Setup(x => x.GetNPCNamesAsync()).ReturnsAsync(emptyNames);
        _mockDataService.Setup(x => x.GetNPCRolesAsync()).ReturnsAsync(npcRoles);
        _mockDataService.Setup(x => x.GetDescriptionsAsync()).ReturnsAsync(descriptions);

        // Act & Assert
        try {
            await _contentGenerationService.GenerateRandomNPC();
        }
        catch (GenerationFailedException) {
            // Verify logger was called with Warning level
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("npc data pools is empty")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }

    [TestMethod]
    public async Task GenerateRandomNPC_CallsDataServiceMethodsOnce() {
        // Arrange
        var npcNames     = new List<string> { "Thorin" };
        var npcRoles     = new List<string> { "Warrior" };
        var descriptions = new List<string> { "Brave hero" };

        _mockDataService.Setup(x => x.GetNPCNamesAsync()).ReturnsAsync(npcNames);
        _mockDataService.Setup(x => x.GetNPCRolesAsync()).ReturnsAsync(npcRoles);
        _mockDataService.Setup(x => x.GetDescriptionsAsync()).ReturnsAsync(descriptions);

        // Act
        await _contentGenerationService.GenerateRandomNPC();

        // Assert
        _mockDataService.Verify(x => x.GetNPCNamesAsync(), Times.Once);
        _mockDataService.Verify(x => x.GetNPCRolesAsync(), Times.Once);
        _mockDataService.Verify(x => x.GetDescriptionsAsync(), Times.Once);
    }

    [TestMethod]
    public async Task GenerateRandomNPC_GeneratesUniqueIds() {
        // Arrange
        var npcNames     = new List<string> { "Thorin" };
        var npcRoles     = new List<string> { "Warrior" };
        var descriptions = new List<string> { "Brave hero" };

        _mockDataService.Setup(x => x.GetNPCNamesAsync()).ReturnsAsync(npcNames);
        _mockDataService.Setup(x => x.GetNPCRolesAsync()).ReturnsAsync(npcRoles);
        _mockDataService.Setup(x => x.GetDescriptionsAsync()).ReturnsAsync(descriptions);

        // Act
        var npc1 = await _contentGenerationService.GenerateRandomNPC();
        var npc2 = await _contentGenerationService.GenerateRandomNPC();

        // Assert
        Assert.AreNotEqual(npc1.Id, npc2.Id);
    }
    #endregion
    
    #region GenerateRandomMonster Tests

    [TestMethod]
    public async Task GenerateRandomMonster_WithValidData_ReturnsMonster() {
        // Arrange
        _mockDataService.Setup(x => x.GetMonsterSpeciesAsync())
            .ReturnsAsync(new List<string> { "Dragon", "Goblin", "Orc" });
        _mockDataService.Setup(x => x.GetMonsterTypesAsync())
            .ReturnsAsync(new List<string> { "Beast", "Undead", "Fiend" });
        _mockDataService.Setup(x => x.GetMonsterAbilitiesAsync())
            .ReturnsAsync(new List<string> { "Fire Breath", "Fly", "Invisibility" });
        _mockDataService.Setup(x => x.GetDescriptionsAsync())
            .ReturnsAsync(new List<string> { "Description 1", "Description 2" });

        // Act
        var result = await _contentGenerationService.GenerateRandomMonster();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Species);
        Assert.IsNotNull(result.Type);
        Assert.IsNotNull(result.Abilities);
        Assert.IsNotNull(result.Description);
        Assert.IsTrue(result.Level      >= 3  && result.Level      < 21);
        Assert.IsTrue(result.Health     >= 10 && result.Health     < 501);
        Assert.IsTrue(result.ArmorClass >= 10 && result.ArmorClass < 21);
        Assert.IsTrue(result.Damage     >= 1  && result.Damage     < 51);
    }

    [TestMethod]
    public async Task GenerateRandomMonster_EmptySpeciesList_ThrowsGenerationFailedException() {
        // Arrange
        _mockDataService.Setup(x => x.GetMonsterSpeciesAsync())
            .ReturnsAsync(new List<string>());
        _mockDataService.Setup(x => x.GetMonsterTypesAsync())
            .ReturnsAsync(new List<string> { "Beast" });
        _mockDataService.Setup(x => x.GetMonsterAbilitiesAsync())
            .ReturnsAsync(new List<string> { "Fire Breath" });
        _mockDataService.Setup(x => x.GetDescriptionsAsync())
            .ReturnsAsync(new List<string> { "Description" });

        // Act & Assert
        await Assert.ThrowsExceptionAsync<GenerationFailedException>(async () =>
            await _contentGenerationService.GenerateRandomMonster()
        );
    }

    [TestMethod]
    public async Task GenerateRandomMonster_EmptyTypesList_ThrowsGenerationFailedException() {
        // Arrange
        _mockDataService.Setup(x => x.GetMonsterSpeciesAsync())
            .ReturnsAsync(new List<string> { "Dragon" });
        _mockDataService.Setup(x => x.GetMonsterTypesAsync())
            .ReturnsAsync(new List<string>());
        _mockDataService.Setup(x => x.GetMonsterAbilitiesAsync())
            .ReturnsAsync(new List<string> { "Fire Breath" });
        _mockDataService.Setup(x => x.GetDescriptionsAsync())
            .ReturnsAsync(new List<string> { "Description" });

        // Act & Assert
        await Assert.ThrowsExceptionAsync<GenerationFailedException>(async () =>
            await _contentGenerationService.GenerateRandomMonster()
        );
    }

    [TestMethod]
    public async Task GenerateRandomMonster_EmptyAbilitiesList_ThrowsGenerationFailedException() {
        // Arrange
        _mockDataService.Setup(x => x.GetMonsterSpeciesAsync())
            .ReturnsAsync(new List<string> { "Dragon" });
        _mockDataService.Setup(x => x.GetMonsterTypesAsync())
            .ReturnsAsync(new List<string> { "Beast" });
        _mockDataService.Setup(x => x.GetMonsterAbilitiesAsync())
            .ReturnsAsync(new List<string>());
        _mockDataService.Setup(x => x.GetDescriptionsAsync())
            .ReturnsAsync(new List<string> { "Description" });

        // Act & Assert
        await Assert.ThrowsExceptionAsync<GenerationFailedException>(async () =>
            await _contentGenerationService.GenerateRandomMonster()
        );
    }

    [TestMethod]
    public async Task GenerateRandomMonster_EmptyDescriptionsList_ThrowsGenerationFailedException() {
        // Arrange
        _mockDataService.Setup(x => x.GetMonsterSpeciesAsync())
            .ReturnsAsync(new List<string> { "Dragon" });
        _mockDataService.Setup(x => x.GetMonsterTypesAsync())
            .ReturnsAsync(new List<string> { "Beast" });
        _mockDataService.Setup(x => x.GetMonsterAbilitiesAsync())
            .ReturnsAsync(new List<string> { "Fire Breath" });
        _mockDataService.Setup(x => x.GetDescriptionsAsync())
            .ReturnsAsync(new List<string>());

        // Act & Assert
        await Assert.ThrowsExceptionAsync<GenerationFailedException>(async () =>
            await _contentGenerationService.GenerateRandomMonster()
        );
    }

    [TestMethod]
    public async Task GenerateRandomMonster_LogsWarning_WhenDataPoolIsEmpty() {
        // Arrange
        _mockDataService.Setup(x => x.GetMonsterSpeciesAsync())
            .ReturnsAsync(new List<string>());
        _mockDataService.Setup(x => x.GetMonsterTypesAsync())
            .ReturnsAsync(new List<string> { "Beast" });
        _mockDataService.Setup(x => x.GetMonsterAbilitiesAsync())
            .ReturnsAsync(new List<string> { "Fire Breath" });
        _mockDataService.Setup(x => x.GetDescriptionsAsync())
            .ReturnsAsync(new List<string> { "Description" });

        // Act
        try {
            await _contentGenerationService.GenerateRandomMonster();
        }
        catch (GenerationFailedException) {
            // Expected exception
        }

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("One of the monster data pools is empty")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [TestMethod]
    public async Task GenerateRandomMonster_GeneratesUniqueId() {
        // Arrange
        _mockDataService.Setup(x => x.GetMonsterSpeciesAsync())
            .ReturnsAsync(new List<string> { "Dragon" });
        _mockDataService.Setup(x => x.GetMonsterTypesAsync())
            .ReturnsAsync(new List<string> { "Beast" });
        _mockDataService.Setup(x => x.GetMonsterAbilitiesAsync())
            .ReturnsAsync(new List<string> { "Fire Breath" });
        _mockDataService.Setup(x => x.GetDescriptionsAsync())
            .ReturnsAsync(new List<string> { "Description" });

        // Act
        var result1 = await _contentGenerationService.GenerateRandomMonster();
        var result2 = await _contentGenerationService.GenerateRandomMonster();

        // Assert
        Assert.AreNotEqual(result1.Id, result2.Id);
    }

    #endregion

    #region GenerateRandomEncounter Tests

    [TestMethod]
    public async Task GenerateRandomEncounter_WithValidData_ReturnsEncounter() {
        // Arrange
        SetupValidDataForEncounter();

        // Act
        var result = await _contentGenerationService.GenerateRandomEncounter();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Name);
        Assert.IsNotNull(result.Description);
        Assert.IsNotNull(result.Location);
        Assert.IsNotNull(result.Weather);
        Assert.IsNotNull(result.TimeOfDay);
        Assert.IsNotNull(result.Terrain);
        Assert.IsNotNull(result.Difficulty);
        Assert.IsNotNull(result.Reward);
        Assert.IsNotNull(result.Notes);
        Assert.IsNotNull(result.InvolvedNPCs);
        Assert.AreEqual(2, result.InvolvedNPCs.Count);
        Assert.IsNotNull(result.InvolvedMonsters);
        Assert.AreEqual(2, result.InvolvedMonsters.Count);
    }

    [TestMethod]
    public async Task GenerateRandomEncounter_EmptyEncounterNamesList_ThrowsGenerationFailedException() {
        // Arrange
        SetupValidDataForEncounter();
        _mockDataService.Setup(x => x.GetEncounterNamesAsync())
            .ReturnsAsync(new List<string>());

        // Act & Assert
        await Assert.ThrowsExceptionAsync<GenerationFailedException>(async () =>
            await _contentGenerationService.GenerateRandomEncounter()
        );
    }

    [TestMethod]
    public async Task GenerateRandomEncounter_EmptyLocationsList_ThrowsGenerationFailedException() {
        // Arrange
        SetupValidDataForEncounter();
        _mockDataService.Setup(x => x.GetLocationsAsync())
            .ReturnsAsync(new List<string>());

        // Act & Assert
        await Assert.ThrowsExceptionAsync<GenerationFailedException>(async () =>
            await _contentGenerationService.GenerateRandomEncounter()
        );
    }

    [TestMethod]
    public async Task GenerateRandomEncounter_EmptyWeathersList_ThrowsGenerationFailedException() {
        // Arrange
        SetupValidDataForEncounter();
        _mockDataService.Setup(x => x.GetWeathersAsync())
            .ReturnsAsync(new List<string>());

        // Act & Assert
        await Assert.ThrowsExceptionAsync<GenerationFailedException>(async () =>
            await _contentGenerationService.GenerateRandomEncounter()
        );
    }

    [TestMethod]
    public async Task GenerateRandomEncounter_EmptyTimesOfDayList_ThrowsGenerationFailedException() {
        // Arrange
        SetupValidDataForEncounter();
        _mockDataService.Setup(x => x.GetTimesOfDayAsync())
            .ReturnsAsync(new List<string>());

        // Act & Assert
        await Assert.ThrowsExceptionAsync<GenerationFailedException>(async () =>
            await _contentGenerationService.GenerateRandomEncounter()
        );
    }

    [TestMethod]
    public async Task GenerateRandomEncounter_EmptyTerrainsList_ThrowsGenerationFailedException() {
        // Arrange
        SetupValidDataForEncounter();
        _mockDataService.Setup(x => x.GetTerrainsAsync())
            .ReturnsAsync(new List<string>());

        // Act & Assert
        await Assert.ThrowsExceptionAsync<GenerationFailedException>(async () =>
            await _contentGenerationService.GenerateRandomEncounter()
        );
    }

    [TestMethod]
    public async Task GenerateRandomEncounter_EmptyDifficultiesList_ThrowsGenerationFailedException() {
        // Arrange
        SetupValidDataForEncounter();
        _mockDataService.Setup(x => x.GetDifficultiesAsync())
            .ReturnsAsync(new List<string>());

        // Act & Assert
        await Assert.ThrowsExceptionAsync<GenerationFailedException>(async () =>
            await _contentGenerationService.GenerateRandomEncounter()
        );
    }

    [TestMethod]
    public async Task GenerateRandomEncounter_EmptyRewardsList_ThrowsGenerationFailedException() {
        // Arrange
        SetupValidDataForEncounter();
        _mockDataService.Setup(x => x.GetRewardsAsync())
            .ReturnsAsync(new List<string>());

        // Act & Assert
        await Assert.ThrowsExceptionAsync<GenerationFailedException>(async () =>
            await _contentGenerationService.GenerateRandomEncounter()
        );
    }

    [TestMethod]
    public async Task GenerateRandomEncounter_EmptyNotesList_ThrowsGenerationFailedException() {
        // Arrange
        SetupValidDataForEncounter();
        _mockDataService.Setup(x => x.GetNotesAsync())
            .ReturnsAsync(new List<string>());

        // Act & Assert
        await Assert.ThrowsExceptionAsync<GenerationFailedException>(async () =>
            await _contentGenerationService.GenerateRandomEncounter()
        );
    }

    [TestMethod]
    public async Task GenerateRandomEncounter_LogsWarning_WhenDataPoolIsEmpty() {
        // Arrange
        SetupValidDataForEncounter();
        _mockDataService.Setup(x => x.GetEncounterNamesAsync())
            .ReturnsAsync(new List<string>());

        // Act
        try {
            await _contentGenerationService.GenerateRandomEncounter();
        }
        catch (GenerationFailedException) {
            // Expected exception
        }

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("One of the encounter data pools is empty")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    #endregion

    #region GenerateRandomMonsters Tests

    [TestMethod]
    public async Task GenerateRandomMonsters_WithValidCount_ReturnsCorrectNumberOfMonsters() {
        // Arrange
        SetupValidDataForMonster();
        int count = 5;

        // Act
        var result = await _contentGenerationService.GenerateRandomMonsters(count);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(count, result.Count);
    }

    [TestMethod]
    public async Task GenerateRandomMonsters_WithZeroCount_ReturnsEmptyList() {
        // Arrange
        SetupValidDataForMonster();

        // Act
        var result = await _contentGenerationService.GenerateRandomMonsters(0);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task GenerateRandomMonsters_GeneratesUniqueMonsters() {
        // Arrange
        SetupValidDataForMonster();

        // Act
        var result = await _contentGenerationService.GenerateRandomMonsters(3);

        // Assert
        Assert.AreEqual(3, result.Count);
        Assert.AreNotEqual(result[0].Id, result[1].Id);
        Assert.AreNotEqual(result[1].Id, result[2].Id);
        Assert.AreNotEqual(result[0].Id, result[2].Id);
    }

    [TestMethod]
    public async Task GenerateRandomMonsters_WithLargeCount_ReturnsCorrectNumber() {
        // Arrange
        SetupValidDataForMonster();
        int count = 100;

        // Act
        var result = await _contentGenerationService.GenerateRandomMonsters(count);

        // Assert
        Assert.AreEqual(count, result.Count);
    }

    #endregion

    #region Helper Methods

    private void SetupValidDataForMonster() {
        _mockDataService.Setup(x => x.GetMonsterSpeciesAsync())
            .ReturnsAsync(new List<string> { "Dragon", "Goblin", "Orc" });
        _mockDataService.Setup(x => x.GetMonsterTypesAsync())
            .ReturnsAsync(new List<string> { "Beast", "Undead", "Fiend" });
        _mockDataService.Setup(x => x.GetMonsterAbilitiesAsync())
            .ReturnsAsync(new List<string> { "Fire Breath", "Fly", "Invisibility" });
        _mockDataService.Setup(x => x.GetDescriptionsAsync())
            .ReturnsAsync(new List<string> { "Description 1", "Description 2" });
    }

    private void SetupValidDataForNPC() {
        _mockDataService.Setup(x => x.GetNPCNamesAsync())
            .ReturnsAsync(new List<string> { "Gandalf", "Aragorn", "Legolas" });
        _mockDataService.Setup(x => x.GetNPCRolesAsync())
            .ReturnsAsync(new List<string> { "Warrior", "Mage", "Rogue" });
        _mockDataService.Setup(x => x.GetDescriptionsAsync())
            .ReturnsAsync(new List<string> { "Description 1", "Description 2" });
    }

    private void SetupValidDataForEncounter() {
        SetupValidDataForNPC();
        SetupValidDataForMonster();

        _mockDataService.Setup(x => x.GetEncounterNamesAsync())
            .ReturnsAsync(new List<string> { "Ambush at Dawn", "The Dragon's Lair" });
        _mockDataService.Setup(x => x.GetLocationsAsync())
            .ReturnsAsync(new List<string> { "Forest", "Cave", "Castle" });
        _mockDataService.Setup(x => x.GetWeathersAsync())
            .ReturnsAsync(new List<string> { "Sunny", "Rainy", "Stormy" });
        _mockDataService.Setup(x => x.GetTimesOfDayAsync())
            .ReturnsAsync(new List<string> { "Dawn", "Noon", "Dusk" });
        _mockDataService.Setup(x => x.GetTerrainsAsync())
            .ReturnsAsync(new List<string> { "Mountain", "Plains", "Swamp" });
        _mockDataService.Setup(x => x.GetDifficultiesAsync())
            .ReturnsAsync(new List<string> { "Easy", "Medium", "Hard" });
        _mockDataService.Setup(x => x.GetRewardsAsync())
            .ReturnsAsync(new List<string> { "Gold coins", "Magic sword" });
        _mockDataService.Setup(x => x.GetNotesAsync())
            .ReturnsAsync(new List<string> { "Note 1", "Note 2" });
    }

    #endregion

}
