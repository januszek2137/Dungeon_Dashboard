document.addEventListener("DOMContentLoaded", () => {

    const dmp = new diff_match_patch();
    const noteStates = {};
    const notesConn = new signalR.HubConnectionBuilder()
        .withUrl("/notesHub")
        .withAutomaticReconnect()
        .build();

    notesConn.on("ReceiveNotes", notes => {
        const c = document.getElementById("notesContainer");
        c.innerHTML = "";
        notes.forEach(n => {
            noteStates[n.id] = n.content;
            renderNote(n.id, n.content);
        });
    });

    notesConn.on("ReceiveNotePatch", (id, patchText) => {
        const el = document.querySelector(`span.note-text[data-note-id='${id}']`);
        if (!el) return;
        const patches = dmp.patch_fromText(patchText);
        const [newText] = dmp.patch_apply(patches, el.textContent);
        noteStates[id] = newText;
        el.textContent = newText;
    });

    notesConn.on("NoteDeleted", id => {
        const item = document.querySelector(`.note-item[data-note-id='${id}']`);
        if (item) item.remove();
    });

    notesConn.start().then(() => notesConn.invoke("FetchNotes", roomId));

    document.getElementById("addNoteBtn").addEventListener("click", () => {
        const txt = document.getElementById("newNoteContent");
        const content = txt.value.trim();
        if (!content) return;
        notesConn.invoke("AddNote", roomId, content, userName)
            .then(() => txt.value = "")
            .catch(console.error);
    });

    function renderNote(id, content) {
        const c = document.getElementById("notesContainer");
        const container = document.createElement("div");
        container.className = "note-item";
        container.setAttribute("data-note-id", id);

        const textSpan = document.createElement("span");
        textSpan.className = "note-text form-input";
        textSpan.setAttribute("data-note-id", id);
        textSpan.setAttribute("contentEditable", "true");
        textSpan.textContent = content;

        const btn = document.createElement("button");
        btn.type = "button";
        btn.className = "btn-close-modal";
        btn.setAttribute("aria-label", "Usuń");
        btn.setAttribute("contentEditable", "false");
        btn.addEventListener("click", e => {
            e.stopPropagation();
            notesConn.invoke("DeleteNote", roomId, id).catch(console.error);
        });

        textSpan.addEventListener("input", debounce(() => {
            const old = noteStates[id];
            const curr = textSpan.textContent;
            const patches = dmp.patch_make(old, curr);
            const patchText = dmp.patch_toText(patches);
            noteStates[id] = curr;
            notesConn.invoke("EditNotePatch", roomId, id, patchText)
                .catch(console.error);
        }, 300));

        container.appendChild(textSpan);
        container.appendChild(btn);
        c.appendChild(container);
    }

    function debounce(fn, delay) {
        let t;
        return (...args) => {
            clearTimeout(t);
            t = setTimeout(() => fn(...args), delay);
        };
    }
});