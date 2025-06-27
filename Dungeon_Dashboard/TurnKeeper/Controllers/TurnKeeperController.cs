using Dungeon_Dashboard.TurnKeeper.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dungeon_Dashboard.TurnKeeper.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class TurnKeeperController : ControllerBase {
        private static List<InitiativeParticipant> Participants = new List<InitiativeParticipant>();

        [HttpGet]
        public IActionResult GetParticipants() => Ok(Participants);

        [HttpPost]
        public IActionResult AddParticipant(InitiativeParticipant participant) {
            participant.Id = Participants.Count > 0 ? Participants.Max(p => p.Id) + 1 : 1;
            Participants.Add(participant);
            return Ok(Participants);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateParticipant(int id, InitiativeParticipant updated) {
            var participant = Participants.FirstOrDefault(p => p.Id == id);
            if(participant == null)
                return NotFound();
            participant.Name = updated.Name;
            participant.Initiative = updated.Initiative;
            participant.Health = updated.Health;
            return Ok(Participants);
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveParticipant(int id) {
            Participants.RemoveAll(p => p.Id == id);
            return Ok(Participants);
        }
    }
}