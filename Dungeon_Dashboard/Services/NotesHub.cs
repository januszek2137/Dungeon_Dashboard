using DiffMatchPatch;
using Dungeon_Dashboard.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Dungeon_Dashboard.Hubs {

    public class NotesHub : Hub {
        private readonly AppDBContext _db;
        private readonly diff_match_patch _dmp = new diff_match_patch();

        public NotesHub(AppDBContext db) {
            _db = db;
        }

        public async Task JoinRoom(int roomId) {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
            var notes = await _db.NoteModel
                                 .Where(n => n.RoomId == roomId)
                                 .OrderBy(n => n.Timestamp)
                                 .ToListAsync();
            await Clients.Caller.SendAsync("ReceiveNotes", notes);
        }

        public async Task AddNote(int roomId, string content, string userName) {
            var note = new NoteModel {
                RoomId = roomId,
                Content = content,
                CreatedBy = userName,
                Timestamp = DateTime.UtcNow
            };
            _db.NoteModel.Add(note);
            await _db.SaveChangesAsync();

            var notes = await _db.NoteModel
                                 .Where(n => n.RoomId == roomId)
                                 .OrderBy(n => n.Timestamp)
                                 .ToListAsync();
            await Clients.Group(roomId.ToString()).SendAsync("ReceiveNotes", notes);
        }

        public async Task EditNotePatch(int roomId, int noteId, string patchText) {
            patchText = Uri.UnescapeDataString(patchText);

            var note = await _db.NoteModel.FindAsync(noteId);
            if(note == null || note.RoomId != roomId)
                return;

            var patches = _dmp.patch_fromText(patchText);
            var result = _dmp.patch_apply(patches, note.Content);
            note.Content = (string)result[0];
            note.Timestamp = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            await Clients.OthersInGroup(roomId.ToString())
                         .SendAsync("ReceiveNotePatch", noteId, patchText);
        }

        public async Task DeleteNote(int roomId, int noteId) {
            var note = await _db.NoteModel.FindAsync(noteId);
            if(note == null || note.RoomId != roomId)
                return;

            _db.NoteModel.Remove(note);
            await _db.SaveChangesAsync();

            await Clients.Group(roomId.ToString())
                         .SendAsync("NoteDeleted", noteId);
        }
    }
}