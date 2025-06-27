using Dungeon_Dashboard.Event.Models;

namespace Dungeon_Dashboard.Event.Services
{
    public interface IEventService
    {
        Task<IEnumerable<EventModel>> GetAllEventsAsync();
        Task<EventModel?> GetEventByIdAsync(int id);
        Task<EventModel> CreateEventAsync(EventModel eventModel);
        Task<bool> UpdateEventAsync(int id, EventModel eventModel);
        Task<bool> DeleteEventAsync(int id);
    }    
}
