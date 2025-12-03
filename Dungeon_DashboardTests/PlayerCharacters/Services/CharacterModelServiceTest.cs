using Dungeon_Dashboard.Home.Data;
using Dungeon_Dashboard.PlayerCharacters.Models;
using Dungeon_Dashboard.PlayerCharacters.Services;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dungeon_DashboardTests.PlayerCharacters.Services;

[TestClass]
[TestSubject(typeof(CharacterModelService))]
public class CharacterModelServiceTest {

    private AppDBContext                         _context;
    private Mock<ILogger<CharacterModelService>> _mockLogger;
    private CharacterModelService                _characterModelService;

    [TestInitialize]
    public void Setup() {
        var options = new DbContextOptionsBuilder<AppDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context               = new AppDBContext(options);
        _mockLogger            = new Mock<ILogger<CharacterModelService>>();
        _characterModelService = new CharacterModelService(_context, _mockLogger.Object);
    }

    [TestCleanup]
    public void Cleanup() {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    #region GetAllCharactersForUserAsync Tests

    [TestMethod]
    public async Task GetAllCharactersForUserAsync_WithNoCharacters_ReturnsEmptyList() {
        // Act
        var result = await _characterModelService.GetAllCharactersForUserAsync("user1");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task GetAllCharactersForUserAsync_WithMultipleCharacters_ReturnsOnlyUserCharacters() {
        // Arrange
        var characters = new List<CharacterModel> {
            CreateTestCharacter("Character1", "user1"),
            CreateTestCharacter("Character2", "user1"),
            CreateTestCharacter("Character3", "user2")
        };
        _context.CharacterModel.AddRange(characters);
        await _context.SaveChangesAsync();

        // Act
        var result = await _characterModelService.GetAllCharactersForUserAsync("user1");

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(c => c.CreatedBy == "user1"));
    }

    [TestMethod]
    public async Task GetAllCharactersForUserAsync_WithSingleCharacter_ReturnsSingleCharacter() {
        // Arrange
        var character = CreateTestCharacter("TestCharacter", "user1");
        _context.CharacterModel.Add(character);
        await _context.SaveChangesAsync();

        // Act
        var result = await _characterModelService.GetAllCharactersForUserAsync("user1");

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("TestCharacter", result[0].Name);
    }

    #endregion

    #region GetCharacterByIdIfUserHasAccessAsync Tests

    [TestMethod]
    public async Task GetCharacterByIdIfUserHasAccessAsync_WithValidIdAndUser_ReturnsCharacter() {
        // Arrange
        var character = CreateTestCharacter("TestCharacter", "user1");
        _context.CharacterModel.Add(character);
        await _context.SaveChangesAsync();

        // Act
        var result = await _characterModelService.GetCharacterByIdIfUserHasAccessAsync(character.Id, "user1");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(character.Id, result.Id);
        Assert.AreEqual("TestCharacter", result.Name);
    }

    [TestMethod]
    public async Task GetCharacterByIdIfUserHasAccessAsync_WithNullId_ThrowsArgumentException() {
        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            await _characterModelService.GetCharacterByIdIfUserHasAccessAsync(null, "user1")
        );
    }

