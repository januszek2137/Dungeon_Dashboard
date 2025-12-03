using Dungeon_Dashboard.Invitations.Services;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dungeon_DashboardTests.Invitations.Services;

[TestClass]
public class UserLookupServiceTest {

    private DbContextOptions<IdentityDbContext> _options;
    private IdentityDbContext                   _context;
    private UserManager<IdentityUser>           _userManager;
    private UserLookupService<IdentityUser>     _userLookupService;

    [TestInitialize]
    public void Setup() {
        _options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new IdentityDbContext(_options);

        var userStore = new UserStore<IdentityUser>(_context);
        _userManager = new UserManager<IdentityUser>(
            userStore,
            null,
            new PasswordHasher<IdentityUser>(),
            null,
            null,
            null,
            null,
            null,
            null
        );

        _userLookupService = new UserLookupService<IdentityUser>(_userManager);
    }

    [TestCleanup]
    public void Cleanup() {
        _userManager?.Dispose();
        _context?.Database.EnsureDeleted();
        _context?.Dispose();
    }

    #region SearchByEmailPrefixAsync Tests

    [TestMethod]
    public async Task SearchByEmailPrefixAsync_WithNoUsers_ReturnsEmptyList() {
        // Act
        var result = await _userLookupService.SearchByEmailPrefixAsync("test");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task SearchByEmailPrefixAsync_WithMatchingPrefix_ReturnsMatchingUsers() {
        // Arrange
        await CreateUserAsync("test1@example.com");
        await CreateUserAsync("test2@example.com");
        await CreateUserAsync("other@example.com");

        // Act
        var result = await _userLookupService.SearchByEmailPrefixAsync("test");

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(u => u.Label.StartsWith("test", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public async Task SearchByEmailPrefixAsync_WithEmptyTerm_ReturnsAllUsers() {
        // Arrange
        await CreateUserAsync("user1@example.com");
        await CreateUserAsync("user2@example.com");
        await CreateUserAsync("user3@example.com");

        // Act
        var result = await _userLookupService.SearchByEmailPrefixAsync("");

        // Assert
        Assert.AreEqual(3, result.Count);
    }

    [TestMethod]
    public async Task SearchByEmailPrefixAsync_WithNullTerm_ReturnsAllUsers() {
        // Arrange
        await CreateUserAsync("user1@example.com");
        await CreateUserAsync("user2@example.com");

        // Act
        var result = await _userLookupService.SearchByEmailPrefixAsync(null);

        // Assert
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public async Task SearchByEmailPrefixAsync_CaseInsensitive_ReturnsMatchingUsers() {
        // Arrange
        await CreateUserAsync("Test@example.com");
        await CreateUserAsync("TEST@example.com");
        await CreateUserAsync("test@example.com");

        // Act
        var result = await _userLookupService.SearchByEmailPrefixAsync("test");

        // Assert
        Assert.AreEqual(3, result.Count);
    }

    [TestMethod]
    public async Task SearchByEmailPrefixAsync_OrdersByEmail_ReturnsUsersInAlphabeticalOrder() {
        // Arrange
        await CreateUserAsync("charlie@example.com");
        await CreateUserAsync("alice@example.com");
        await CreateUserAsync("bob@example.com");

        // Act
        var result = await _userLookupService.SearchByEmailPrefixAsync("");

        // Assert
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual("alice@example.com", result[0].Label);
        Assert.AreEqual("bob@example.com", result[1].Label);
        Assert.AreEqual("charlie@example.com", result[2].Label);
    }

    [TestMethod]
    public async Task SearchByEmailPrefixAsync_WithTakeParameter_LimitsResults() {
        // Arrange
        await CreateUserAsync("test1@example.com");
        await CreateUserAsync("test2@example.com");
        await CreateUserAsync("test3@example.com");
        await CreateUserAsync("test4@example.com");
        await CreateUserAsync("test5@example.com");

        // Act
        var result = await _userLookupService.SearchByEmailPrefixAsync("test", take: 3);

        // Assert
        Assert.AreEqual(3, result.Count);
    }

    [TestMethod]
    public async Task SearchByEmailPrefixAsync_WithDefaultTake_Returns10Results() {
        // Arrange
        for (int i = 1; i <= 15; i++) {
            await CreateUserAsync($"test{i}@example.com");
        }

        // Act
        var result = await _userLookupService.SearchByEmailPrefixAsync("test");

        // Assert
        Assert.AreEqual(10, result.Count);
    }

    [TestMethod]
    public async Task SearchByEmailPrefixAsync_WithExcludeUserId_ExcludesSpecifiedUser() {
        // Arrange
        var user1 = await CreateUserAsync("test1@example.com");
        await CreateUserAsync("test2@example.com");
        await CreateUserAsync("test3@example.com");

        // Act
        var result = await _userLookupService.SearchByEmailPrefixAsync("test", excludeUserId: user1.Id);

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.IsFalse(result.Any(u => u.Id == user1.Id));
    }

    [TestMethod]
    public async Task SearchByEmailPrefixAsync_WithNonExistentExcludeUserId_ReturnsAllMatchingUsers() {
        // Arrange
        await CreateUserAsync("test1@example.com");
        await CreateUserAsync("test2@example.com");

        // Act
        var result = await _userLookupService.SearchByEmailPrefixAsync("test", excludeUserId: "nonexistent-id");

        // Assert
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public async Task SearchByEmailPrefixAsync_ReturnsUserSuggestionDto_WithCorrectProperties() {
        // Arrange
        var user = await CreateUserAsync("test@example.com");

        // Act
        var result = await _userLookupService.SearchByEmailPrefixAsync("test");

        // Assert
        Assert.AreEqual(1, result.Count);
        var dto = result[0];
        Assert.AreEqual(user.Id, dto.Id);
        Assert.AreEqual("test@example.com", dto.Label);
        Assert.AreEqual("test@example.com", dto.Value);
    }

    [TestMethod]
    public async Task SearchByEmailPrefixAsync_WithPartialEmail_ReturnsMatchingUsers() {
        // Arrange
        await CreateUserAsync("john.doe@example.com");
        await CreateUserAsync("john.smith@example.com");
        await CreateUserAsync("jane.doe@example.com");

        // Act
        var result = await _userLookupService.SearchByEmailPrefixAsync("john");

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(u => u.Label.StartsWith("john", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public async Task SearchByEmailPrefixAsync_WithNoMatches_ReturnsEmptyList() {
        // Arrange
        await CreateUserAsync("test1@example.com");
        await CreateUserAsync("test2@example.com");

        // Act
        var result = await _userLookupService.SearchByEmailPrefixAsync("nomatch");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task SearchByEmailPrefixAsync_WithSpecialCharacters_HandlesCorrectly() {
        // Arrange
        await CreateUserAsync("test+tag@example.com");
        await CreateUserAsync("test.dot@example.com");

        // Act
        var result = await _userLookupService.SearchByEmailPrefixAsync("test");

        // Assert
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public async Task SearchByEmailPrefixAsync_WithCancellationToken_CompletesSuccessfully() {
        // Arrange
        await CreateUserAsync("test@example.com");
        var cancellationToken = new CancellationToken();

        // Act
        var result = await _userLookupService.SearchByEmailPrefixAsync("test", ct: cancellationToken);

        // Assert
        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public async Task SearchByEmailPrefixAsync_MultipleParameters_AppliesAllFilters() {
        // Arrange
        var user1 = await CreateUserAsync("test1@example.com");
        await CreateUserAsync("test2@example.com");
        await CreateUserAsync("test3@example.com");
        await CreateUserAsync("test4@example.com");

        // Act
        var result = await _userLookupService.SearchByEmailPrefixAsync(
            "test",
            excludeUserId: user1.Id,
            take: 2);

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.IsFalse(result.Any(u => u.Id == user1.Id));
    }

    [TestMethod]
    public async Task SearchByEmailPrefixAsync_WithZeroTake_ReturnsEmptyList() {
        // Arrange
        await CreateUserAsync("test1@example.com");
        await CreateUserAsync("test2@example.com");

        // Act
        var result = await _userLookupService.SearchByEmailPrefixAsync("test", take: 0);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    #endregion

    #region Helper Methods

    private async Task<IdentityUser> CreateUserAsync(string email) {
        var user = new IdentityUser {
            UserName           = email,
            Email              = email,
            NormalizedEmail    = _userManager.NormalizeEmail(email),
            NormalizedUserName = _userManager.NormalizeName(email)
        };

        await _userManager.CreateAsync(user);
        return user;
    }

    #endregion
}