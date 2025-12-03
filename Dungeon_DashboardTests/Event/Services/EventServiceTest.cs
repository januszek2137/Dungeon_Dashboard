using Dungeon_Dashboard.Event.Models;
using Dungeon_Dashboard.Event.Services;
using Dungeon_Dashboard.Home.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dungeon_DashboardTests.Event.Services {
    
    [TestClass]
    public class EventServiceTest {
        private AppDBContext _context;
        private EventService _eventService;

        [TestInitialize]
        public void Setup() {
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDBContext(options);
            _eventService = new EventService(_context);
        }

        [TestCleanup]
        public void Cleanup() {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region GetAllEventsAsync Tests

        [TestMethod]
        public async Task GetAllEventsAsync_WithNoEvents_ReturnsEmptyList() {
            // Act
            var result = await _eventService.GetAllEventsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetAllEventsAsync_WithMultipleEvents_ReturnsAllEvents() {
            // Arrange
            var events = new List<EventModel> {
                new EventModel { 
                    Id = 1, 
                    Title = "Event 1", 
                    Description = "Description 1",
                    Start = DateTime.Now,
                    Location = "Location 1"
                },
                new EventModel { 
                    Id = 2, 
                    Title = "Event 2", 
                    Description = "Description 2",
                    Start = DateTime.Now.AddDays(1),
                    Location = "Location 2"
                },
                new EventModel { 
                    Id = 3, 
                    Title = "Event 3", 
                    Description = "Description 3",
                    Start = DateTime.Now.AddDays(2),
                    Location = "Location 3"
                }
            };
            _context.EventModel.AddRange(events);
            await _context.SaveChangesAsync();

            // Act
            var result = await _eventService.GetAllEventsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
        }

        #endregion

        #region GetEventByIdAsync Tests

        [TestMethod]
        public async Task GetEventByIdAsync_WithExistingId_ReturnsEvent() {
            // Arrange
            var eventModel = new EventModel { 
                Id = 1, 
                Title = "Test Event", 
                Description = "Test Description",
                Start = DateTime.Now,
                Location = "Test Location"
            };
            _context.EventModel.Add(eventModel);
            await _context.SaveChangesAsync();

            // Act
            var result = await _eventService.GetEventByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Test Event", result.Title);
            Assert.AreEqual("Test Description", result.Description);
            Assert.AreEqual("Test Location", result.Location);
        }

        [TestMethod]
        public async Task GetEventByIdAsync_WithNonExistingId_ReturnsNull() {
            // Act
            var result = await _eventService.GetEventByIdAsync(999);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetEventByIdAsync_WithMultipleEvents_ReturnsCorrectEvent() {
            // Arrange
            var events = new List<EventModel> {
                new EventModel { 
                    Id = 1, 
                    Title = "Event 1", 
                    Description = "Description 1",
                    Start = DateTime.Now,
                    Location = "Location 1"
                },
                new EventModel { 
                    Id = 2, 
                    Title = "Event 2", 
                    Description = "Description 2",
                    Start = DateTime.Now.AddDays(1),
                    Location = "Location 2"
                },
                new EventModel { 
                    Id = 3, 
                    Title = "Event 3", 
                    Description = "Description 3",
                    Start = DateTime.Now.AddDays(2),
                    Location = "Location 3"
                }
            };
            _context.EventModel.AddRange(events);
            await _context.SaveChangesAsync();

            // Act
            var result = await _eventService.GetEventByIdAsync(2);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Id);
            Assert.AreEqual("Event 2", result.Title);
            Assert.AreEqual("Location 2", result.Location);
        }

        #endregion

        #region CreateEventAsync Tests

        [TestMethod]
        public async Task CreateEventAsync_WithValidEvent_AddsEventToDatabase() {
            // Arrange
            var eventModel = new EventModel { 
                Title = "New Event", 
                Description = "New Description",
                Start = DateTime.Now,
                Location = "New Location"
            };

            // Act
            var result = await _eventService.CreateEventAsync(eventModel);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("New Event", result.Title);
            Assert.AreEqual("New Description", result.Description);
            Assert.AreEqual("New Location", result.Location);
            
            var eventsInDb = await _context.EventModel.ToListAsync();
            Assert.AreEqual(1, eventsInDb.Count);
        }

        [TestMethod]
        public async Task CreateEventAsync_WithValidEvent_ReturnsCreatedEvent() {
            // Arrange
            var eventModel = new EventModel { 
                Title = "Test Event", 
                Description = "Test Description",
                Start = DateTime.Now,
                Location = "Test Location"
            };

            // Act
            var result = await _eventService.CreateEventAsync(eventModel);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(eventModel.Title, result.Title);
            Assert.AreEqual(eventModel.Description, result.Description);
            Assert.AreEqual(eventModel.Location, result.Location);
        }

        [TestMethod]
        public async Task CreateEventAsync_WithMultipleEvents_AddsAllEventsToDatabase() {
            // Arrange
            var event1 = new EventModel { 
                Title = "Event 1", 
                Description = "Description 1",
                Start = DateTime.Now,
                Location = "Location 1"
            };
            var event2 = new EventModel { 
                Title = "Event 2", 
                Description = "Description 2",
                Start = DateTime.Now.AddDays(1),
                Location = "Location 2"
            };

            // Act
            await _eventService.CreateEventAsync(event1);
            await _eventService.CreateEventAsync(event2);

            // Assert
            var eventsInDb = await _context.EventModel.ToListAsync();
            Assert.AreEqual(2, eventsInDb.Count);
        }

        #endregion

        #region UpdateEventAsync Tests

        [TestMethod]
        public async Task UpdateEventAsync_WithValidData_UpdatesEvent() {
            // Arrange
            var eventModel = new EventModel { 
                Id = 1, 
                Title = "Original Event", 
                Description = "Original Description",
                Start = DateTime.Now,
                Location = "Original Location"
            };
            _context.EventModel.Add(eventModel);
            await _context.SaveChangesAsync();
            _context.Entry(eventModel).State = EntityState.Detached;

            var updatedEvent = new EventModel { 
                Id = 1, 
                Title = "Updated Event", 
                Description = "Updated Description",
                Start = DateTime.Now.AddDays(1),
                Location = "Updated Location"
            };

            // Act
            var result = await _eventService.UpdateEventAsync(1, updatedEvent);

            // Assert
            Assert.IsTrue(result);
            var eventInDb = await _context.EventModel.FindAsync(1);
            Assert.IsNotNull(eventInDb);
            Assert.AreEqual("Updated Event", eventInDb.Title);
            Assert.AreEqual("Updated Description", eventInDb.Description);
            Assert.AreEqual("Updated Location", eventInDb.Location);
        }

        [TestMethod]
        public async Task UpdateEventAsync_WithMismatchedId_ReturnsFalse() {
            // Arrange
            var eventModel = new EventModel { 
                Id = 1, 
                Title = "Test Event", 
                Description = "Test Description",
                Start = DateTime.Now,
                Location = "Test Location"
            };
            _context.EventModel.Add(eventModel);
            await _context.SaveChangesAsync();

            var updatedEvent = new EventModel { 
                Id = 2, 
                Title = "Updated Event", 
                Description = "Updated Description",
                Start = DateTime.Now,
                Location = "Updated Location"
            };

            // Act
            var result = await _eventService.UpdateEventAsync(1, updatedEvent);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task UpdateEventAsync_WithNonExistingEvent_ReturnsFalse() {
            // Arrange
            var updatedEvent = new EventModel { 
                Id = 999, 
                Title = "Non-Existing Event", 
                Description = "Description",
                Start = DateTime.Now,
                Location = "Location"
            };

            // Act
            var result = await _eventService.UpdateEventAsync(999, updatedEvent);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task UpdateEventAsync_WithValidData_ReturnsTrueAndPersistsChanges() {
            // Arrange
            var eventModel = new EventModel { 
                Id = 1, 
                Title = "Original", 
                Description = "Original",
                Start = DateTime.Now,
                Location = "Original"
            };
            _context.EventModel.Add(eventModel);
            await _context.SaveChangesAsync();
            _context.Entry(eventModel).State = EntityState.Detached;

            var updatedEvent = new EventModel { 
                Id = 1, 
                Title = "Updated", 
                Description = "Updated",
                Start = DateTime.Now.AddDays(1),
                Location = "Updated"
            };

            // Act
            var result = await _eventService.UpdateEventAsync(1, updatedEvent);

            // Assert
            Assert.IsTrue(result);
            
            // Verify changes persisted
            var eventInDb = await _context.EventModel.FindAsync(1);
            Assert.AreEqual("Updated", eventInDb.Title);
            Assert.AreEqual("Updated", eventInDb.Description);
            Assert.AreEqual("Updated", eventInDb.Location);
        }

        #endregion

        #region DeleteEventAsync Tests

        [TestMethod]
        public async Task DeleteEventAsync_WithExistingEvent_DeletesEventAndReturnsTrue() {
            // Arrange
            var eventModel = new EventModel { 
                Id = 1, 
                Title = "Event to Delete", 
                Description = "Description",
                Start = DateTime.Now,
                Location = "Location"
            };
            _context.EventModel.Add(eventModel);
            await _context.SaveChangesAsync();

            // Act
            var result = await _eventService.DeleteEventAsync(1);

            // Assert
            Assert.IsTrue(result);
            var eventInDb = await _context.EventModel.FindAsync(1);
            Assert.IsNull(eventInDb);
        }

        [TestMethod]
        public async Task DeleteEventAsync_WithNonExistingEvent_ReturnsFalse() {
            // Act
            var result = await _eventService.DeleteEventAsync(999);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DeleteEventAsync_WithExistingEvent_RemovesFromDatabase() {
            // Arrange
            var eventModel = new EventModel { 
                Id = 1, 
                Title = "Event to Delete", 
                Description = "Description",
                Start = DateTime.Now,
                Location = "Location"
            };
            _context.EventModel.Add(eventModel);
            await _context.SaveChangesAsync();

            // Act
            await _eventService.DeleteEventAsync(1);

            // Assert
            var eventsInDb = await _context.EventModel.ToListAsync();
            Assert.AreEqual(0, eventsInDb.Count);
        }

        [TestMethod]
        public async Task DeleteEventAsync_WithMultipleEvents_DeletesOnlySpecifiedEvent() {
            // Arrange
            var events = new List<EventModel> {
                new EventModel { 
                    Id = 1, 
                    Title = "Event 1", 
                    Description = "Description 1",
                    Start = DateTime.Now,
                    Location = "Location 1"
                },
                new EventModel { 
                    Id = 2, 
                    Title = "Event 2", 
                    Description = "Description 2",
                    Start = DateTime.Now.AddDays(1),
                    Location = "Location 2"
                },
                new EventModel { 
                    Id = 3, 
                    Title = "Event 3", 
                    Description = "Description 3",
                    Start = DateTime.Now.AddDays(2),
                    Location = "Location 3"
                }
            };
            _context.EventModel.AddRange(events);
            await _context.SaveChangesAsync();

            // Act
            var result = await _eventService.DeleteEventAsync(2);

            // Assert
            Assert.IsTrue(result);
            var eventsInDb = await _context.EventModel.ToListAsync();
            Assert.AreEqual(2, eventsInDb.Count);
            Assert.IsFalse(eventsInDb.Any(e => e.Id == 2));
            Assert.IsTrue(eventsInDb.Any(e => e.Id == 1));
            Assert.IsTrue(eventsInDb.Any(e => e.Id == 3));
        }

        #endregion
    }
}