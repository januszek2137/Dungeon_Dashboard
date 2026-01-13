using Dungeon_Dashboard.Room;
using Dungeon_Dashboard.Room.Hubs;
using Dungeon_Dashboard.Room.Models;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace Dungeon_DashboardTests.Room.Services;

[TestClass]
[TestSubject(typeof(MapService))]
public class MapServiceTest {

    private Mock<IMarkerRepository>   _mockMarkerRepo;
    private Mock<IRoomRepository>     _mockRoomRepo;
    private Mock<IHubContext<MapHub>> _mockHubContext;
    private Mock<IHubClients>         _mockClients;
    private Mock<IClientProxy>        _mockClientProxy;
    private MapService                _mapService;

    [TestInitialize]
    public void Setup() {
        _mockMarkerRepo  = new Mock<IMarkerRepository>();
        _mockRoomRepo    = new Mock<IRoomRepository>();
        _mockHubContext  = new Mock<IHubContext<MapHub>>();
        _mockClients     = new Mock<IHubClients>();
        _mockClientProxy = new Mock<IClientProxy>();

        _mockHubContext.Setup(h => h.Clients).Returns(_mockClients.Object);
        _mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(_mockClientProxy.Object);

        _mapService = new MapService(
            _mockMarkerRepo.Object,
            _mockRoomRepo.Object,
            _mockHubContext.Object
        );
    }

    #region UploadMapAsync Tests

