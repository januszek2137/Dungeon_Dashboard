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

namespace Dungeon_Dashboard.PlayerCharacters.Controllers
{

    [Authorize]
    public class CharacterModelsController : Controller
    {
        private readonly ICharacterModelService _characterModelService;
        private readonly ILogger<CharacterModelsController> _logger;


        public CharacterModelsController(ICharacterModelService characterModelService, ILogger<CharacterModelsController> logger)
        {
            _characterModelService = characterModelService;
            _logger = logger;
        }

        // GET: CharacterModels
        public async Task<IActionResult> Index()
        {
            var username = User.Identity.Name.Split("@")[0];
            return View(await _characterModelService.GetAllCharactersForUserAsync(username));
        }

        // GET: CharacterModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var username = User.Identity.Name.Split('@')[0];
                var characterModel = await _characterModelService.GetCharacterByIdIfUserHasAccessAsync(id, username);
                return View(characterModel);
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is ArgumentException)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
                return Forbid();
            }
        }

        // GET: CharacterModels/Create
        public IActionResult Create()
        {
            ViewBag.ClassList = _characterModelService.GetClasses()
                                    .Select(c => new SelectListItem
                                    {
                                        Text = c.ToString(),
                                        Value = ((int)c).ToString()
                                    }).ToList();


            ViewBag.RaceList = _characterModelService.GetRaces()
                                   .Select(r => new SelectListItem
                                   {
                                       Text = r.ToString(),
                                       Value = ((int)r).ToString()
                                   }).ToList();

            return View();
        }

        // POST: CharacterModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Class,Race,Level,Speed,ArmorClass,HitPoints,Strength,Dexterity,Constitution,Intelligence,Wisdom,Charisma,Skills,Equipment,Inventory,Copper,Silver,Electrum,Gold,Platinum")] CharacterModel characterModel)
        {

            characterModel.Skills = characterModel.Skills?
                .Where(skill => !string.IsNullOrWhiteSpace(skill))
                .ToArray();

            var username = User.Identity.Name.Split('@')[0];
            if (username != null)
            {
                characterModel.CreatedBy = username;
            }

            if (ModelState.IsValid)
            {
                await _characterModelService.AddCharacterAsync(characterModel);
                return RedirectToAction(nameof(Index));
            }
            return View(characterModel);
        }

        // GET: CharacterModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var username = User.Identity.Name.Split('@')[0];
                var characterModel = await _characterModelService.GetCharacterByIdIfUserHasAccessAsync(id, username);


                ViewBag.ClassList = _characterModelService.GetClasses()
                                        .Select(c => new SelectListItem
                                        {
                                            Text = c.ToString(),
                                            Value = ((int)c).ToString()
                                        }).ToList();


                ViewBag.RaceList = _characterModelService.GetRaces()
                                       .Select(r => new SelectListItem
                                       {
                                           Text = r.ToString(),
                                           Value = ((int)r).ToString()
                                       }).ToList();

                return View(characterModel);
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is ArgumentException)
            {
                return NotFound();
            }
        }

        // POST: CharacterModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Class,Race,Level,Speed,ArmorClass,HitPoints,Strength,Dexterity,Constitution,Intelligence,Wisdom,Charisma,Skills,Equipment,Inventory,Copper,Silver,Electrum,Gold,Platinum")] CharacterModel characterModel)
        {
            var username = User.Identity.Name.Split('@')[0];
            if (username != null)
            {
                characterModel.CreatedBy = username;
            }

            if (id != characterModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _characterModelService.UpdateAsync(id, characterModel);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Conflict("Another user updated this record. Please reload and try again.");
                }
                return RedirectToAction(nameof(Index));
            }
            return View(characterModel);
        }

        // GET: CharacterModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                var username = User.Identity.Name.Split('@')[0];
                var characterModel = await _characterModelService.GetCharacterByIdIfUserHasAccessAsync(id, username);
                return View(characterModel);
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is ArgumentException)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
                return Forbid();
            }
        }

        // POST: CharacterModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var username = User.Identity.Name.Split('@')[0];
                await _characterModelService.DeleteIfUserHasAccessAsync(id, username);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is ArgumentException)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
                return Forbid();
            }
        }

        public async Task<IActionResult> GenerateCharacterPdf(int id)
        {
            var username = User.Identity.Name.Split('@')[0];

            var character = await _characterModelService.GetCharacterByIdIfUserHasAccessAsync(id, username);
            _logger.LogInformation("User {Username} requested PDF generation for character {CharacterName}", username, character?.Name);

            if (character == null)
                return NotFound();

            try
            {
                var pdfBytes = _characterModelService.GenerateCharacterPdf(character);

                _logger.LogInformation("PDF generated for character {CharacterName} by user {Username}", character.Name, username);
                return File(pdfBytes, "application/pdf", $"CharacterSheet_{id}.pdf");
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(ex, "Template file not found while generating PDF for character {CharacterName}", character.Name);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF for character {CharacterName}, error: {ErrorMessage}", character.Name, ex.Message);
                return StatusCode(500, "Internal server error while generating PDF.");
            }




            //var username = User.Identity.Name.Split('@')[0];
            //var characterModel = await _characterModelService.GetCharacterByIdIfUserHasAccessAsync(id, username);
            //if (characterModel == null)
            //{
            //    return NotFound();
            //}

            //var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs", "CharacterSheetTemplate.pdf");

            //if (!System.IO.File.Exists(templatePath))
            //{
            //    return NotFound($"Template file not found: {templatePath}");
            //}

            //try
            //{
            //    byte[] pdfBytes;

            //    using (var memoryStream = new MemoryStream())
            //    {
            //        using (var reader = new PdfReader(templatePath))
            //        {
            //            using (var pdfWriter = new PdfWriter(memoryStream))
            //            {
            //                using (var pdfDocument = new PdfDocument(reader, pdfWriter))
            //                {
            //                    var form = PdfAcroForm.GetAcroForm(pdfDocument, true);
            //                    var font = PdfFontFactory.CreateFont("Helvetica");
            //                    var StatCounter = new CharacterStatCounter();

            //                    int proficiencyBonus = StatCounter.CalculateProficiencyBonus(characterModel.Level);

            //                    string strMod = StatCounter.CalculateStatModifier(characterModel.Strength).ToString();
            //                    string dexMod = StatCounter.CalculateStatModifier(characterModel.Dexterity).ToString();
            //                    string conMod = StatCounter.CalculateStatModifier(characterModel.Constitution).ToString();
            //                    string intMod = StatCounter.CalculateStatModifier(characterModel.Intelligence).ToString();
            //                    string wisMod = StatCounter.CalculateStatModifier(characterModel.Wisdom).ToString();
            //                    string chaMod = StatCounter.CalculateStatModifier(characterModel.Charisma).ToString();
            //                    string passiveWisdom = StatCounter.CalculatePassiveWisdom(characterModel.Wisdom).ToString();

            //                    string skills = characterModel.Skills?.Where(skill => !string.IsNullOrWhiteSpace(skill)).Any() == true
            //                    ? string.Join(", ", characterModel.Skills
            //                            .Where(skill => !string.IsNullOrWhiteSpace(skill))
            //                            .Select(skill => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(skill.ToLower())))
            //                    : string.Empty;

            //                    string equipment = characterModel.Equipment?.Where(item => !string.IsNullOrWhiteSpace(item)).Any() == true
            //                    ? string.Join(", ", characterModel.Equipment
            //                            .Where(item => !string.IsNullOrWhiteSpace(item))
            //                            .Select(item => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.ToLower())))
            //                    : string.Empty;

            //                    string inventory = characterModel.Inventory?.Where(item => !string.IsNullOrWhiteSpace(item)).Any() == true
            //                    ? string.Join(", ", characterModel.Inventory
            //                        .Where(item => !string.IsNullOrWhiteSpace(item))
            //                        .Select(item => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.ToLower())))
            //                    : string.Empty;

            //                    form.GetField("CharacterName")?.SetValue(characterModel.Name).SetFont(font);
            //                    form.GetField("ClassLevel")?.SetValue($"{characterModel.Class} {characterModel.Level}").SetFont(font);

            //                    form.GetField("Race ")?.SetValue(characterModel.Race.ToString()).SetFont(font);
            //                    form.GetField("Speed")?.SetValue(characterModel.Speed.ToString()).SetFont(font);
            //                    form.GetField("AC")?.SetValue(characterModel.ArmorClass.ToString()).SetFont(font);

            //                    form.GetField("STR")?.SetValue(characterModel.Strength.ToString()).SetFont(font);
            //                    form.GetField("DEX")?.SetValue(characterModel.Dexterity.ToString()).SetFont(font);
            //                    form.GetField("CON")?.SetValue(characterModel.Constitution.ToString()).SetFont(font);
            //                    form.GetField("INT")?.SetValue(characterModel.Intelligence.ToString()).SetFont(font);
            //                    form.GetField("WIS")?.SetValue(characterModel.Wisdom.ToString()).SetFont(font);
            //                    form.GetField("CHA")?.SetValue(characterModel.Charisma.ToString()).SetFont(font);

            //                    form.GetField("STRmod")?.SetValue(strMod);
            //                    form.GetField("DEXmod ")?.SetValue(dexMod);
            //                    form.GetField("CONmod")?.SetValue(conMod);
            //                    form.GetField("INTmod")?.SetValue(intMod);
            //                    form.GetField("WISmod")?.SetValue(wisMod);
            //                    form.GetField("CHamod")?.SetValue(chaMod);

            //                    form.GetField("ProfBonus")?.SetValue(proficiencyBonus.ToString()).SetFont(font);

            //                    form.GetField("Passive")?.SetValue(passiveWisdom).SetFont(font);
            //                    form.GetField("ST Strength")?.SetValue(strMod).SetFont(font);
            //                    form.GetField("ST Dexterity")?.SetValue(dexMod).SetFont(font);
            //                    form.GetField("ST Constitution")?.SetValue(conMod).SetFont(font);
            //                    form.GetField("ST Intelligence")?.SetValue(intMod).SetFont(font);
            //                    form.GetField("ST Wisdom")?.SetValue(wisMod).SetFont(font);
            //                    form.GetField("ST Charisma")?.SetValue(chaMod).SetFont(font);
            //                    form.GetField("HPMax")?.SetValue(characterModel.HitPoints.ToString()).SetFont(font);
            //
            //                    form.GetField("Acrobatics")?.SetValue(dexMod).SetFont(font);
            //                    form.GetField("Animal")?.SetValue(wisMod).SetFont(font);
            //                    form.GetField("Arcana")?.SetValue(intMod).SetFont(font);
            //                    form.GetField("Athletics")?.SetValue(strMod).SetFont(font);
            //                    form.GetField("Deception ")?.SetValue(chaMod).SetFont(font);

            //                    form.GetField("History ")?.SetValue(intMod).SetFont(font);
            //                    form.GetField("Insight")?.SetValue(wisMod).SetFont(font);
            //                    form.GetField("Intimidation")?.SetValue(chaMod).SetFont(font);
            //                    form.GetField("Investigation ")?.SetValue(intMod).SetFont(font);
            //                    form.GetField("Medicine")?.SetValue(wisMod).SetFont(font);
            //                    form.GetField("Nature")?.SetValue(intMod).SetFont(font);
            //                    form.GetField("Perception ")?.SetValue(wisMod).SetFont(font);
            //                    form.GetField("Performance")?.SetValue(chaMod).SetFont(font);
            //                    form.GetField("Persuasion")?.SetValue(chaMod).SetFont(font);
            //                    form.GetField("Religion")?.SetValue(intMod).SetFont(font);
            //                    form.GetField("SleightofHand")?.SetValue(dexMod).SetFont(font);
            //                    form.GetField("Stealth ")?.SetValue(dexMod).SetFont(font);
            //                    form.GetField("Survival")?.SetValue(wisMod).SetFont(font);

            //                    form.GetField("ProficienciesLang")?.SetValue(skills).SetFont(font);
            //                    form.GetField("Equipment")?.SetValue(equipment).SetFont(font);

            //                    form.GetField("CP")?.SetValue(characterModel.Copper.ToString()).SetFont(font);
            //                    form.GetField("SP")?.SetValue(characterModel.Silver.ToString()).SetFont(font);
            //                    form.GetField("EP")?.SetValue(characterModel.Electrum.ToString()).SetFont(font);
            //                    form.GetField("GP")?.SetValue(characterModel.Gold.ToString()).SetFont(font);
            //                    form.GetField("PP")?.SetValue(characterModel.Platinum.ToString()).SetFont(font);

            //                    form.FlattenFields();
            //                }
            //            }
            //        }

            //        pdfBytes = memoryStream.ToArray();
            //    }

            //    return File(pdfBytes, "application/pdf", $"CharacterSheet_{id}.pdf");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Error generating PDF: {ex.Message}");
            //    return StatusCode(500, $"Internal server error while generating PDF. {ex.Message}");
            //}
        }
    }
}