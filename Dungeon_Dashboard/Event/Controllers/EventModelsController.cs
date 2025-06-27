using Dungeon_Dashboard.Event.Models;
using Dungeon_Dashboard.Event.Services;
using Dungeon_Dashboard.Home.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dungeon_Dashboard.Event.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventModelsController : ControllerBase {
        private readonly IEventService _eventService;

        public EventModelsController(AppDBContext context, IEventService eventService) {
            _eventService = eventService;
        }

        // GET: api/EventModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventModel>>> GetEventModel() {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        // GET: api/EventModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EventModel>> GetEventModel(int id) {
            var eventModel = await _eventService.GetEventByIdAsync(id);
            if (eventModel == null) return NotFound();

            return Ok(eventModel);
        }

        // PUT: api/EventModels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEventModel(int id, EventModel eventModel) {
            var result = await _eventService.UpdateEventAsync(id, eventModel);
            if (!result) return NotFound();

            return NoContent();
        }

        // POST: api/EventModels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EventModel>> PostEventModel(EventModel eventModel) {
            var created = await _eventService.CreateEventAsync(eventModel);
            return CreatedAtAction(nameof(GetEventModel), new { id = created.Id }, created);
        }

        // DELETE: api/EventModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEventModel(int id) {
            var deleted = await _eventService.DeleteEventAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}