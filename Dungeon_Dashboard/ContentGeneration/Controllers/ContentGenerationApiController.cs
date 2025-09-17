using Dungeon_Dashboard.ContentGeneration.Models;
using Dungeon_Dashboard.ContentGeneration.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dungeon_Dashboard.ContentGeneration.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class ContentGenerationApiController : ControllerBase {
        private readonly IContentGenerationService _generatorService;

        public ContentGenerationApiController(IContentGenerationService generatorService) {
            _generatorService = generatorService;
        }

        [HttpGet("npc")]
        public ActionResult<NPC> GetRandomNPC() {
            var npc = _generatorService.GenerateRandomNPC();
            if(npc == null) {
                return StatusCode(500, "Failed to generate an NPC");
            }
            return Ok(npc);
        }

        [HttpGet("monster")]
        public ActionResult<Monster> GetRandomMonster() {
            var monster = _generatorService.GenerateRandomMonster();
            if(monster == null) {
                return StatusCode(500, "Failed to generate a monster");
            }
            return Ok(monster);
        }

        [HttpGet("encounter")]
        public ActionResult<RandomEncounter> GetRandomEncounter() {
            var encounter = _generatorService.GenerateRandomEncounter();
            if(encounter == null) {
                return StatusCode(500, "Failed to generate an encounter");
            }
            return Ok(encounter);
        }

        [HttpGet("npcs")]
        public ActionResult<List<NPC>> GetRandomNPCs([FromQuery] int count = 100) {
            if(count < 1 || count > 1000) {
                return BadRequest("Count must be between 1 and 1000");
            }

            var npcs = _generatorService.GenerateRandomNPCs(count);
            return Ok(npcs);
        }

        [HttpGet("monsters")]
        public ActionResult<List<Monster>> GetRandomMonsters([FromQuery] int count = 100) {
            if(count < 1 || count > 1000) {
                return BadRequest("Count must be between 1 and 1000");
            }

            var monsters = _generatorService.GenerateRandomMonsters(count);
            return Ok(monsters);
        }
    }
}