using Dungeon_Dashboard.Home.Data;
using Dungeon_Dashboard.Invitations.Hubs;
using Dungeon_Dashboard.Invitations.Models;
using Dungeon_Dashboard.Invitations.Services;
using Dungeon_Dashboard.Room.Models;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dungeon_DashboardTests.Invitations.Services;

[TestClass]
[TestSubject(typeof(InvitationService))]
public class InvitationServiceTest {

    [TestClass]
    public class InvitationServiceTests {
        private AppDBContext                       _context;
        private Mock<IHubContext<NotificationHub>> _mockHubContext;
        private Mock<ILogger<InvitationService>>   _mockLogger;
        private InvitationService                  _invitationService;
        private Mock<IHubClients>                  _mockClients;
        private Mock<IClientProxy>                 _mockClientProxy;

        [TestInitialize]
        public void Setup() {
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context        = new AppDBContext(options);
            _mockHubContext = new Mock<IHubContext<NotificationHub>>();
            _mockLogger     = new Mock<ILogger<InvitationService>>();

            _mockClients     = new Mock<IHubClients>();
            _mockClientProxy = new Mock<IClientProxy>();

            _mockHubContext.Setup(h => h.Clients).Returns(_mockClients.Object);
            _mockClients.Setup(c => c.User(It.IsAny<string>())).Returns(_mockClientProxy.Object);

            _invitationService = new InvitationService(_context, _mockHubContext.Object, _mockLogger.Object);
        }

