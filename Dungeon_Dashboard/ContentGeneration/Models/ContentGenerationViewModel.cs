using System.ComponentModel.DataAnnotations.Schema;
using Dungeon_Dashboard.ContentGeneration.Services;

namespace Dungeon_Dashboard.ContentGeneration.Models;

[NotMapped]
public class ContentGenerationViewModel {
    public string           ModelType { get; set; } = string.Empty;
    public NPC?             Npc       { get; set; }
    public Monster?         Monster   { get; set; }
    public RandomEncounter? Encounter { get; set; }
    public string?          ErrorMessage { get; set; }
}