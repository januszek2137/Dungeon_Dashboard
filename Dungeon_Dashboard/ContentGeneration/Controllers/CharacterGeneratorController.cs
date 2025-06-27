using Dungeon_Dashboard.ContentGeneration.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Dungeon_Dashboard.ContentGeneration.Controllers {

    [Authorize]
    [Route("CharacterGenerator")]
    public class CharacterGeneratorController : Controller {
        private readonly HttpClient _client;

        public CharacterGeneratorController(IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor) {
            _client = httpClientFactory.CreateClient();

            var baseAddress = $"{contextAccessor.HttpContext.Request.Scheme}://{contextAccessor.HttpContext.Request.Host}";
            _client.BaseAddress = new Uri(baseAddress);
        }

        public IActionResult Index() {
            return View();
        }

        [HttpGet("GetRandomNPC")]
        public async Task<IActionResult> GetRandomNPC() {
            var response = await _client.GetAsync("/api/randomcharacters/npc");
            if(response.IsSuccessStatusCode) {
                var jsonString = await response.Content.ReadAsStringAsync();
                var npc = JsonConvert.DeserializeObject<NPC>(jsonString);
                return Json(npc);
            } else {
                return StatusCode((int)response.StatusCode, response.ReasonPhrase);
            }
        }

        [HttpGet("GetRandomMonster")]
        public async Task<IActionResult> GetRandomMonster() {
            var response = await _client.GetAsync("/api/randomcharacters/monster");
            if(response.IsSuccessStatusCode) {
                var jsonString = await response.Content.ReadAsStringAsync();
                var monster = JsonConvert.DeserializeObject<Monster>(jsonString);
                return Json(monster);
            } else {
                return StatusCode((int)response.StatusCode, response.ReasonPhrase);
            }
        }


        [HttpGet("GetRandomEncounter")]
        public async Task<IActionResult> GetRandomEncounter() {
            var response = await _client.GetAsync("/api/randomcharacters/encounter");

            if(response.IsSuccessStatusCode) {
                var jsonString = await response.Content.ReadAsStringAsync();
                var encounter = JsonConvert.DeserializeObject<RandomEncounter>(jsonString);
                return Json(encounter);
            } else {
                return StatusCode((int)response.StatusCode, response.ReasonPhrase);
            }
        }

        [HttpGet("GetNPCs")]
        public async Task<IActionResult> GetNPCs(int count = 10) {
            var response = await _client.GetAsync($"/api/randomcharacters/npcs?count={count}");

            if(response.IsSuccessStatusCode) {
                var jsonString = await response.Content.ReadAsStringAsync();
                var npcs = JsonConvert.DeserializeObject<List<NPC>>(jsonString);
                return Json(npcs);
            } else {
                return StatusCode((int)response.StatusCode, "Błąd podczas pobierania NPC-ów.");
            }
        }

        [HttpGet("GetMonsters")]
        public async Task<IActionResult> GetMonsters(int count = 100) {
            var response = await _client.GetAsync($"/api/randomcharacters/monsters?count={count}");

            if(response.IsSuccessStatusCode) {
                var jsonString = await response.Content.ReadAsStringAsync();
                var monsters = JsonConvert.DeserializeObject<List<Monster>>(jsonString);
                return Json(monsters);
            } else {
                return StatusCode((int)response.StatusCode, "Błąd podczas pobierania potworów.");
            }
        }
    }
}