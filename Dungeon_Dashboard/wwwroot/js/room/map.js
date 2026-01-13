document.addEventListener("DOMContentLoaded", () => {
    function debounce(fn, delay) {
        let t;
        return (...args) => {
            clearTimeout(t);
            t = setTimeout(() => fn(...args), delay);
        };
    }
    
    const mapEl = document.getElementById("map");
    const gridCanvas = document.getElementById("grid");
    const markersLayer = document.getElementById("markers");
    const ctx = gridCanvas.getContext("2d");
    let mapConn;
    let positions = new Map(); // Stores GRID positions (col, row)
    let displayGridSize = 50; // Display grid size (changes with scale)
    let scale = 1; // Scale factor between natural and display size
    let naturalWidth = 1000; // Will be updated when image loads
    let naturalHeight = 1000; // Will be updated when image loads

// Grid dimensions - calculated based on image size
    let GRID_COLS = 20; // Will be recalculated based on image
    let GRID_ROWS = 20; // Will be recalculated based on image
    const BASE_GRID_SIZE = 100; // Target size for each grid cell in natural image pixels

    fetch(`/api/map/${roomId}`)
        .then(r => r.json())
        .then(data => {
            if (data.mapUrl) {
                mapEl.src = data.mapUrl;
            }
        });

// Function to update canvas and grid when image loads or window resizes
    function updateCanvasSize() {
        if (!mapEl.naturalWidth || !mapEl.naturalHeight) return;

        const displayWidth = mapEl.clientWidth;
        const displayHeight = mapEl.clientHeight;

        // Set canvas size to match displayed image size
        gridCanvas.width = displayWidth;
        gridCanvas.height = displayHeight;
        markersLayer.style.width = displayWidth + "px";
        markersLayer.style.height = displayHeight + "px";

        // Update natural dimensions
        naturalWidth = mapEl.naturalWidth;
        naturalHeight = mapEl.naturalHeight;

        // Calculate grid dimensions based on natural image size
        GRID_COLS = Math.floor(naturalWidth / BASE_GRID_SIZE);
        GRID_ROWS = Math.floor(naturalHeight / BASE_GRID_SIZE);

        // Calculate display grid size
        displayGridSize = displayWidth / GRID_COLS;
        scale = displayWidth / naturalWidth;

        drawGrid();
        updateAllMarkers();
    }

// Convert grid coordinates to display coordinates
    function gridToDisplay(gridCol, gridRow) {
        return {
            x: (gridCol + 0.5) * displayGridSize, // +0.5 to center in cell
            y: (gridRow + 0.5) * displayGridSize
        };
    }

// Convert display coordinates to grid coordinates
    function displayToGrid(displayX, displayY) {
        return {
            col: Math.floor(displayX / displayGridSize),
            row: Math.floor(displayY / displayGridSize)
        };
    }

// Convert old pixel-based positions to grid coordinates (for backward compatibility)
    function pixelToGrid(pixelX, pixelY) {
        // Calculate which grid cell this pixel position would be in
        const cellWidth = naturalWidth / GRID_COLS;
        const cellHeight = naturalHeight / GRID_ROWS;

        let col = Math.floor(pixelX / cellWidth);
        let row = Math.floor(pixelY / cellHeight);

        // Check if the pixel position is close to the center of a cell
        const cellCenterX = (col + 0.5) * cellWidth;
        const cellCenterY = (row + 0.5) * cellHeight;
        const distToCenter = Math.sqrt(Math.pow(pixelX - cellCenterX, 2) + Math.pow(pixelY - cellCenterY, 2));

        // If position is far from center, it might be from the old system - snap to nearest
        if (distToCenter > cellWidth * 0.3) {
            col = Math.round(pixelX / cellWidth - 0.5);
            row = Math.round(pixelY / cellHeight - 0.5);
        }

        // Clamp to valid grid bounds
        col = Math.min(GRID_COLS - 1, Math.max(0, col));
        row = Math.min(GRID_ROWS - 1, Math.max(0, row));

        return {col, row};
    }

// Convert grid coordinates to pixel coordinates (for SignalR compatibility)
    function gridToPixel(gridCol, gridRow) {
        const cellWidth = naturalWidth / GRID_COLS;
        const cellHeight = naturalHeight / GRID_ROWS;
        return {
            x: Math.round((gridCol + 0.5) * cellWidth),
            y: Math.round((gridRow + 0.5) * cellHeight)
        };
    }

    mapEl.onload = () => {
        naturalWidth = mapEl.naturalWidth;
        naturalHeight = mapEl.naturalHeight;

        // Calculate grid dimensions based on image
        GRID_COLS = Math.floor(naturalWidth / BASE_GRID_SIZE);
        GRID_ROWS = Math.floor(naturalHeight / BASE_GRID_SIZE);

        // Ensure minimum grid size
        GRID_COLS = Math.max(10, GRID_COLS);
        GRID_ROWS = Math.max(10, GRID_ROWS);

        console.log(`Grid initialized: ${GRID_COLS}x${GRID_ROWS} for image ${naturalWidth}x${naturalHeight}`);

        updateCanvasSize();

        // Convert any existing pixel positions to grid positions
        if (mapConn && mapConn.state === signalR.HubConnectionState.Connected) {
            const updatedPositions = new Map();
            positions.forEach((pos, uid) => {
                // If position has x,y (old format), convert to col,row
                if (pos.x !== undefined && pos.y !== undefined) {
                    const gridPos = pixelToGrid(pos.x, pos.y);
                    updatedPositions.set(uid, gridPos);

                    // Send updated position to server
                    const pixelPos = gridToPixel(gridPos.col, gridPos.row);
                    mapConn.invoke("MoveMarker", roomId, uid, pixelPos.x, pixelPos.y)
                        .catch(err => console.error("Error updating marker:", err));
                } else {
                    updatedPositions.set(uid, pos);
                }
            });
            positions = updatedPositions;
            updateAllMarkers();
        }
    };

// Add resize observer to handle responsive changes
    const resizeObserver = new ResizeObserver(() => {
        if (mapEl.complete && mapEl.naturalHeight !== 0) {
            updateCanvasSize();
        }
    });
    resizeObserver.observe(mapEl);

// Also handle window resize
    window.addEventListener('resize', debounce(() => {
        if (mapEl.complete && mapEl.naturalHeight !== 0) {
            updateCanvasSize();
        }
    }, 250));

    function drawGrid() {
        ctx.clearRect(0, 0, gridCanvas.width, gridCanvas.height);
        ctx.strokeStyle = "rgba(0,0,0,1)";
        ctx.lineWidth = Math.max(1, Math.ceil(scale));

        const totalWidth = GRID_COLS * displayGridSize;
        const totalHeight = GRID_ROWS * displayGridSize;

        // Draw vertical lines (GRID_COLS + 1 lines)
        for (let i = 0; i <= GRID_COLS; i++) {
            const x = i * displayGridSize;
            ctx.beginPath();
            ctx.moveTo(x, 0);
            ctx.lineTo(x, totalHeight);
            ctx.stroke();
        }

        // Draw horizontal lines (GRID_ROWS + 1 lines)
        for (let i = 0; i <= GRID_ROWS; i++) {
            const y = i * displayGridSize;
            ctx.beginPath();
            ctx.moveTo(0, y);
            ctx.lineTo(totalWidth, y);
            ctx.stroke();
        }
    }

    function ensureMarker(uid) {
        let el = document.getElementById("m-" + uid);
        if (!el) {
            el = document.createElement("div");
            el.id = "m-" + uid;
            el.className = "marker text-white fw-bold rounded-circle d-flex align-items-center justify-content-center";

            el.style.position = "absolute";
            el.style.transform = "translate(-50%,-50%)";
            el.textContent = uid.substring(0, 2).toUpperCase();
            el.style.pointerEvents = "auto";

            if (uid === userName) {
                el.style.backgroundColor = "#28a745";
            } else {
                el.style.backgroundColor = "#343a40";
            }

            markersLayer.appendChild(el);

            if (uid === userName || userName === roomCreatedBy) {
                enableDrag(el, uid);
            }
        }

        // Always update marker size based on current scale
        const markerSize = Math.max(24, Math.min(displayGridSize, Math.floor(55 * scale)));
        el.style.width = markerSize + "px";
        el.style.height = markerSize + "px";
        el.style.fontSize = Math.max(12, Math.floor(16 * scale)) + "px";

        return el;
    }

    function setMarker(uid, pixelX, pixelY) {
        // Convert pixel coordinates from server to grid coordinates
        const gridPos = pixelToGrid(pixelX, pixelY);

        // Check if position actually changed
        const existingPos = positions.get(uid);
        if (!existingPos || existingPos.col !== gridPos.col || existingPos.row !== gridPos.row) {
            positions.set(uid, gridPos);
            updateMarkerDisplay(uid);
        }
    }

    function updateMarkerDisplay(uid) {
        const pos = positions.get(uid);
        if (!pos) return;

        // Convert grid position to display position
        const displayPos = gridToDisplay(pos.col, pos.row);
        const el = ensureMarker(uid);
        el.style.left = displayPos.x + "px";
        el.style.top = displayPos.y + "px";
    }

    function updateAllMarkers() {
        positions.forEach((pos, uid) => {
            updateMarkerDisplay(uid);
        });
    }

    function enableDrag(el, uid) {
        let isDragging = false;

        el.onpointerdown = e => {
            e.preventDefault();
            e.stopPropagation();

            isDragging = true;
            el.setPointerCapture(e.pointerId);
            el.style.cursor = "grabbing";
            el.style.transition = "none";

            let oldPos = {...positions.get(uid)};
            let lastHighlightedCell = null;

            el.onpointermove = ev => {
                if (!isDragging) return;
                ev.preventDefault();

                let rect = gridCanvas.getBoundingClientRect();
                let displayX = ev.clientX - rect.left;
                let displayY = ev.clientY - rect.top;

                // Get grid cell under cursor
                let gridPos = displayToGrid(displayX, displayY);

                // Clamp to grid bounds
                gridPos.col = Math.max(0, Math.min(GRID_COLS - 1, gridPos.col));
                gridPos.row = Math.max(0, Math.min(GRID_ROWS - 1, gridPos.row));

                // Get display position for this grid cell
                let displayPos = gridToDisplay(gridPos.col, gridPos.row);

                // Check for conflicts with other markers
                let conflict = false;
                positions.forEach((pos, otherUid) => {
                    if (otherUid !== uid && pos.col === gridPos.col && pos.row === gridPos.row) {
                        conflict = true;
                    }
                });

                // Only redraw if cell changed
                if (!lastHighlightedCell ||
                    lastHighlightedCell.col !== gridPos.col ||
                    lastHighlightedCell.row !== gridPos.row) {

                    drawGrid();

                    // Highlight current cell
                    if (conflict) {
                        ctx.fillStyle = "rgba(255,0,0,0.3)";
                    } else {
                        ctx.fillStyle = "rgba(0,255,0,0.1)";
                    }
                    ctx.fillRect(
                        gridPos.col * displayGridSize,
                        gridPos.row * displayGridSize,
                        displayGridSize,
                        displayGridSize
                    );

                    lastHighlightedCell = gridPos;
                }

                // Update visual position
                el.style.left = displayPos.x + "px";
                el.style.top = displayPos.y + "px";
            };

            el.onpointerup = async ev => {
                if (!isDragging) return;
                isDragging = false;
                ev.preventDefault();

                let rect = gridCanvas.getBoundingClientRect();
                let displayX = ev.clientX - rect.left;
                let displayY = ev.clientY - rect.top;

                // Get final grid position
                let gridPos = displayToGrid(displayX, displayY);

                // Clamp to grid bounds
                gridPos.col = Math.max(0, Math.min(GRID_COLS - 1, gridPos.col));
                gridPos.row = Math.max(0, Math.min(GRID_ROWS - 1, gridPos.row));

                // Check for final conflict
                let conflict = false;
                positions.forEach((pos, otherUid) => {
                    if (otherUid !== uid && pos.col === gridPos.col && pos.row === gridPos.row) {
                        conflict = true;
                    }
                });

                if (conflict) {
                    // Revert to old position
                    positions.set(uid, oldPos);
                    updateMarkerDisplay(uid);
                } else {
                    // Update local position
                    positions.set(uid, gridPos);
                    updateMarkerDisplay(uid);

                    // Convert to pixel coordinates for server (backward compatibility)
                    const pixelPos = gridToPixel(gridPos.col, gridPos.row);

                    // Broadcast to other clients
                    if (mapConn && mapConn.state === signalR.HubConnectionState.Connected) {
                        await mapConn.invoke("MoveMarker", roomId, uid, pixelPos.x, pixelPos.y)
                            .catch(err => console.error("Error broadcasting marker position:", err));
                    }
                }

                drawGrid();
                el.onpointermove = null;
                el.onpointerup = null;
                el.style.cursor = "grab";
                el.style.transition = "left 0.1s linear, top 0.1s linear";
                el.releasePointerCapture(ev.pointerId);
            };
        };
    }

    mapConn = new signalR.HubConnectionBuilder()
        .withUrl(`/mapHub?roomId=${roomId}`)
        .withAutomaticReconnect()
        .build();

    mapConn.on("MarkersLoaded", markers => {
        markers.forEach(m => setMarker(m.userId, m.x, m.y));
    });

    mapConn.on("MarkerMoved", m => setMarker(m.userId, m.x, m.y));

    mapConn.start().then(() => mapConn.invoke("LoadMarkers", roomId));

    mapConn.on("MapUpdated", newUrl => {
        setTimeout(() => {
            const img = new Image();
            img.onload = () => {
                mapEl.src = img.src;
            };
            img.src = newUrl + "?v=" + Date.now();
        }, 200);
    });

    document.getElementById("mapUploadForm").addEventListener("submit", async e => {
        e.preventDefault();
        const formData = new FormData(e.target);
        const res = await fetch(`/api/map/${roomId}/upload`, {
            method: "POST",
            body: formData
        });
        if (res.ok) {
            const data = await res.json();
            mapEl.src = data.mapUrl + "?v=" + Date.now();
        } else {
            alert("Error uploading map");
        }
    });
});