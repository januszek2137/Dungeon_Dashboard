using Dungeon_Dashboard.ContentGeneration.Models;
using Dungeon_Dashboard.ContentGeneration.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Dungeon_Dashboard.ContentGeneration.Controllers {

    [Authorize]
    public class CharacterGeneratorController : Controller {
        private readonly IContentGenerationService _generationService;

        public CharacterGeneratorController(IContentGenerationService generationService) {
            _generationService = generationService;
        }

        public IActionResult Index() {
            return View();
        }

        private async Task<IActionResult> GenerateAndReturnViewAsync<T>(Func<Task<T>> generator) {
            try {
                var model = await generator();
                return View("Index", model);
            }
            catch (GenerationFailedException ex) {
                return View("Index", ex);
            }
        }

        [HttpPost]
        public Task<IActionResult> GetRandomNPC() =>
            GenerateAndReturnViewAsync(_generationService.GenerateRandomNPC);

        [HttpPost]
        public Task<IActionResult> GetRandomMonster() =>
            GenerateAndReturnViewAsync(_generationService.GenerateRandomMonster);

        [HttpPost]
        public Task<IActionResult> GetRandomEncounter() =>
            GenerateAndReturnViewAsync(_generationService.GenerateRandomEncounter);

    }
}