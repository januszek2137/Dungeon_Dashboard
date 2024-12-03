$(document).ready(function () {
    // Load Single NPC
    $('#btnLoadNPC').click(function () {
        $('#resultContainer').html('<div class="spinner-border" role="status"><span class="sr-only">Loading...</span></div>');
        $.ajax({
            url: '/CharacterGenerator/GetRandomNPC',
            type: 'GET',
            success: function (data) {
                $('#resultContainer').html(`
                    <div class="col-md-12">
                        <div class="card npc">
                            <h3>🧙 ${data.name || "Unknown Name"}</h3>
                            <p><em>${data.description || "No Description"}</em></p>
                            <div class="card-content">
                                <div>
                                    <p><span class="icon">🎚️</span><strong>Level:</strong> ${data.level || "Unknown"}</p>
                                    <p><span class="icon">🎭</span><strong>Role:</strong> ${data.role || "Unknown"}</p>
                                    <p><span class="icon">❤️</span><strong>Health:</strong> ${data.health || "Unknown"}</p>
                                    <p><span class="icon">🛡️</span><strong>AC:</strong> ${data.armorClass || "Unknown"}</p>
                                </div>
                                <div>
                                    <table class="attributes-table">
                                        <tr>
                                            <td><span class="icon">💪</span><strong>STR:</strong> ${data.strength || "Unknown"}</td>
                                            <td><span class="icon">🤸</span><strong>DEX:</strong> ${data.dexterity || "Unknown"}</td>
                                        </tr>
                                        <tr>
                                            <td><span class="icon">🏋️</span><strong>CON:</strong> ${data.constitution || "Unknown"}</td>
                                            <td><span class="icon">🧠</span><strong>INT:</strong> ${data.intelligence || "Unknown"}</td>
                                        </tr>
                                        <tr>
                                            <td><span class="icon">👁️</span><strong>WIS:</strong> ${data.wisdom || "Unknown"}</td>
                                            <td><span class="icon">🗣️</span><strong>CHA:</strong> ${data.charisma || "Unknown"}</td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                `);
            },
            error: function (xhr, status, error) {
                $('#resultContainer').html('<div class="alert alert-danger">Error loading NPC.</div>');
            }
        });
    });

    // Load Single Monster
    $('#btnLoadMonster').click(function () {
        $('#resultContainer').html('<div class="spinner-border" role="status"><span class="sr-only">Loading...</span></div>');
        $.ajax({
            url: '/CharacterGenerator/GetRandomMonster',
            type: 'GET',
            success: function (data) {
                $('#resultContainer').html(`
                    <div class="col-md-12">
                        <div class="card monster">
                            <h3>🐲 ${data.species || "Unknown Species"}</h3>
                            <p><em>${data.description || "No Description"}</em></p>
                            <div class="card-content">
                                <div>
                                    <p><span class="icon">🗺️</span><strong>Type:</strong> ${data.type || "Unknown Type"}</p>
                                    <p><span class="icon">🎚️</span><strong>Level:</strong> ${data.level || "Unknown"}</p>
                                    <p><span class="icon">❤️</span><strong>Health:</strong> ${data.health || "Unknown"}</p>
                                    <p><span class="icon">🛡️</span><strong>AC:</strong> ${data.armorClass || "Unknown"}</p>
                                </div>
                                <div>
                                    <p><span class="icon">⚔️</span><strong>Damage:</strong> ${data.damage || "Unknown"}</p>
                                    <p><span class="icon">✨</span><strong>Abilities:</strong> ${data.abilities || "None"}</p>
                                </div>
                            </div>
                        </div>
                    </div>
                `);
            },
            error: function (xhr, status, error) {
                $('#resultContainer').html('<div class="alert alert-danger">Error loading Monster.</div>');
            }
        });
    });

    // Load Single Encounter
    $('#btnLoadEncounter').click(function () {
        $('#resultContainer').html('<div class="spinner-border" role="status"><span class="sr-only">Loading...</span></div>');
        $.ajax({
            url: '/CharacterGenerator/GetRandomEncounter',
            type: 'GET',
            success: function (data) {
                $('#resultContainer').html(`
                    <div class="col-md-12">
                        <div class="card encounter">
                            <!-- Encounter Header -->
                            <div class="encounter-header">
                                <h3>⚔️ ${data.name || "Unknown Encounter Name"}</h3>
                                <p><em>${data.description || "No Description"}</em></p>
                                <div class="card-content">
                                    <div>
                                        <p><span class="icon">📍</span><strong>Location:</strong> ${data.location || "Unknown Location"}</p>
                                        <p><span class="icon">🌦️</span><strong>Weather:</strong> ${data.weather || "Unknown Weather"}</p>
                                    </div>
                                    <div>
                                        <p><span class="icon">⏰</span><strong>Time:</strong> ${data.timeOfDay || "Unknown Time"}</p>
                                        <p><span class="icon">🌄</span><strong>Terrain:</strong> ${data.terrain || "Unknown Terrain"}</p>
                                    </div>
                                </div>
                            </div>

                            <!-- Involved NPCs Section -->
                            <div class="encounter-npcs">
                                <h4>🧑‍🤝‍🧑 Involved NPCs:</h4>
                                <div class="npcs row">
                                    ${Array.isArray(data.involvedNPCs) && data.involvedNPCs.length > 0
                        ? data.involvedNPCs.map(npc => `
                                            <div class="col-md-6">
                                                <div class="card npc">
                                                    <h5>🧙 ${npc.name || "Unknown NPC"}</h5>
                                                    <div class="card-content">
                                                        <div>
                                                            <p><span class="icon">🎭</span><strong>Role:</strong> ${npc.role || "Unknown Role"}</p>
                                                            <p><span class="icon">🎚️</span><strong>Level:</strong> ${npc.level || "Unknown"}</p>
                                                            <p><span class="icon">❤️</span><strong>HP:</strong> ${npc.health || "Unknown"}</p>
                                                            <p><span class="icon">🛡️</span><strong>AC:</strong> ${npc.armorClass || "Unknown"}</p>
                                                        </div>
                                                        <div>
                                                            <table class="attributes-table">
                                                                <tr>
                                                                    <td><span class="icon">💪</span><strong>STR:</strong> ${npc.strength || "Unknown"}</td>
                                                                    <td><span class="icon">🤸</span><strong>DEX:</strong> ${npc.dexterity || "Unknown"}</td>
                                                                </tr>
                                                                <tr>
                                                                    <td><span class="icon">🏋️</span><strong>CON:</strong> ${npc.constitution || "Unknown"}</td>
                                                                    <td><span class="icon">🧠</span><strong>INT:</strong> ${npc.intelligence || "Unknown"}</td>
                                                                </tr>
                                                                <tr>
                                                                    <td><span class="icon">👁️</span><strong>WIS:</strong> ${npc.wisdom || "Unknown"}</td>
                                                                    <td><span class="icon">🗣️</span><strong>CHA:</strong> ${npc.charisma || "Unknown"}</td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        `).join('')
                        : "<p>No NPCs involved.</p>"}
                                </div>
                            </div>

                            <!-- Involved Monsters Section -->
                            <div class="encounter-monsters">
                                <h4>🦖 Involved Monsters:</h4>
                                <div class="monsters row">
                                    ${Array.isArray(data.involvedMonsters) && data.involvedMonsters.length > 0
                        ? data.involvedMonsters.map(monster => `
                                            <div class="col-md-6">
                                                <div class="card monster">
                                                    <h5>🐲 ${monster.species || "Unknown Species"}</h5>
                                                    <div class="card-content">
                                                        <div>
                                                            <p><span class="icon">🗺️</span><strong>Type:</strong> ${monster.type || "Unknown Type"}</p>
                                                            <p><span class="icon">🎚️</span><strong>Level:</strong> ${monster.level || "Unknown"}</p>
                                                            <p><span class="icon">❤️</span><strong>HP:</strong> ${monster.health || "Unknown"}</p>
                                                            <p><span class="icon">🛡️</span><strong>AC:</strong> ${monster.armorClass || "Unknown"}</p>
                                                        </div>
                                                        <div>
                                                            <p><span class="icon">⚔️</span><strong>Damage:</strong> ${monster.damage || "Unknown"}</p>
                                                            <p><span class="icon">✨</span><strong>Abilities:</strong> ${monster.abilities || "None"}</p>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        `).join('')
                        : "<p>No Monsters involved.</p>"}
                                </div>
                            </div>
                        </div>
                    </div>
                `);
            },
            error: function (xhr, status, error) {
                $('#resultContainer').html('<div class="alert alert-danger">Error loading Encounter.</div>');
            }
        });
    });

    $(".action-btn").on("click", function () {
        $("#placeholder").remove();
    });

    // Load Multiple NPCs
    $('#btnLoadNPCs').click(function () {
        $.ajax({
            url: '/CharacterGenerator/GetNPCs',
            type: 'GET',
            data: { count: 5 },
            success: function (data) {
                $('#multipleResultsContainer').empty();
                data.forEach(npc => {
                    const npcHtml = `
                <div class="npc-card">
                    <h4>${npc.name} (${npc.role})</h4>
                    <p><strong>Level:</strong> ${npc.level}</p>
                    <p><strong>Health:</strong> ${npc.health}</p>
                    <p><strong>Armor Class:</strong> ${npc.armorClass}</p>
                    <p><strong>Strength:</strong> ${npc.strength}</p>
                    <p><strong>Dexterity:</strong> ${npc.dexterity}</p>
                    <p><strong>Constitution:</strong> ${npc.constitution}</p>
                    <p><strong>Intelligence:</strong> ${npc.intelligence}</p>
                    <p><strong>Wisdom:</strong> ${npc.wisdom}</p>
                    <p><strong>Charisma:</strong> ${npc.charisma}</p>
                    <p><strong>Description:</strong> ${npc.description}</p>
                </div>
                <hr>
                `;
                    $('#multipleResultsContainer').append(npcHtml);
                });
            },
            error: function (xhr, status, error) {
                console.error('Error:', error);
                console.error('Status:', status);
                console.error('Response:', xhr.responseText);
                $('#multipleResultsContainer').html('<div class="alert alert-danger">Error loading NPCs.</div>');
            }
        });
    });




    $('#btnLoadMonsters').click(function () {
        $.ajax({
            url: '/CharacterGenerator/GetMonsters',
            type: 'GET',
            data: { count: 10 }, // Możesz zmienić liczbę Monsterów do załadowania
            success: function (data) {
                $('#multipleResultsContainer').empty();
                data.forEach(monster => {
                    const monsterHtml = `
            <div class="monster-card">
                <h4>${monster.species} (${monster.type})</h4>
                <p><strong>Level:</strong> ${monster.level}</p>
                <p><strong>Health:</strong> ${monster.health}</p>
                <p><strong>Armor Class:</strong> ${monster.armorClass}</p>
                <p><strong>Damage:</strong> ${monster.damage}</p>
                <p><strong>Abilities:</strong> ${monster.abilities}</p>
                <p><strong>Description:</strong> ${monster.description}</p>
            </div>
            <hr>
            `;
                    $('#multipleResultsContainer').append(monsterHtml);
                });
            },
            error: function (xhr, status, error) {
                console.error('Error:', error);
                console.error('Status:', status);
                console.error('Response:', xhr.responseText);
                $('#multipleResultsContainer').html('<div class="alert alert-danger">Error loading Monsters.</div>');
            }
        });
    });

});