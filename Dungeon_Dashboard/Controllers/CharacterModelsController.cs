using Dungeon_Dashboard.Models;
using iText.Forms;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Dungeon_Dashboard.Controllers {

    public class CharacterModelsController : Controller {
        private readonly AppDBContext _context;

        public CharacterModelsController(AppDBContext context) {
            _context = context;
        }

        // GET: CharacterModels
        [Authorize]
        public async Task<IActionResult> Index() {
            return View(await _context.CharacterModel.ToListAsync());
        }

        // GET: CharacterModels/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id) {
            if(id == null) {
                return NotFound();
            }

            var characterModel = await _context.CharacterModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if(characterModel == null) {
                return NotFound();
            }

            return View(characterModel);
        }

        // GET: CharacterModels/Create
        [Authorize]
        public IActionResult Create() {
            ViewBag.ClassList = Enum.GetValues(typeof(Classes))
                           .Cast<Classes>()
                           .Select(c => new SelectListItem {
                               Text = c.ToString(),
                               Value = ((int)c).ToString()
                           }).ToList();

            ViewBag.RaceList = Enum.GetValues(typeof(Races))
                                   .Cast<Races>()
                                   .Select(r => new SelectListItem {
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
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Name,Class,Race,Level,Speed,ArmorClass,HitPoints,Strength,Dexterity,Constitution,Intelligence,Wisdom,Charisma,Skills,Equipment,Inventory,Copper,Silver,Electrum,Gold,Platinum")] CharacterModel characterModel) {
            ViewBag.ClassList = Enum.GetValues(typeof(Classes))
                            .Cast<Classes>()
                            .Select(c => new SelectListItem {
                                Text = c.ToString(),
                                Value = ((int)c).ToString()
                            }).ToList();

            ViewBag.RaceList = Enum.GetValues(typeof(Races))
                                   .Cast<Races>()
                                   .Select(r => new SelectListItem {
                                       Text = r.ToString(),
                                       Value = ((int)r).ToString()
                                   }).ToList();

            characterModel.Skills = characterModel.Skills?
            .Where(skill => !string.IsNullOrWhiteSpace(skill))
             .ToArray();

            var username = User.Identity.Name.Split('@')[0];
            if(username != null) {
                characterModel.CreatedBy = username;
            }

            if(ModelState.IsValid) {
                _context.Add(characterModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if(!ModelState.IsValid) {
                ViewBag.ValidationErrors = ModelState.Values.SelectMany(v => v.Errors)
                                                .Select(e => e.ErrorMessage)
                                                .ToList();
                return View(characterModel);
            }
            return View(characterModel);
        }

        // GET: CharacterModels/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id) {
            ViewBag.ClassList = Enum.GetValues(typeof(Classes))
                           .Cast<Classes>()
                           .Select(c => new SelectListItem {
                               Text = c.ToString(),
                               Value = ((int)c).ToString()
                           }).ToList();

            ViewBag.RaceList = Enum.GetValues(typeof(Races))
                                   .Cast<Races>()
                                   .Select(r => new SelectListItem {
                                       Text = r.ToString(),
                                       Value = ((int)r).ToString()
                                   }).ToList();

            if(id == null) {
                return NotFound();
            }

            var characterModel = await _context.CharacterModel.FindAsync(id);
            if(characterModel == null) {
                return NotFound();
            }
            return View(characterModel);
        }

        // POST: CharacterModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Class,Race,Level,Speed,ArmorClass,HitPoints,Strength,Dexterity,Constitution,Intelligence,Wisdom,Charisma,Skills,Equipment,Inventory,Copper,Silver,Electrum,Gold,Platinum")] CharacterModel characterModel) {
            var username = User.Identity.Name.Split('@')[0];
            if(username != null) {
                characterModel.CreatedBy = username;
            }

            ViewBag.ClassList = Enum.GetValues(typeof(Classes))
                           .Cast<Classes>()
                           .Select(c => new SelectListItem {
                               Text = c.ToString(),
                               Value = ((int)c).ToString()
                           }).ToList();

            ViewBag.RaceList = Enum.GetValues(typeof(Races))
                                   .Cast<Races>()
                                   .Select(r => new SelectListItem {
                                       Text = r.ToString(),
                                       Value = ((int)r).ToString()
                                   }).ToList();

            if(id != characterModel.Id) {
                return NotFound();
            }

            if(ModelState.IsValid) {
                try {
                    _context.Update(characterModel);
                    await _context.SaveChangesAsync();
                } catch(DbUpdateConcurrencyException) {
                    if(!CharacterModelExists(characterModel.Id)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(characterModel);
        }

        // GET: CharacterModels/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id) {
            if(id == null) {
                return NotFound();
            }

            var characterModel = await _context.CharacterModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if(characterModel == null) {
                return NotFound();
            }

            return View(characterModel);
        }

        // POST: CharacterModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var characterModel = await _context.CharacterModel.FindAsync(id);
            if(characterModel != null) {
                _context.CharacterModel.Remove(characterModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CharacterModelExists(int id) {
            return _context.CharacterModel.Any(e => e.Id == id);
        }

        public async Task<IActionResult> GenerateCharacterPdf(int id) {
            var characterModel = await _context.CharacterModel.FindAsync(id);
            if(characterModel == null) {
                return NotFound();
            }

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs", "CharacterSheetTemplate.pdf");

            if(!System.IO.File.Exists(templatePath)) {
                return NotFound($"Template file not found: {templatePath}");
            }

            try {
                byte[] pdfBytes;

                using(var memoryStream = new MemoryStream()) {
                    using(var reader = new PdfReader(templatePath)) {
                        using(var pdfWriter = new PdfWriter(memoryStream)) {
                            using(var pdfDocument = new PdfDocument(reader, pdfWriter)) {
                                var form = PdfAcroForm.GetAcroForm(pdfDocument, true);
                                var font = PdfFontFactory.CreateFont("Helvetica");
                                var StatCounter = new CharacterStatCounter();

                                int proficiencyBonus = StatCounter.CalculateProficiencyBonus(characterModel.Level);

                                string strMod = StatCounter.CalculateStatModifier(characterModel.Strength).ToString();
                                string dexMod = StatCounter.CalculateStatModifier(characterModel.Dexterity).ToString();
                                string conMod = StatCounter.CalculateStatModifier(characterModel.Constitution).ToString();
                                string intMod = StatCounter.CalculateStatModifier(characterModel.Intelligence).ToString();
                                string wisMod = StatCounter.CalculateStatModifier(characterModel.Wisdom).ToString();
                                string chaMod = StatCounter.CalculateStatModifier(characterModel.Charisma).ToString();
                                string passiveWisdom = StatCounter.CalculatePassiveWisdom(characterModel.Wisdom).ToString();

                                string skills = characterModel.Skills?.Where(skill => !string.IsNullOrWhiteSpace(skill)).Any() == true
                                ? string.Join(", ", characterModel.Skills
                                        .Where(skill => !string.IsNullOrWhiteSpace(skill))
                                        .Select(skill => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(skill.ToLower())))
                                : string.Empty;

                                string equipment = characterModel.Equipment?.Where(item => !string.IsNullOrWhiteSpace(item)).Any() == true
                                ? string.Join(", ", characterModel.Equipment
                                        .Where(item => !string.IsNullOrWhiteSpace(item))
                                        .Select(item => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.ToLower())))
                                : string.Empty;

                                string inventory = characterModel.Inventory?.Where(item => !string.IsNullOrWhiteSpace(item)).Any() == true
                                ? string.Join(", ", characterModel.Inventory
                                    .Where(item => !string.IsNullOrWhiteSpace(item))
                                    .Select(item => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.ToLower())))
                                : string.Empty;

                                form.GetField("CharacterName")?.SetValue(characterModel.Name).SetFont(font);
                                form.GetField("ClassLevel")?.SetValue($"{characterModel.Class} {characterModel.Level}").SetFont(font);
                                form.GetField("Race ")?.SetValue(characterModel.Race.ToString()).SetFont(font);
                                form.GetField("Speed")?.SetValue(characterModel.Speed.ToString()).SetFont(font);
                                form.GetField("AC")?.SetValue(characterModel.ArmorClass.ToString()).SetFont(font);
                                form.GetField("STR")?.SetValue(characterModel.Strength.ToString()).SetFont(font);
                                form.GetField("DEX")?.SetValue(characterModel.Dexterity.ToString()).SetFont(font);
                                form.GetField("CON")?.SetValue(characterModel.Constitution.ToString()).SetFont(font);
                                form.GetField("INT")?.SetValue(characterModel.Intelligence.ToString()).SetFont(font);
                                form.GetField("WIS")?.SetValue(characterModel.Wisdom.ToString()).SetFont(font);
                                form.GetField("CHA")?.SetValue(characterModel.Charisma.ToString()).SetFont(font);
                                form.GetField("STRmod")?.SetValue(strMod);
                                form.GetField("DEXmod ")?.SetValue(dexMod);
                                form.GetField("CONmod")?.SetValue(conMod);
                                form.GetField("INTmod")?.SetValue(intMod);
                                form.GetField("WISmod")?.SetValue(wisMod);
                                form.GetField("CHamod")?.SetValue(chaMod);
                                form.GetField("ProfBonus")?.SetValue(proficiencyBonus.ToString()).SetFont(font);
                                form.GetField("Passive")?.SetValue(passiveWisdom).SetFont(font);
                                form.GetField("ST Strength")?.SetValue(strMod).SetFont(font);
                                form.GetField("ST Dexterity")?.SetValue(dexMod).SetFont(font);
                                form.GetField("ST Constitution")?.SetValue(conMod).SetFont(font);
                                form.GetField("ST Intelligence")?.SetValue(intMod).SetFont(font);
                                form.GetField("ST Wisdom")?.SetValue(wisMod).SetFont(font);
                                form.GetField("ST Charisma")?.SetValue(chaMod).SetFont(font);
                                form.GetField("HPMax")?.SetValue(characterModel.HitPoints.ToString()).SetFont(font);
                                form.GetField("Acrobatics")?.SetValue(dexMod).SetFont(font);
                                form.GetField("Animal")?.SetValue(wisMod).SetFont(font);
                                form.GetField("Arcana")?.SetValue(intMod).SetFont(font);
                                form.GetField("Athletics")?.SetValue(strMod).SetFont(font);
                                form.GetField("Deception ")?.SetValue(chaMod).SetFont(font);
                                form.GetField("History ")?.SetValue(intMod).SetFont(font);
                                form.GetField("Insight")?.SetValue(wisMod).SetFont(font);
                                form.GetField("Intimidation")?.SetValue(chaMod).SetFont(font);
                                form.GetField("Investigation ")?.SetValue(intMod).SetFont(font);
                                form.GetField("Medicine")?.SetValue(wisMod).SetFont(font);
                                form.GetField("Nature")?.SetValue(intMod).SetFont(font);
                                form.GetField("Perception ")?.SetValue(wisMod).SetFont(font);
                                form.GetField("Performance")?.SetValue(chaMod).SetFont(font);
                                form.GetField("Persuasion")?.SetValue(chaMod).SetFont(font);
                                form.GetField("Religion")?.SetValue(intMod).SetFont(font);
                                form.GetField("SleightofHand")?.SetValue(dexMod).SetFont(font);
                                form.GetField("Stealth ")?.SetValue(dexMod).SetFont(font);
                                form.GetField("Survival")?.SetValue(wisMod).SetFont(font);
                                form.GetField("ProficienciesLang")?.SetValue(skills).SetFont(font);
                                form.GetField("Equipment")?.SetValue(equipment).SetFont(font);
                                form.GetField("CP")?.SetValue(characterModel.Copper.ToString()).SetFont(font);
                                form.GetField("SP")?.SetValue(characterModel.Silver.ToString()).SetFont(font);
                                form.GetField("EP")?.SetValue(characterModel.Electrum.ToString()).SetFont(font);
                                form.GetField("GP")?.SetValue(characterModel.Gold.ToString()).SetFont(font);
                                form.GetField("PP")?.SetValue(characterModel.Platinum.ToString()).SetFont(font);

                                form.FlattenFields();
                            }
                        }
                    }

                    pdfBytes = memoryStream.ToArray();
                }

                return File(pdfBytes, "application/pdf", $"CharacterSheet_{id}.pdf");
            } catch(Exception ex) {
                Console.WriteLine($"Error generating PDF: {ex.Message}");
                return StatusCode(500, $"Internal server error while generating PDF. {ex.Message}");
            }
        }
    }
}