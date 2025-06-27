using Dungeon_Dashboard.Event.Models;
using Dungeon_Dashboard.Home.Data;
using Microsoft.EntityFrameworkCore;

namespace Dungeon_Dashboard.Event.Services
{
    public class EventService : IEventService
    {
        private readonly AppDBContext _context;

        public EventService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventModel>> GetAllEventsAsync()
        {
            return await _context.EventModel.ToListAsync();
        }

        public async Task<EventModel?> GetEventByIdAsync(int id)
        {
            return await _context.EventModel.FindAsync(id);
        }

        public async Task<EventModel> CreateEventAsync(EventModel eventModel)
        {
            _context.EventModel.Add(eventModel);
            await _context.SaveChangesAsync();
            return eventModel;
        }

        public async Task<bool> UpdateEventAsync(int id, EventModel eventModel)
        {
            if (id != eventModel.Id) return false;

            _context.Entry(eventModel).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.EventModel.Any(e => e.Id == id))
                {
                    return false;
                }
                throw;
            }
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var eventModel = await _context.EventModel.FindAsync(id);
            if (eventModel == null) return false;

            _context.EventModel.Remove(eventModel);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