        [TestCleanup]
        public void Cleanup() {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region CreateInvitationAsync Tests

        [TestMethod]
        public async Task CreateInvitationAsync_WithValidData_CreatesInvitation() {
            // Arrange
            var room = new RoomModel {
                Id           = 1,
                Name         = "Test Room",
                Participants = new List<string> { "user1" },
                Description = "test"
            };
            _context.RoomModel.Add(room);
            await _context.SaveChangesAsync();

            var invitation = new InvitationModel {
                RoomId  = 1,
                Invitee = "user2"
            };

            // Act
            var result = await _invitationService.CreateInvitationAsync(invitation, "user1");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("user1", result.Inviter);
            Assert.AreEqual("user2", result.Invitee);
            Assert.AreEqual(1, result.RoomId);
            Assert.IsNotNull(result.CreatedAt);

            var invitationsInDb = await _context.InvitationModel.ToListAsync();
            Assert.AreEqual(1, invitationsInDb.Count);
        }

        [TestMethod]
        public async Task CreateInvitationAsync_SendsSignalRNotification() {
            // Arrange
            var room = new RoomModel {
                Id           = 1,
                Name         = "Test Room",
                Participants = new List<string> { "user1" },
                Description = "test"
            };
            _context.RoomModel.Add(room);
            await _context.SaveChangesAsync();

            var invitation = new InvitationModel {
                RoomId  = 1,
                Invitee = "user2"
            };

            // Act
            await _invitationService.CreateInvitationAsync(invitation, "user1");

            // Assert
            _mockClientProxy.Verify(
                c => c.SendCoreAsync(
                    "ReceiveNotification",
                    It.Is<object[]>(o => o.Length == 1),
                    default),
                Times.Once);
        }

        [TestMethod]
        public async Task CreateInvitationAsync_WhenInviteeIsAlreadyParticipant_ReturnsNull() {
            // Arrange
            var room = new RoomModel {
                Id           = 1,
                Name         = "Test Room",
                Participants = new List<string> { "user1", "user2" },
                Description = "test"
            };
            _context.RoomModel.Add(room);
            await _context.SaveChangesAsync();

            var invitation = new InvitationModel {
                RoomId  = 1,
                Invitee = "user2"
            };

            // Act
            var result = await _invitationService.CreateInvitationAsync(invitation, "user1");

            // Assert
            Assert.IsNull(result);

            var invitationsInDb = await _context.InvitationModel.ToListAsync();
            Assert.AreEqual(0, invitationsInDb.Count);
        }

        [TestMethod]
        public async Task CreateInvitationAsync_CaseInsensitiveCheck_ReturnsNull() {
            // Arrange
            var room = new RoomModel {
                Id           = 1,
                Name         = "Test Room",
                Participants = new List<string> { "user1", "User2" },
                Description = "test"
            };
            _context.RoomModel.Add(room);
            await _context.SaveChangesAsync();

            var invitation = new InvitationModel {
                RoomId  = 1,
                Invitee = "user2"
            };

            // Act
            var result = await _invitationService.CreateInvitationAsync(invitation, "user1");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task CreateInvitationAsync_LogsInformation() {
            // Arrange
            var room = new RoomModel {
                Id           = 1,
                Name         = "Test Room",
                Participants = new List<string> { "user1" },
                Description = "test"
            };
            _context.RoomModel.Add(room);
            await _context.SaveChangesAsync();

            var invitation = new InvitationModel {
                RoomId  = 1,
                Invitee = "user2"
            };

            // Act
            await _invitationService.CreateInvitationAsync(invitation, "user1");

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("CreateInvitation")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                Times.Once);
        }

        [TestMethod]
        public async Task CreateInvitationAsync_SetsCreatedAtToCurrentTime() {
            // Arrange
            var room = new RoomModel {
                Id           = 1,
                Name         = "Test Room",
                Participants = new List<string> { "user1" },
                Description = "test"
            };
            _context.RoomModel.Add(room);
            await _context.SaveChangesAsync();

            var beforeCreation = DateTime.UtcNow;
            var invitation = new InvitationModel {
                RoomId  = 1,
                Invitee = "user2"
            };

            // Act
            var result        = await _invitationService.CreateInvitationAsync(invitation, "user1");
            var afterCreation = DateTime.UtcNow;

            // Assert
            Assert.IsNotNull(result.CreatedAt);
            Assert.IsTrue(result.CreatedAt >= beforeCreation);
            Assert.IsTrue(result.CreatedAt <= afterCreation);
        }

        #endregion

        #region GetUserInvitationsAsync Tests

        [TestMethod]
        public async Task GetUserInvitationsAsync_WithNoInvitations_ReturnsEmptyList() {
            // Act
            var result = await _invitationService.GetUserInvitationsAsync("user1");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetUserInvitationsAsync_WithMultipleInvitations_ReturnsOnlyUserInvitations() {
            // Arrange
            var invitations = new List<InvitationModel> {
                new InvitationModel { Id = 1, Invitee = "user1", Inviter = "user2", RoomId = 1 },
                new InvitationModel { Id = 2, Invitee = "user1", Inviter = "user3", RoomId = 2 },
                new InvitationModel { Id = 3, Invitee = "user2", Inviter = "user3", RoomId = 3 }
            };
            _context.InvitationModel.AddRange(invitations);
            await _context.SaveChangesAsync();

            // Act
            var result = await _invitationService.GetUserInvitationsAsync("user1");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(i => i.Invitee == "user1"));
        }

        [TestMethod]
        public async Task GetUserInvitationsAsync_WithSingleInvitation_ReturnsSingleInvitation() {
            // Arrange
            var invitation = new InvitationModel {
                Id      = 1,
                Invitee = "user1",
                Inviter = "user2",
                RoomId  = 1
            };
            _context.InvitationModel.Add(invitation);
            await _context.SaveChangesAsync();

            // Act
            var result = await _invitationService.GetUserInvitationsAsync("user1");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("user1", result[0].Invitee);
        }

        #endregion

        #region AcceptInvitationAsync Tests

        [TestMethod]
        public async Task AcceptInvitationAsync_WithValidInvitation_AcceptsAndAddsUserToRoom() {
            // Arrange
            var room = new RoomModel {
                Id           = 1,
                Name         = "Test Room",
                Participants = new List<string> { "user1" },
                Description =  "test"
            };
            _context.RoomModel.Add(room);

            var invitation = new InvitationModel {
                Id         = 1,
                Invitee    = "user2",
                Inviter    = "user1",
                RoomId     = 1,
                IsAccepted = false
            };
            _context.InvitationModel.Add(invitation);
            await _context.SaveChangesAsync();

            // Act
            var result = await _invitationService.AcceptInvitationAsync(1, "user2");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.IsTrue(result.Participants.Contains("user2"));

            var invitationInDb = await _context.InvitationModel.FindAsync(1);
            Assert.IsTrue(invitationInDb.IsAccepted == true);
        }

        [TestMethod]
        public async Task AcceptInvitationAsync_SendsSignalRNotificationToInviter() {
            // Arrange
            var room = new RoomModel {
                Id           = 1,
                Name         = "Test Room",
                Participants = new List<string> { "user1" },
                Description = "test"
            };
            _context.RoomModel.Add(room);

            var invitation = new InvitationModel {
                Id      = 1,
                Invitee = "user2",
                Inviter = "user1",
                RoomId  = 1
            };
            _context.InvitationModel.Add(invitation);
            await _context.SaveChangesAsync();

            // Act
            await _invitationService.AcceptInvitationAsync(1, "user2");

            // Assert
            _mockClientProxy.Verify(
                c => c.SendCoreAsync(
                    "InvitationAcceptedDeclined",
                    It.Is<object[]>(o => o.Length == 1),
                    default),
                Times.Once);
        }

        [TestMethod]
        public async Task AcceptInvitationAsync_WithNonExistingInvitation_ReturnsNull() {
            // Act
            var result = await _invitationService.AcceptInvitationAsync(999, "user1");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AcceptInvitationAsync_WithWrongUsername_ReturnsNull() {
            // Arrange
            var room = new RoomModel {
                Id           = 1,
                Name         = "Test Room",
                Participants = new List<string> { "user1" },
                Description = "test"
            };
            _context.RoomModel.Add(room);

            var invitation = new InvitationModel {
                Id      = 1,
                Invitee = "user2",
                Inviter = "user1",
                RoomId  = 1
            };
            _context.InvitationModel.Add(invitation);
            await _context.SaveChangesAsync();

            // Act
            var result = await _invitationService.AcceptInvitationAsync(1, "user3");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AcceptInvitationAsync_WithNonExistingRoom_ReturnsNull() {
            // Arrange
            var invitation = new InvitationModel {
                Id      = 1,
                Invitee = "user2",
                Inviter = "user1",
                RoomId  = 999
            };
            _context.InvitationModel.Add(invitation);
            await _context.SaveChangesAsync();

            // Act
            var result = await _invitationService.AcceptInvitationAsync(1, "user2");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AcceptInvitationAsync_SetsIsAcceptedToTrue() {
            // Arrange
            var room = new RoomModel {
                Id           = 1,
                Name         = "Test Room",
                Participants = new List<string> { "user1" },
                Description = "test"
            };
            _context.RoomModel.Add(room);

            var invitation = new InvitationModel {
                Id         = 1,
                Invitee    = "user2",
                Inviter    = "user1",
                RoomId     = 1,
                IsAccepted = null
            };
            _context.InvitationModel.Add(invitation);
            await _context.SaveChangesAsync();

            // Act
            await _invitationService.AcceptInvitationAsync(1, "user2");

            // Assert
            var invitationInDb = await _context.InvitationModel.FindAsync(1);
            Assert.IsNotNull(invitationInDb);
            Assert.AreEqual(true, invitationInDb.IsAccepted);
        }

        #endregion

        #region DeclineInvitationAsync Tests

        [TestMethod]
        public async Task DeclineInvitationAsync_WithValidInvitation_RemovesInvitation() {
            // Arrange
            var invitation = new InvitationModel {
                Id      = 1,
                Invitee = "user2",
                Inviter = "user1",
                RoomId  = 1
            };
            _context.InvitationModel.Add(invitation);
            await _context.SaveChangesAsync();

            // Act
            var result = await _invitationService.DeclineInvitationAsync(1, "user2");

            // Assert
            Assert.IsTrue(result);

            var invitationInDb = await _context.InvitationModel.FindAsync(1);
            Assert.IsNull(invitationInDb);
        }

        [TestMethod]
        public async Task DeclineInvitationAsync_SendsSignalRNotificationToInviter() {
            // Arrange
            var invitation = new InvitationModel {
                Id      = 1,
                Invitee = "user2",
                Inviter = "user1",
                RoomId  = 1
            };
            _context.InvitationModel.Add(invitation);
            await _context.SaveChangesAsync();

            // Act
            await _invitationService.DeclineInvitationAsync(1, "user2");

            // Assert
            _mockClientProxy.Verify(
                c => c.SendCoreAsync(
                    "InvitationAcceptedDeclined",
                    It.Is<object[]>(o => o.Length == 1),
                    default),
                Times.Once);
        }

        [TestMethod]
        public async Task DeclineInvitationAsync_WithNonExistingInvitation_ReturnsFalse() {
            // Act
            var result = await _invitationService.DeclineInvitationAsync(999, "user1");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DeclineInvitationAsync_WithWrongUsername_ReturnsFalse() {
            // Arrange
            var invitation = new InvitationModel {
                Id      = 1,
                Invitee = "user2",
                Inviter = "user1",
                RoomId  = 1
            };
            _context.InvitationModel.Add(invitation);
            await _context.SaveChangesAsync();

            // Act
            var result = await _invitationService.DeclineInvitationAsync(1, "user3");

            // Assert
            Assert.IsFalse(result);

            var invitationInDb = await _context.InvitationModel.FindAsync(1);
            Assert.IsNotNull(invitationInDb);
        }

        #endregion

        #region DeleteInvitationAsync Tests

        [TestMethod]
        public async Task DeleteInvitationAsync_WithValidInvitation_RemovesInvitation() {
            // Arrange
            var invitation = new InvitationModel {
                Id      = 1,
                Invitee = "user2",
                Inviter = "user1",
                RoomId  = 1
            };
            _context.InvitationModel.Add(invitation);
            await _context.SaveChangesAsync();

            // Act
            var result = await _invitationService.DeleteInvitationAsync(1, "user1");

            // Assert
            Assert.IsTrue(result);

            var invitationInDb = await _context.InvitationModel.FindAsync(1);
            Assert.IsNull(invitationInDb);
        }

        [TestMethod]
        public async Task DeleteInvitationAsync_WithNonExistingInvitation_ReturnsFalse() {
            // Act
            var result = await _invitationService.DeleteInvitationAsync(999, "user1");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DeleteInvitationAsync_WithWrongUsername_ReturnsFalse() {
            // Arrange
            var invitation = new InvitationModel {
                Id      = 1,
                Invitee = "user2",
                Inviter = "user1",
                RoomId  = 1
            };
            _context.InvitationModel.Add(invitation);
            await _context.SaveChangesAsync();

            // Act
            var result = await _invitationService.DeleteInvitationAsync(1, "user3");

            // Assert
            Assert.IsFalse(result);

            var invitationInDb = await _context.InvitationModel.FindAsync(1);
            Assert.IsNotNull(invitationInDb);
        }

        [TestMethod]
        public async Task DeleteInvitationAsync_OnlyInviterCanDelete_InviteeCannotDelete() {
            // Arrange
            var invitation = new InvitationModel {
                Id      = 1,
                Invitee = "user2",
                Inviter = "user1",
                RoomId  = 1
            };
            _context.InvitationModel.Add(invitation);
            await _context.SaveChangesAsync();

            // Act
            var result = await _invitationService.DeleteInvitationAsync(1, "user2");

            // Assert
            Assert.IsFalse(result);

            var invitationInDb = await _context.InvitationModel.FindAsync(1);
            Assert.IsNotNull(invitationInDb);
        }

        #endregion
    }
}