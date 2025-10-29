using Dungeon_Dashboard.Home.Data;
using Dungeon_Dashboard.PlayerCharacters;
using Dungeon_Dashboard.PlayerCharacters.Models;
using Dungeon_Dashboard.PlayerCharacters.Services;
using iText.Forms;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Dungeon_Dashboard.PlayerCharacters.Controllers {
    [Authorize]
    public class CharacterModelsController : Controller {
        private readonly ICharacterModelService             _characterModelService;
        private readonly ILogger<CharacterModelsController> _logger;


        public CharacterModelsController(ICharacterModelService characterModelService,
            ILogger<CharacterModelsController> logger) {
            _characterModelService = characterModelService;
            _logger                = logger;
        }

        // GET: CharacterModels
        public async Task<IActionResult> Index() {
            var username = User.Identity.Name.Split("@")[0];
            return View(await _characterModelService.GetAllCharactersForUserAsync(username));
        }

        // GET: CharacterModels/Details/5
        public async Task<IActionResult> Details(int? id) {
            try {
                var username       = User.Identity.Name.Split('@')[0];
                var characterModel = await _characterModelService.GetCharacterByIdIfUserHasAccessAsync(id, username);
                return View(characterModel);
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is ArgumentException) {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
            catch (UnauthorizedAccessException ex) {
                Console.WriteLine(ex.Message);
                return Forbid();
            }
        }

        // GET: CharacterModels/Create
        public IActionResult Create() {
            ViewBag.ClassList = _characterModelService.GetClasses()
                .Select(c => new SelectListItem {
                    Text  = c.ToString(),
                    Value = ((int)c).ToString()
                }).ToList();


            ViewBag.RaceList = _characterModelService.GetRaces()
                .Select(r => new SelectListItem {
                    Text  = r.ToString(),
                    Value = ((int)r).ToString()
                }).ToList();

            return View();
        }

        // POST: CharacterModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "Id,Name,Class,Race,Level,Speed,ArmorClass,HitPoints,Strength,Dexterity,Constitution,Intelligence,Wisdom,Charisma,Skills,Equipment,Inventory,Copper,Silver,Electrum,Gold,Platinum")]
            CharacterModel characterModel) {
            characterModel.Skills = characterModel.Skills?
                .Where(skill => !string.IsNullOrWhiteSpace(skill))
                .ToArray();

            var username = User.Identity.Name.Split('@')[0];
            if (username != null) {
                characterModel.CreatedBy = username;
            }

            if (ModelState.IsValid) {
                await _characterModelService.AddCharacterAsync(characterModel);
                return RedirectToAction(nameof(Index));
            }

            return View(characterModel);
        }

        // GET: CharacterModels/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            try {
                var username       = User.Identity.Name.Split('@')[0];
                var characterModel = await _characterModelService.GetCharacterByIdIfUserHasAccessAsync(id, username);


                ViewBag.ClassList = _characterModelService.GetClasses()
                    .Select(c => new SelectListItem {
                        Text  = c.ToString(),
                        Value = ((int)c).ToString()
                    }).ToList();


                ViewBag.RaceList = _characterModelService.GetRaces()
                    .Select(r => new SelectListItem {
                        Text  = r.ToString(),
                        Value = ((int)r).ToString()
                    }).ToList();

                return View(characterModel);
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is ArgumentException) {
                return NotFound();
            }
        }

        // POST: CharacterModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind(
                "Id,Name,Class,Race,Level,Speed,ArmorClass,HitPoints,Strength,Dexterity,Constitution,Intelligence,Wisdom,Charisma,Skills,Equipment,Inventory,Copper,Silver,Electrum,Gold,Platinum")]
            CharacterModel characterModel) {
            var username = User.Identity.Name.Split('@')[0];
            if (username != null) {
                characterModel.CreatedBy = username;
            }

            if (id != characterModel.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    await _characterModelService.UpdateAsync(id, characterModel);
                }
                catch (DbUpdateConcurrencyException) {
                    return Conflict("Another user updated this record. Please reload and try again.");
                }

                return RedirectToAction(nameof(Index));
            }

            return View(characterModel);
        }

        // GET: CharacterModels/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            try {
                var username       = User.Identity.Name.Split('@')[0];
                var characterModel = await _characterModelService.GetCharacterByIdIfUserHasAccessAsync(id, username);
                return View(characterModel);
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is ArgumentException) {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
            catch (UnauthorizedAccessException ex) {
                Console.WriteLine(ex.Message);
                return Forbid();
            }
        }

        // POST: CharacterModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            try {
                var username = User.Identity.Name.Split('@')[0];
                await _characterModelService.DeleteIfUserHasAccessAsync(id, username);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is ArgumentException) {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
            catch (UnauthorizedAccessException ex) {
                Console.WriteLine(ex.Message);
                return Forbid();
            }
        }

        public async Task<IActionResult> GenerateCharacterPdf(int id) {
            var username = User.Identity.Name.Split('@')[0];

            var character = await _characterModelService.GetCharacterByIdIfUserHasAccessAsync(id, username);
            _logger.LogInformation("User {Username} requested PDF generation for character {CharacterName}", username,
                character?.Name);

            if (character == null)
                return NotFound();

            try {
                var pdfBytes = _characterModelService.GenerateCharacterPdf(character);

                _logger.LogInformation("PDF generated for character {CharacterName} by user {Username}", character.Name,
                    username);
                return File(pdfBytes, "application/pdf", $"CharacterSheet_{id}.pdf");
            }
            catch (FileNotFoundException ex) {
                _logger.LogError(ex, "Template file not found while generating PDF for character {CharacterName}",
                    character.Name);
                return NotFound(ex.Message);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error generating PDF for character {CharacterName}, error: {ErrorMessage}",
                    character.Name, ex.Message);
                return StatusCode(500, "Internal server error while generating PDF.");
            }
        }
    }
}