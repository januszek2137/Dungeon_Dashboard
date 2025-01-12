using Dungeon_Dashboard.Models;
using Dungeon_Dashboard.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Dungeon_Dashboard.Controllers.Tests {

    [TestClass]
    public class RandomCharactersControllerTests {
        private Mock<ICharacterGeneratorService> _mockGeneratorService;
        private RandomCharactersController _controller;

        [TestInitialize]
        public void Setup() {
            _mockGeneratorService = new Mock<ICharacterGeneratorService>();
            _controller = new RandomCharactersController(_mockGeneratorService.Object);
        }

        [TestMethod]
        public void GetRandomNPCTest() {
            var mockNPC = new NPC { Name = "Test NPC" };
            _mockGeneratorService.Setup(s => s.GenerateRandomNPC()).Returns(mockNPC);

            var result = _controller.GetRandomNPC();

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var returnedNPC = okResult.Value as NPC;
            Assert.IsNotNull(returnedNPC);
            Assert.AreEqual("Test NPC", returnedNPC.Name);
        }

        [TestMethod]
        public void GetRandomNPCTest_Returns500_WhenGenerationFails() {
            _mockGeneratorService.Setup(s => s.GenerateRandomNPC()).Returns((NPC)null);

            var result = _controller.GetRandomNPC();

            var statusResult = result.Result as ObjectResult;
            Assert.IsNotNull(statusResult);
            Assert.AreEqual(500, statusResult.StatusCode);
        }

        [TestMethod]
        public void GetRandomMonsterTest() {
            var mockMonster = new Monster { Species = "Dragon", Level = 10 };
            _mockGeneratorService.Setup(s => s.GenerateRandomMonster()).Returns(mockMonster);

            var result = _controller.GetRandomMonster();

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var returnedMonster = okResult.Value as Monster;
            Assert.IsNotNull(returnedMonster);
            Assert.AreEqual("Dragon", returnedMonster.Species);
            Assert.AreEqual(10, returnedMonster.Level);
        }

        [TestMethod]
        public void GetRandomMonsterTest_Returns500_WhenGenerationFails() {
            _mockGeneratorService.Setup(s => s.GenerateRandomMonster()).Returns((Monster)null);

            var result = _controller.GetRandomMonster();

            var statusResult = result.Result as ObjectResult;
            Assert.IsNotNull(statusResult);
            Assert.AreEqual(500, statusResult.StatusCode);
        }

        [TestMethod]
        public void GetRandomEncounterTest() {
            var mockEncounter = new RandomEncounter { Description = "Test Encounter" };
            _mockGeneratorService.Setup(s => s.GenerateRandomEncounter()).Returns(mockEncounter);

            var result = _controller.GetRandomEncounter();

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var returnedEncounter = okResult.Value as RandomEncounter;
            Assert.IsNotNull(returnedEncounter);
            Assert.AreEqual("Test Encounter", returnedEncounter.Description);
        }

        [TestMethod]
        public void GetRandomEncounterTest_Returns500_WhenGenerationFails() {
            _mockGeneratorService.Setup(s => s.GenerateRandomEncounter()).Returns((RandomEncounter)null);

            var result = _controller.GetRandomEncounter();

            var statusResult = result.Result as ObjectResult;
            Assert.IsNotNull(statusResult);
            Assert.AreEqual(500, statusResult.StatusCode);
        }

        [TestMethod]
        public void GetRandomNPCsTest() {
            int count = 10;
            var mockNPCs = new List<NPC> {
                new NPC { Name = "NPC 1" },
                new NPC { Name = "NPC 2" }
            };
            _mockGeneratorService.Setup(s => s.GenerateRandomNPCs(count)).Returns(mockNPCs);

            var result = _controller.GetRandomNPCs(count);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var returnedNPCs = okResult.Value as List<NPC>;
            Assert.IsNotNull(returnedNPCs);
            Assert.AreEqual(2, returnedNPCs.Count);
        }

        [TestMethod]
        public void GetRandomNPCsTest_ReturnsBadRequest_ForInvalidCount() {
            int invalidCount = 0;

            var result = _controller.GetRandomNPCs(invalidCount);

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Count must be between 1 and 1000", badRequestResult.Value);
        }

        [TestMethod]
        public void GetRandomMonstersTest() {
            int count = 2;
            var mockMonsters = new List<Monster> {
                new Monster { Species = "Orc", Level = 5 },
                new Monster { Species = "Troll", Level = 8 }
            };
            _mockGeneratorService.Setup(s => s.GenerateRandomMonsters(count)).Returns(mockMonsters);

            var result = _controller.GetRandomMonsters(count);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var returnedMonsters = okResult.Value as List<Monster>;
            Assert.IsNotNull(returnedMonsters);
            Assert.AreEqual(2, returnedMonsters.Count);
            Assert.AreEqual("Orc", returnedMonsters[0].Species);
            Assert.AreEqual(8, returnedMonsters[1].Level);
        }

        [TestMethod]
        public void GetRandomMonstersTest_ReturnsBadRequest_ForInvalidCount() {
            int invalidCount = 2000;

            var result = _controller.GetRandomMonsters(invalidCount);

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Count must be between 1 and 1000", badRequestResult.Value);
        }
    }
}