    [TestMethod]
    public async Task GetCharacterByIdIfUserHasAccessAsync_WithNonExistingId_ThrowsKeyNotFoundException() {
        // Act & Assert
        await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            await _characterModelService.GetCharacterByIdIfUserHasAccessAsync(999, "user1")
        );
    }

    [TestMethod]
    public async Task GetCharacterByIdIfUserHasAccessAsync_WithWrongUser_ThrowsKeyNotFoundException() {
        // Arrange
        var character = CreateTestCharacter("TestCharacter", "user1");
        _context.CharacterModel.Add(character);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            await _characterModelService.GetCharacterByIdIfUserHasAccessAsync(character.Id, "user2")
        );
    }

    #endregion

    #region AddCharacterAsync Tests

    [TestMethod]
    public async Task AddCharacterAsync_WithValidCharacter_AddsCharacterToDatabase() {
        // Arrange
        var character = CreateTestCharacter("NewCharacter", "user1");

        // Act
        var result = await _characterModelService.AddCharacterAsync(character);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("NewCharacter", result.Name);

        var charactersInDb = await _context.CharacterModel.ToListAsync();
        Assert.AreEqual(1, charactersInDb.Count);
    }

    [TestMethod]
    public async Task AddCharacterAsync_WithValidCharacter_ReturnsAddedCharacter() {
        // Arrange
        var character = CreateTestCharacter("TestCharacter", "user1");

        // Act
        var result = await _characterModelService.AddCharacterAsync(character);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(character.Name, result.Name);
        Assert.AreEqual(character.Class, result.Class);
        Assert.AreEqual(character.Race, result.Race);
    }

    [TestMethod]
    public async Task AddCharacterAsync_WithMultipleCharacters_AddsAllCharacters() {
        // Arrange
        var character1 = CreateTestCharacter("Character1", "user1");
        var character2 = CreateTestCharacter("Character2", "user1");

        // Act
        await _characterModelService.AddCharacterAsync(character1);
        await _characterModelService.AddCharacterAsync(character2);

        // Assert
        var charactersInDb = await _context.CharacterModel.ToListAsync();
        Assert.AreEqual(2, charactersInDb.Count);
    }

    #endregion

    #region UpdateAsync Tests

    [TestMethod]
    public async Task UpdateAsync_WithValidData_UpdatesCharacter() {
        // Arrange
        var character = CreateTestCharacter("OriginalName", "user1");
        _context.CharacterModel.Add(character);
        await _context.SaveChangesAsync();
        _context.Entry(character).State = EntityState.Detached;

        var updatedCharacter = CreateTestCharacter("UpdatedName", "user1");
        updatedCharacter.Id = character.Id;

        // Act
        var result = await _characterModelService.UpdateAsync(character.Id, updatedCharacter);

        // Assert
        Assert.IsNotNull(result);
        var characterInDb = await _context.CharacterModel.FindAsync(character.Id);
        Assert.AreEqual("UpdatedName", characterInDb.Name);
    }

    [TestMethod]
    public async Task UpdateAsync_WithMismatchedId_ThrowsArgumentException() {
        // Arrange
        var character = CreateTestCharacter("TestCharacter", "user1");
        character.Id = 1;

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            await _characterModelService.UpdateAsync(2, character)
        );
    }

    [TestMethod]
    public async Task UpdateAsync_WithValidData_ReturnsTrueAndPersistsChanges() {
        // Arrange
        var character = CreateTestCharacter("Original", "user1");
        character.Level = 5;
        _context.CharacterModel.Add(character);
        await _context.SaveChangesAsync();
        _context.Entry(character).State = EntityState.Detached;

        var updatedCharacter = CreateTestCharacter("Updated", "user1");
        updatedCharacter.Id    = character.Id;
        updatedCharacter.Level = 10;

        // Act
        var result = await _characterModelService.UpdateAsync(character.Id, updatedCharacter);

        // Assert
        Assert.IsNotNull(result);
        var characterInDb = await _context.CharacterModel.FindAsync(character.Id);
        Assert.AreEqual("Updated", characterInDb.Name);
        Assert.AreEqual(10, characterInDb.Level);
    }

    #endregion

    #region DeleteIfUserHasAccessAsync Tests

    [TestMethod]
    public async Task DeleteIfUserHasAccessAsync_WithValidCharacter_DeletesCharacter() {
        // Arrange
        var character = CreateTestCharacter("ToDelete", "user1");
        _context.CharacterModel.Add(character);
        await _context.SaveChangesAsync();

        // Act
        var result = await _characterModelService.DeleteIfUserHasAccessAsync(character.Id, "user1");

        // Assert
        Assert.IsNotNull(result);
        var characterInDb = await _context.CharacterModel.FindAsync(character.Id);
        Assert.IsNull(characterInDb);
    }

    [TestMethod]
    public async Task DeleteIfUserHasAccessAsync_WithNonExistingCharacter_ThrowsKeyNotFoundException() {
        // Act & Assert
        await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            await _characterModelService.DeleteIfUserHasAccessAsync(999, "user1")
        );
    }

    [TestMethod]
    public async Task DeleteIfUserHasAccessAsync_WithWrongUser_ThrowsKeyNotFoundException() {
        // Arrange
        var character = CreateTestCharacter("TestCharacter", "user1");
        _context.CharacterModel.Add(character);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            await _characterModelService.DeleteIfUserHasAccessAsync(character.Id, "user2")
        );
    }

    [TestMethod]
    public async Task DeleteIfUserHasAccessAsync_WithValidCharacter_ReturnsDeletedCharacter() {
        // Arrange
        var character = CreateTestCharacter("ToDelete", "user1");
        _context.CharacterModel.Add(character);
        await _context.SaveChangesAsync();

        // Act
        var result = await _characterModelService.DeleteIfUserHasAccessAsync(character.Id, "user1");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("ToDelete", result.Name);
    }

    #endregion

    #region GenerateCharacterPdf Tests

    [TestMethod]
    public void GenerateCharacterPdf_WithNonExistingTemplate_ThrowsFileNotFoundException() {
        // Arrange
        var character = CreateTestCharacter("TestCharacter", "user1");

        // Act & Assert
        Assert.ThrowsException<FileNotFoundException>(() => _characterModelService.GenerateCharacterPdf(character)
        );
    }

    #endregion

    #region GetClasses Tests

    [TestMethod]
    public void GetClasses_ReturnsAllClasses() {
        // Act
        var result = _characterModelService.GetClasses();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(12, result.Count);
        Assert.IsTrue(result.Contains(Classes.Barbarian));
        Assert.IsTrue(result.Contains(Classes.Bard));
        Assert.IsTrue(result.Contains(Classes.Cleric));
        Assert.IsTrue(result.Contains(Classes.Druid));
        Assert.IsTrue(result.Contains(Classes.Fighter));
        Assert.IsTrue(result.Contains(Classes.Monk));
        Assert.IsTrue(result.Contains(Classes.Paladin));
        Assert.IsTrue(result.Contains(Classes.Ranger));
        Assert.IsTrue(result.Contains(Classes.Rogue));
        Assert.IsTrue(result.Contains(Classes.Sorcerer));
        Assert.IsTrue(result.Contains(Classes.Warlock));
        Assert.IsTrue(result.Contains(Classes.Wizard));
    }

    #endregion

    #region GetRaces Tests

    [TestMethod]
    public void GetRaces_ReturnsAllRaces() {
        // Act
        var result = _characterModelService.GetRaces();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(9, result.Count);
        Assert.IsTrue(result.Contains(Races.Dragonborn));
        Assert.IsTrue(result.Contains(Races.Dwarf));
        Assert.IsTrue(result.Contains(Races.Elf));
        Assert.IsTrue(result.Contains(Races.Gnome));
        Assert.IsTrue(result.Contains(Races.HalfElf));
        Assert.IsTrue(result.Contains(Races.Halfling));
        Assert.IsTrue(result.Contains(Races.HalfOrc));
        Assert.IsTrue(result.Contains(Races.Human));
        Assert.IsTrue(result.Contains(Races.Tiefling));
    }

    #endregion

    #region Helper Methods

    private CharacterModel CreateTestCharacter(string name, string createdBy) {
        return new CharacterModel {
            Name         = name,
            Class        = Classes.Fighter,
            Race         = Races.Human,
            Level        = 5,
            Speed        = 30,
            ArmorClass   = 15,
            HitPoints    = 50,
            Strength     = 16,
            Dexterity    = 14,
            Constitution = 15,
            Intelligence = 10,
            Wisdom       = 12,
            Charisma     = 8,
            Skills       = new[] { "Athletics", "Intimidation" },
            Equipment    = new[] { "Longsword", "Shield" },
            Inventory    = new[] { "Rope", "Torch" },
            Copper       = 10,
            Silver       = 20,
            Electrum     = 5,
            Gold         = 100,
            Platinum     = 2,
            CreatedBy    = createdBy
        };
    }

    #endregion
}