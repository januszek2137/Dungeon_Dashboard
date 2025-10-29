using Dungeon_Dashboard.Home.Data;
using Dungeon_Dashboard.PlayerCharacters.Models;
using iText.Forms;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Dungeon_Dashboard.PlayerCharacters.Services {
    public interface ICharacterModelService {
        Task<List<CharacterModel>> GetAllCharactersForUserAsync(string username);
        Task<CharacterModel>       GetCharacterByIdIfUserHasAccessAsync(int? id, string username);
        Task<CharacterModel>       AddCharacterAsync(CharacterModel character);
        Task<CharacterModel>       UpdateAsync(int id, CharacterModel character);
        Task<CharacterModel>       DeleteIfUserHasAccessAsync(int id, string username);
        byte[]                     GenerateCharacterPdf(CharacterModel characterModel);
        List<Classes>              GetClasses();
        List<Races>                GetRaces();
    }

    public class CharacterModelService : ICharacterModelService {
        private readonly AppDBContext                   _context;
        private readonly CharacterStatCounter           _statCounter;
        private readonly string                         _templatePath;
        private readonly ILogger<CharacterModelService> _logger;

        public CharacterModelService(AppDBContext context, ILogger<CharacterModelService> logger) {
            _context     = context;
            _statCounter = new CharacterStatCounter();
            _templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs",
                "CharacterSheetTemplate.pdf");
            _logger = logger;
        }

        public async Task<List<CharacterModel>> GetAllCharactersForUserAsync(string username) {
            return await _context.CharacterModel.Where(c => c.CreatedBy == username).ToListAsync();
        }

        public async Task<CharacterModel> GetCharacterByIdIfUserHasAccessAsync(int? id, string username) {
            if (!id.HasValue)
                throw new ArgumentException("Character ID must be provided", nameof(id));

            var character = await _context.CharacterModel
                .SingleOrDefaultAsync(c => c.Id == id.Value && c.CreatedBy == username);

            if (character == null)
                throw new KeyNotFoundException("Character not found or access denied");

            return character;
        }

        public async Task<CharacterModel> AddCharacterAsync(CharacterModel character) {
            _context.CharacterModel.Add(character);
            await _context.SaveChangesAsync();
            return character;
        }

        public async Task<CharacterModel> UpdateAsync(int id, CharacterModel character) {
            if (id != character.Id)
                throw new ArgumentException("Character ID mismatch");

            _context.Entry(character).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return character;
        }

        public async Task<CharacterModel> DeleteIfUserHasAccessAsync(int id, string username) {
            var character = await GetCharacterByIdIfUserHasAccessAsync(id, username);

            if (character == null)
                throw new KeyNotFoundException("Character not found");

            _context.CharacterModel.Remove(character);
            await _context.SaveChangesAsync();
            return character;
        }

        public byte[] GenerateCharacterPdf(CharacterModel characterModel) {
            if (!File.Exists(_templatePath))
                throw new FileNotFoundException($"Template not found: {_templatePath}");

            var pdfFields = ToPdfFields(characterModel);

            using (var ms = new MemoryStream()) {
                FillCharacterPdf(ms, pdfFields);

                _logger.LogInformation("PDF generated for character {CharacterName}", characterModel.Name);

                return ms.ToArray();
            }
        }

        private void FillCharacterPdf(Stream output, Dictionary<string, string> stats) {
            using (var reader = new PdfReader(_templatePath)) {
                using (var writer = new PdfWriter(output)) {
                    using (var pdf = new PdfDocument(reader, writer)) {
                        var form = PdfAcroForm.GetAcroForm(pdf, true);
                        var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                        foreach (var field in stats) {
                            form.GetField(field.Key)?.SetValue(field.Value).SetFont(font);
                        }

                        form.FlattenFields();

                        _logger.LogInformation("PDF generation completed successfully.");
                    }
                }
            }
        }

        private Dictionary<string, string> ToPdfFields(CharacterModel character) {
            var proficiency = _statCounter.CalculateProficiencyBonus(character.Level);
            var mods = new {
                STR = _statCounter.CalculateStatModifier(character.Strength),
                DEX = _statCounter.CalculateStatModifier(character.Dexterity),
                CON = _statCounter.CalculateStatModifier(character.Constitution),
                INT = _statCounter.CalculateStatModifier(character.Intelligence),
                WIS = _statCounter.CalculateStatModifier(character.Wisdom),
                CHA = _statCounter.CalculateStatModifier(character.Charisma)
            };

            string FormatList(IEnumerable<string>? items) {
                if (items == null) return string.Empty;
                var valid = items.Where(i => !string.IsNullOrWhiteSpace(i));
                return valid.Any()
                    ? string.Join(", ", valid.Select(i =>
                        CultureInfo.CurrentCulture.TextInfo.ToTitleCase(i.ToLower())))
                    : string.Empty;
            }

            string skills    = FormatList(character.Skills);
            string equipment = FormatList(character.Equipment);
            string inventory = FormatList(character.Inventory);

            var passiveWisdom = _statCounter.CalculatePassiveWisdom(character.Wisdom);

            return new Dictionary<string, string> {
                ["CharacterName"] = character.Name,
                ["ClassLevel"]    = $"{character.Class} {character.Level}",

                ["Race "] = character.Race.ToString(),
                ["AC"]    = character.ArmorClass.ToString(),
                ["Speed"] = character.Speed.ToString(),

                ["STR"] = character.Strength.ToString(),
                ["DEX"] = character.Dexterity.ToString(),
                ["CON"] = character.Constitution.ToString(),
                ["INT"] = character.Intelligence.ToString(),
                ["WIS"] = character.Wisdom.ToString(),
                ["CHA"] = character.Charisma.ToString(),

                ["STRmod"]  = mods.STR.ToString(),
                ["DEXmod "] = mods.DEX.ToString(),
                ["CONmod"]  = mods.CON.ToString(),
                ["INTmod"]  = mods.INT.ToString(),
                ["WISmod"]  = mods.WIS.ToString(),
                ["CHamod"]  = mods.CHA.ToString(),

                ["ProfBonus"] = proficiency.ToString(),

                ["Passive"]         = passiveWisdom.ToString(),
                ["ST Strength"]     = mods.STR.ToString(),
                ["ST Dexterity"]    = mods.DEX.ToString(),
                ["ST Constitution"] = mods.CON.ToString(),
                ["ST Intelligence"] = mods.INT.ToString(),
                ["ST Wisdom"]       = mods.WIS.ToString(),
                ["ST Charisma"]     = mods.CHA.ToString(),
                ["HPMax"]           = character.HitPoints.ToString(),

                ["Acrobatics"]  = mods.DEX.ToString(),
                ["Animal"]      = mods.WIS.ToString(),
                ["Athletics"]   = mods.STR.ToString(),
                ["Arcana"]      = mods.INT.ToString(),
                ["Perception "] = mods.WIS.ToString(),
                ["Deception "]  = mods.CHA.ToString(),
                ["Persuasion"]  = mods.CHA.ToString(),

                ["History "]       = mods.INT.ToString(),
                ["Insight"]        = mods.WIS.ToString(),
                ["Intimidation"]   = mods.CHA.ToString(),
                ["Investigation "] = mods.INT.ToString(),
                ["Medicine"]       = mods.WIS.ToString(),
                ["Nature"]         = mods.INT.ToString(),
                ["Performance"]    = mods.CHA.ToString(),
                ["Religion"]       = mods.INT.ToString(),
                ["SleightofHand"]  = mods.DEX.ToString(),
                ["Stealth "]       = mods.DEX.ToString(),
                ["Survival"]       = mods.WIS.ToString(),

                ["CP"] = character.Copper.ToString(),
                ["SP"] = character.Silver.ToString(),
                ["EP"] = character.Electrum.ToString(),
                ["GP"] = character.Gold.ToString(),
                ["PP"] = character.Platinum.ToString(),

                ["ProficienciesLang"] = skills,
                ["Equipment"]         = equipment
            };
        }

        public List<Classes> GetClasses() {
            return Enum.GetValues(typeof(Classes))
                .Cast<Classes>()
                .ToList();
        }

        public List<Races> GetRaces() {
            return Enum.GetValues(typeof(Races))
                .Cast<Races>()
                .ToList();
        }
    }
}