using Dungeon_Dashboard.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dungeon_Dashboard.Event {

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventModelsController : ControllerBase {
        private readonly AppDBContext _context;

        public EventModelsController(AppDBContext context) {
            _context = context;
        }

        // GET: api/EventModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventModel>>> GetEventModel() {
            return await _context.EventModel.ToListAsync();
        }

        // GET: api/EventModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EventModel>> GetEventModel(int id) {
            var eventModel = await _context.EventModel.FindAsync(id);

            if(eventModel == null) {
                return NotFound();
            }

            return eventModel;
        }

        // PUT: api/EventModels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEventModel(int id, EventModel eventModel) {
            if(id != eventModel.Id) {
                return BadRequest();
            }

            _context.Entry(eventModel).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch(DbUpdateConcurrencyException) {
                if(!EventModelExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/EventModels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EventModel>> PostEventModel(EventModel eventModel) {
            _context.EventModel.Add(eventModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEventModel", new { id = eventModel.Id }, eventModel);
        }

        // DELETE: api/EventModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEventModel(int id) {
            var eventModel = await _context.EventModel.FindAsync(id);
            if(eventModel == null) {
                return NotFound();
            }

            _context.EventModel.Remove(eventModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EventModelExists(int id) {
            return _context.EventModel.Any(e => e.Id == id);
        }
    }
}