    [TestMethod]
    public async Task UploadMapAsync_WithValidData_UploadsMapAndReturnsRoom() {
        // Arrange
        var roomId   = 1;
        var userName = "testUser";
        var room = new RoomModel {
            Id          = roomId,
            Name        = "Test Room",
            Description = "Test Description",
            CreatedBy   = userName
        };
        var mapUrl   = "https://example.com/map.png";
        var mockFile = new Mock<IFormFile>();

        _mockRoomRepo.Setup(r => r.GetByIdAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _mockRoomRepo.Setup(r => r.SaveMapAsync(room, mockFile.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mapUrl);

        // Act
        var result = await _mapService.UploadMapAsync(roomId, mockFile.Object, userName);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(roomId, result.Id);
        _mockRoomRepo.Verify(r => r.SaveMapAsync(room, mockFile.Object, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task UploadMapAsync_SendsSignalRNotification() {
        // Arrange
        var roomId   = 1;
        var userName = "testUser";
        var room = new RoomModel {
            Id          = roomId,
            Name        = "Test Room",
            Description = "Test Description",
            CreatedBy   = userName
        };
        var mapUrl   = "https://example.com/map.png";
        var mockFile = new Mock<IFormFile>();

        _mockRoomRepo.Setup(r => r.GetByIdAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _mockRoomRepo.Setup(r => r.SaveMapAsync(room, mockFile.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mapUrl);

        // Act
        await _mapService.UploadMapAsync(roomId, mockFile.Object, userName);

        // Assert
        _mockClientProxy.Verify(
            c => c.SendCoreAsync(
                "MapUpdated",
                It.Is<object[]>(o => o.Length == 1 && o[0].ToString() == mapUrl),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task UploadMapAsync_WithNonExistingRoom_ReturnsNull() {
        // Arrange
        var roomId   = 999;
        var userName = "testUser";
        var mockFile = new Mock<IFormFile>();

        _mockRoomRepo.Setup(r => r.GetByIdAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RoomModel)null);

        // Act
        var result = await _mapService.UploadMapAsync(roomId, mockFile.Object, userName);

        // Assert
        Assert.IsNull(result);
        _mockRoomRepo.Verify(
            r => r.SaveMapAsync(It.IsAny<RoomModel>(), It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    public async Task UploadMapAsync_WithWrongUser_ReturnsNull() {
        // Arrange
        var roomId   = 1;
        var userName = "testUser";
        var room = new RoomModel {
            Id          = roomId,
            Name        = "Test Room",
            Description = "Test Description",
            CreatedBy   = "differentUser"
        };
        var mockFile = new Mock<IFormFile>();

        _mockRoomRepo.Setup(r => r.GetByIdAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        // Act
        var result = await _mapService.UploadMapAsync(roomId, mockFile.Object, userName);

        // Assert
        Assert.IsNull(result);
        _mockRoomRepo.Verify(
            r => r.SaveMapAsync(It.IsAny<RoomModel>(), It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    public async Task UploadMapAsync_CaseInsensitiveUserComparison_UploadsMap() {
        // Arrange
        var roomId   = 1;
        var userName = "TestUser";
        var room = new RoomModel {
            Id          = roomId,
            Name        = "Test Room",
            Description = "Test Description",
            CreatedBy   = "testuser"
        };
        var mapUrl   = "https://example.com/map.png";
        var mockFile = new Mock<IFormFile>();

        _mockRoomRepo.Setup(r => r.GetByIdAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _mockRoomRepo.Setup(r => r.SaveMapAsync(room, mockFile.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mapUrl);

        // Act
        var result = await _mapService.UploadMapAsync(roomId, mockFile.Object, userName);

        // Assert
        Assert.IsNotNull(result);
        _mockRoomRepo.Verify(r => r.SaveMapAsync(room, mockFile.Object, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task UploadMapAsync_WithCancellationToken_PassesTokenToRepositories() {
        // Arrange
        var roomId   = 1;
        var userName = "testUser";
        var room = new RoomModel {
            Id          = roomId,
            Name        = "Test Room",
            Description = "Test Description",
            CreatedBy   = userName
        };
        var mapUrl            = "https://example.com/map.png";
        var mockFile          = new Mock<IFormFile>();
        var cancellationToken = new CancellationToken();

        _mockRoomRepo.Setup(r => r.GetByIdAsync(roomId, cancellationToken))
            .ReturnsAsync(room);
        _mockRoomRepo.Setup(r => r.SaveMapAsync(room, mockFile.Object, cancellationToken))
            .ReturnsAsync(mapUrl);

        // Act
        await _mapService.UploadMapAsync(roomId, mockFile.Object, userName, cancellationToken);

        // Assert
        _mockRoomRepo.Verify(r => r.GetByIdAsync(roomId, cancellationToken), Times.Once);
        _mockRoomRepo.Verify(r => r.SaveMapAsync(room, mockFile.Object, cancellationToken), Times.Once);
    }

    [TestMethod]
    public async Task UploadMapAsync_SendsNotificationToCorrectGroup() {
        // Arrange
        var roomId   = 1;
        var userName = "testUser";
        var room = new RoomModel {
            Id          = roomId,
            Name        = "Test Room",
            Description = "Test Description",
            CreatedBy   = userName
        };
        var mapUrl   = "https://example.com/map.png";
        var mockFile = new Mock<IFormFile>();

        _mockRoomRepo.Setup(r => r.GetByIdAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _mockRoomRepo.Setup(r => r.SaveMapAsync(room, mockFile.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mapUrl);

        // Act
        await _mapService.UploadMapAsync(roomId, mockFile.Object, userName);

        // Assert
        _mockClients.Verify(c => c.Group(roomId.ToString()), Times.Once);
    }

    #endregion

    #region MoveMarkerAsync Tests

    [TestMethod]
    public async Task MoveMarkerAsync_WithValidData_CreatesAndReturnsMarker() {
        // Arrange
        var roomId = 1;
        var userId = "user1";
        var x      = 100;
        var y      = 200;

        _mockMarkerRepo.Setup(r => r.UpsertAsync(It.IsAny<MarkerModel>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _mapService.MoveMarkerAsync(roomId, userId, x, y);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(roomId, result.RoomId);
        Assert.AreEqual(userId, result.UserId);
        Assert.AreEqual(x, result.X);
        Assert.AreEqual(y, result.Y);
    }

    [TestMethod]
    public async Task MoveMarkerAsync_CallsUpsertOnRepository() {
        // Arrange
        var roomId = 1;
        var userId = "user1";
        var x      = 100;
        var y      = 200;

        _mockMarkerRepo.Setup(r => r.UpsertAsync(It.IsAny<MarkerModel>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _mapService.MoveMarkerAsync(roomId, userId, x, y);

        // Assert
        _mockMarkerRepo.Verify(
            r => r.UpsertAsync(
                It.Is<MarkerModel>(m =>
                    m.RoomId == roomId &&
                    m.UserId == userId &&
                    m.X      == x      &&
                    m.Y      == y),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task MoveMarkerAsync_WithZeroCoordinates_CreatesMarkerAtOrigin() {
        // Arrange
        var roomId = 1;
        var userId = "user1";
        var x      = 0;
        var y      = 0;

        _mockMarkerRepo.Setup(r => r.UpsertAsync(It.IsAny<MarkerModel>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _mapService.MoveMarkerAsync(roomId, userId, x, y);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.X);
        Assert.AreEqual(0, result.Y);
    }

    [TestMethod]
    public async Task MoveMarkerAsync_WithNegativeCoordinates_CreatesMarkerWithNegativeValues() {
        // Arrange
        var roomId = 1;
        var userId = "user1";
        var x      = -50;
        var y      = -100;

        _mockMarkerRepo.Setup(r => r.UpsertAsync(It.IsAny<MarkerModel>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _mapService.MoveMarkerAsync(roomId, userId, x, y);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(-50, result.X);
        Assert.AreEqual(-100, result.Y);
    }

    [TestMethod]
    public async Task MoveMarkerAsync_WithCancellationToken_PassesTokenToRepository() {
        // Arrange
        var roomId            = 1;
        var userId            = "user1";
        var x                 = 100;
        var y                 = 200;
        var cancellationToken = new CancellationToken();

        _mockMarkerRepo.Setup(r => r.UpsertAsync(It.IsAny<MarkerModel>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _mapService.MoveMarkerAsync(roomId, userId, x, y, cancellationToken);

        // Assert
        _mockMarkerRepo.Verify(
            r => r.UpsertAsync(It.IsAny<MarkerModel>(), cancellationToken),
            Times.Once);
    }

    #endregion

    #region GetMarkersAsync Tests

    [TestMethod]
    public async Task GetMarkersAsync_WithExistingMarkers_ReturnsMarkers() {
        // Arrange
        var roomId = 1;
        var markers = new List<MarkerModel> {
            new MarkerModel { RoomId = roomId, UserId = "user1", X = 100, Y = 200 },
            new MarkerModel { RoomId = roomId, UserId = "user2", X = 150, Y = 250 }
        };

        _mockMarkerRepo.Setup(r => r.GetByRoomAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(markers);

        // Act
        var result = await _mapService.GetMarkersAsync(roomId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(m => m.RoomId == roomId));
    }

    [TestMethod]
    public async Task GetMarkersAsync_WithNoMarkers_ReturnsEmptyList() {
        // Arrange
        var roomId  = 1;
        var markers = new List<MarkerModel>();

        _mockMarkerRepo.Setup(r => r.GetByRoomAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(markers);

        // Act
        var result = await _mapService.GetMarkersAsync(roomId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task GetMarkersAsync_CallsRepositoryWithCorrectRoomId() {
        // Arrange
        var roomId  = 1;
        var markers = new List<MarkerModel>();

        _mockMarkerRepo.Setup(r => r.GetByRoomAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(markers);

        // Act
        await _mapService.GetMarkersAsync(roomId);

        // Assert
        _mockMarkerRepo.Verify(r => r.GetByRoomAsync(roomId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task GetMarkersAsync_WithCancellationToken_PassesTokenToRepository() {
        // Arrange
        var roomId            = 1;
        var markers           = new List<MarkerModel>();
        var cancellationToken = new CancellationToken();

        _mockMarkerRepo.Setup(r => r.GetByRoomAsync(roomId, cancellationToken))
            .ReturnsAsync(markers);

        // Act
        await _mapService.GetMarkersAsync(roomId, cancellationToken);

        // Assert
        _mockMarkerRepo.Verify(r => r.GetByRoomAsync(roomId, cancellationToken), Times.Once);
    }

    [TestMethod]
    public async Task GetMarkersAsync_WithMultipleMarkers_ReturnsAllMarkers() {
        // Arrange
        var roomId = 1;
        var markers = new List<MarkerModel> {
            new MarkerModel { RoomId = roomId, UserId = "user1", X = 10, Y = 20 },
            new MarkerModel { RoomId = roomId, UserId = "user2", X = 30, Y = 40 },
            new MarkerModel { RoomId = roomId, UserId = "user3", X = 50, Y = 60 }
        };

        _mockMarkerRepo.Setup(r => r.GetByRoomAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(markers);

        // Act
        var result = await _mapService.GetMarkersAsync(roomId);

        // Assert
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual("user1", result[0].UserId);
        Assert.AreEqual("user2", result[1].UserId);
        Assert.AreEqual("user3", result[2].UserId);
    }

    #endregion
}