using Dungeon_Dashboard.Room;
using Dungeon_Dashboard.Room.Models;
using JetBrains.Annotations;
using Moq;

namespace Dungeon_DashboardTests.Room.Services;

[TestClass]
[TestSubject(typeof(RoomService))]
public class RoomServiceTest {

    private Mock<IRoomRepository> _mockRoomRepo;
    private RoomService           _roomService;

    [TestInitialize]
    public void Setup() {
        _mockRoomRepo = new Mock<IRoomRepository>();
        _roomService  = new RoomService(_mockRoomRepo.Object);
    }

    #region CreateRoomAsync Tests

    [TestMethod]
    public async Task CreateRoomAsync_WithValidData_CreatesRoomWithCorrectProperties() {
        // Arrange
        var model = new RoomModel {
            Name        = "Test Room",
            Description = "Test Description"
        };
        var createdBy = "testUser";

        _mockRoomRepo.Setup(r => r.AddAsync(It.IsAny<RoomModel>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _roomService.CreateRoomAsync(model, createdBy);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Test Room", result.Name);
        Assert.AreEqual("Test Description", result.Description);
        Assert.AreEqual(createdBy, result.CreatedBy);
        Assert.AreEqual("/maps/default.jpg", result.MapUrl);
    }

    [TestMethod]
    public async Task CreateRoomAsync_AddsCreatorToParticipants() {
        // Arrange
        var model = new RoomModel {
            Name        = "Test Room",
            Description = "Test Description"
        };
        var createdBy = "testUser";

        _mockRoomRepo.Setup(r => r.AddAsync(It.IsAny<RoomModel>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _roomService.CreateRoomAsync(model, createdBy);

        // Assert
        Assert.IsNotNull(result.Participants);
        Assert.AreEqual(1, result.Participants.Count);
        Assert.IsTrue(result.Participants.Contains(createdBy));
    }

    [TestMethod]
    public async Task CreateRoomAsync_CallsRepositoryAddAsync() {
        // Arrange
        var model = new RoomModel {
            Name        = "Test Room",
            Description = "Test Description"
        };
        var createdBy = "testUser";

        _mockRoomRepo.Setup(r => r.AddAsync(It.IsAny<RoomModel>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _roomService.CreateRoomAsync(model, createdBy);

        // Assert
        _mockRoomRepo.Verify(
            r => r.AddAsync(
                It.Is<RoomModel>(rm =>
                    rm.Name        == "Test Room"        &&
                    rm.Description == "Test Description" &&
                    rm.CreatedBy   == createdBy          &&
                    rm.MapUrl      == "/maps/default.jpg"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task CreateRoomAsync_WithCancellationToken_PassesTokenToRepository() {
        // Arrange
        var model = new RoomModel {
            Name        = "Test Room",
            Description = "Test Description"
        };
        var createdBy         = "testUser";
        var cancellationToken = new CancellationToken();

        _mockRoomRepo.Setup(r => r.AddAsync(It.IsAny<RoomModel>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _roomService.CreateRoomAsync(model, createdBy, cancellationToken);

        // Assert
        _mockRoomRepo.Verify(
            r => r.AddAsync(It.IsAny<RoomModel>(), cancellationToken),
            Times.Once);
    }

    [TestMethod]
    public async Task CreateRoomAsync_SetsDefaultMapUrl() {
        // Arrange
        var model = new RoomModel {
            Name        = "Test Room",
            Description = "Test Description"
        };
        var createdBy = "testUser";

        _mockRoomRepo.Setup(r => r.AddAsync(It.IsAny<RoomModel>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _roomService.CreateRoomAsync(model, createdBy);

        // Assert
        Assert.AreEqual("/maps/default.jpg", result.MapUrl);
    }

    [TestMethod]
    public async Task CreateRoomAsync_ReturnsCreatedRoom() {
        // Arrange
        var model = new RoomModel {
            Name        = "Test Room",
            Description = "Test Description"
        };
        var createdBy = "testUser";

        _mockRoomRepo.Setup(r => r.AddAsync(It.IsAny<RoomModel>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _roomService.CreateRoomAsync(model, createdBy);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(RoomModel));
    }

    #endregion

    #region GetRoomForUserAsync Tests

    [TestMethod]
    public async Task GetRoomForUserAsync_WithValidRoomAndUser_ReturnsRoom() {
        // Arrange
        var roomId = 1;
        var user   = "testUser";
        var room = new RoomModel {
            Id           = roomId,
            Name         = "Test Room",
            Description  = "Test Description",
            CreatedBy    = "creator",
            Participants = new List<string> { "creator", user }
        };

        _mockRoomRepo.Setup(r => r.GetByIdWithDetailsAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        // Act
        var result = await _roomService.GetRoomForUserAsync(roomId, user);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(roomId, result.Id);
        Assert.AreEqual("Test Room", result.Name);
    }

    [TestMethod]
    public async Task GetRoomForUserAsync_WithNonExistingRoom_ThrowsKeyNotFoundException() {
        // Arrange
        var roomId = 999;
        var user   = "testUser";

        _mockRoomRepo.Setup(r => r.GetByIdWithDetailsAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RoomModel)null);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            await _roomService.GetRoomForUserAsync(roomId, user)
        );
    }

    [TestMethod]
    public async Task GetRoomForUserAsync_WithNullUser_ThrowsUnauthorizedAccessException() {
        // Arrange
        var roomId = 1;
        var room = new RoomModel {
            Id           = roomId,
            Name         = "Test Room",
            Description  = "Test Description",
            CreatedBy    = "creator",
            Participants = new List<string> { "creator" }
        };

        _mockRoomRepo.Setup(r => r.GetByIdWithDetailsAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(async () =>
            await _roomService.GetRoomForUserAsync(roomId, null)
        );
    }

    [TestMethod]
    public async Task GetRoomForUserAsync_WithUserNotInParticipants_ThrowsUnauthorizedAccessException() {
        // Arrange
        var roomId = 1;
        var user   = "unauthorizedUser";
        var room = new RoomModel {
            Id           = roomId,
            Name         = "Test Room",
            Description  = "Test Description",
            CreatedBy    = "creator",
            Participants = new List<string> { "creator", "authorizedUser" }
        };

        _mockRoomRepo.Setup(r => r.GetByIdWithDetailsAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(async () =>
            await _roomService.GetRoomForUserAsync(roomId, user)
        );
    }

    [TestMethod]
    public async Task GetRoomForUserAsync_WithNullParticipants_ThrowsUnauthorizedAccessException() {
        // Arrange
        var roomId = 1;
        var user   = "testUser";
        var room = new RoomModel {
            Id           = roomId,
            Name         = "Test Room",
            Description  = "Test Description",
            CreatedBy    = "creator",
            Participants = null
        };

        _mockRoomRepo.Setup(r => r.GetByIdWithDetailsAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(async () =>
            await _roomService.GetRoomForUserAsync(roomId, user)
        );
    }

    [TestMethod]
    public async Task GetRoomForUserAsync_WithCancellationToken_PassesTokenToRepository() {
        // Arrange
        var roomId            = 1;
        var user              = "testUser";
        var cancellationToken = new CancellationToken();
        var room = new RoomModel {
            Id           = roomId,
            Name         = "Test Room",
            Description  = "Test Description",
            CreatedBy    = "creator",
            Participants = new List<string> { user }
        };

        _mockRoomRepo.Setup(r => r.GetByIdWithDetailsAsync(roomId, cancellationToken))
            .ReturnsAsync(room);

        // Act
        await _roomService.GetRoomForUserAsync(roomId, user, cancellationToken);

        // Assert
        _mockRoomRepo.Verify(
            r => r.GetByIdWithDetailsAsync(roomId, cancellationToken),
            Times.Once);
    }

    [TestMethod]
    public async Task GetRoomForUserAsync_CallsGetByIdWithDetailsAsync() {
        // Arrange
        var roomId = 1;
        var user   = "testUser";
        var room = new RoomModel {
            Id           = roomId,
            Name         = "Test Room",
            Description  = "Test Description",
            CreatedBy    = "creator",
            Participants = new List<string> { user }
        };

        _mockRoomRepo.Setup(r => r.GetByIdWithDetailsAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        // Act
        await _roomService.GetRoomForUserAsync(roomId, user);

        // Assert
        _mockRoomRepo.Verify(
            r => r.GetByIdWithDetailsAsync(roomId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task GetRoomForUserAsync_WithCreatorAsUser_ReturnsRoom() {
        // Arrange
        var roomId  = 1;
        var creator = "creator";
        var room = new RoomModel {
            Id           = roomId,
            Name         = "Test Room",
            Description  = "Test Description",
            CreatedBy    = creator,
            Participants = new List<string> { creator }
        };

        _mockRoomRepo.Setup(r => r.GetByIdWithDetailsAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        // Act
        var result = await _roomService.GetRoomForUserAsync(roomId, creator);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(creator, result.CreatedBy);
    }

    #endregion

    #region GetRoomsForUserAsync Tests

    [TestMethod]
    public async Task GetRoomsForUserAsync_WithExistingRooms_ReturnsRoomsList() {
        // Arrange
        var user = "testUser";
        var rooms = new List<RoomModel> {
            new RoomModel {
                Id           = 1,
                Name         = "Room 1",
                Description  = "Description 1",
                CreatedBy    = user,
                Participants = new List<string> { user }
            },
            new RoomModel {
                Id           = 2,
                Name         = "Room 2",
                Description  = "Description 2",
                CreatedBy    = "otherUser",
                Participants = new List<string> { "otherUser", user }
            }
        };

        _mockRoomRepo.Setup(r => r.GetForUserAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rooms);

        // Act
        var result = await _roomService.GetRoomsForUserAsync(user);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public async Task GetRoomsForUserAsync_WithNoRooms_ReturnsEmptyList() {
        // Arrange
        var user  = "testUser";
        var rooms = new List<RoomModel>();

        _mockRoomRepo.Setup(r => r.GetForUserAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rooms);

        // Act
        var result = await _roomService.GetRoomsForUserAsync(user);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task GetRoomsForUserAsync_CallsRepositoryWithCorrectUser() {
        // Arrange
        var user  = "testUser";
        var rooms = new List<RoomModel>();

        _mockRoomRepo.Setup(r => r.GetForUserAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rooms);

        // Act
        await _roomService.GetRoomsForUserAsync(user);

        // Assert
        _mockRoomRepo.Verify(
            r => r.GetForUserAsync(user, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task GetRoomsForUserAsync_WithCancellationToken_PassesTokenToRepository() {
        // Arrange
        var user              = "testUser";
        var cancellationToken = new CancellationToken();
        var rooms             = new List<RoomModel>();

        _mockRoomRepo.Setup(r => r.GetForUserAsync(user, cancellationToken))
            .ReturnsAsync(rooms);

        // Act
        await _roomService.GetRoomsForUserAsync(user, cancellationToken);

        // Assert
        _mockRoomRepo.Verify(
            r => r.GetForUserAsync(user, cancellationToken),
            Times.Once);
    }

    [TestMethod]
    public async Task GetRoomsForUserAsync_WithMultipleRooms_ReturnsAllRooms() {
        // Arrange
        var user = "testUser";
        var rooms = new List<RoomModel> {
            new RoomModel {
                Id          = 1,
                Name        = "Room 1",
                Description = "Description 1",
                CreatedBy   = user
            },
            new RoomModel {
                Id          = 2,
                Name        = "Room 2",
                Description = "Description 2",
                CreatedBy   = user
            },
            new RoomModel {
                Id          = 3,
                Name        = "Room 3",
                Description = "Description 3",
                CreatedBy   = "otherUser"
            }
        };

        _mockRoomRepo.Setup(r => r.GetForUserAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rooms);

        // Act
        var result = await _roomService.GetRoomsForUserAsync(user);

        // Assert
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual("Room 1", result[0].Name);
        Assert.AreEqual("Room 2", result[1].Name);
        Assert.AreEqual("Room 3", result[2].Name);
    }

    [TestMethod]
    public async Task GetRoomsForUserAsync_ReturnsList() {
        // Arrange
        var user = "testUser";
        var rooms = new List<RoomModel> {
            new RoomModel {
                Id          = 1,
                Name        = "Room 1",
                Description = "Description 1",
                CreatedBy   = user
            }
        };

        _mockRoomRepo.Setup(r => r.GetForUserAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rooms);

        // Act
        var result = await _roomService.GetRoomsForUserAsync(user);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(List<RoomModel>));
    }

    #endregion
}