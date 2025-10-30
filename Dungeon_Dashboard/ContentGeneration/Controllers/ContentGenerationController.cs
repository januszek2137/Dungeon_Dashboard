using Dungeon_Dashboard.ContentGeneration.Models;
using Dungeon_Dashboard.ContentGeneration.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Dungeon_Dashboard.ContentGeneration.Controllers {
    [Authorize]
    public class ContentGenerationController : Controller {
        private readonly IContentGenerationService _generationService;

        public ContentGenerationController(IContentGenerationService generationService) {
            _generationService = generationService;
        }

        public IActionResult Index() {
            return View(new ContentGenerationViewModel());
        }


        [HttpPost]
        public async Task<IActionResult> GetRandomNPC() =>
            await GenerateAndReturnViewAsync(_generationService.GenerateRandomNPC);

        [HttpPost]
        public async Task<IActionResult> GetRandomMonster() =>
            await GenerateAndReturnViewAsync(_generationService.GenerateRandomMonster);

        [HttpPost]
        public async Task<IActionResult> GetRandomEncounter() =>
            await GenerateAndReturnViewAsync(_generationService.GenerateRandomEncounter);

        private async Task<IActionResult> GenerateAndReturnViewAsync<T>(Func<Task<T>> generator) {
            try {
                var model     = await generator();
                var viewModel = new ContentGenerationViewModel();

                switch (model) {
                    case NPC npc:
                        viewModel.Npc       = npc;
                        viewModel.ModelType = "NPC";
                        break;
                    case Monster monster:
                        viewModel.Monster   = monster;
                        viewModel.ModelType = "Monster";
                        break;
                    case RandomEncounter encounter:
                        viewModel.Encounter = encounter;
                        viewModel.ModelType = "Encounter";
                        break;
                    default:
                        throw new GenerationFailedException($"Unsupported model type generated {typeof(T).Name}");
                }

                return View("Index", viewModel);
            }
            catch (GenerationFailedException ex) {
                var errorViewModel = new ContentGenerationViewModel {
                    ErrorMessage = ex.Message,
                    ModelType    = "Error"
                };
                return View("Index", errorViewModel);
            }
        }
    }
}