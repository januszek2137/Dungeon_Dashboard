﻿using Dungeon_Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Dungeon_Dashboard.Controllers {

    public class CharacterModelsController : Controller {
        private readonly AppDBContext _context;

        public CharacterModelsController(AppDBContext context) {
            _context = context;
        }

        // GET: CharacterModels
        public async Task<IActionResult> Index() {
            return View(await _context.CharacterModel.ToListAsync());
        }

        // GET: CharacterModels/Details/5
        public async Task<IActionResult> Details(int? id) {
            if(id == null) {
                return NotFound();
            }

            var characterModel = await _context.CharacterModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if(characterModel == null) {
                return NotFound();
            }

            return View(characterModel);
        }

        // GET: CharacterModels/Create
        public IActionResult Create() {
            ViewBag.ClassList = Enum.GetValues(typeof(Classes))
                           .Cast<Classes>()
                           .Select(c => new SelectListItem {
                               Text = c.ToString(),
                               Value = ((int)c).ToString()
                           }).ToList();

            ViewBag.RaceList = Enum.GetValues(typeof(Races))
                                   .Cast<Races>()
                                   .Select(r => new SelectListItem {
                                       Text = r.ToString(),
                                       Value = ((int)r).ToString()
                                   }).ToList();

            return View();
        }

        // POST: CharacterModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Class,Level,Speed,ArmorClass,HitPoints,Strength,Dexterity,Constitution,Intelligence,Wisdom,Charisma,Skills,Equipment,Inventory,Copper,Silver,Electrum,Gold,Platinum")] CharacterModel characterModel) {
            ViewBag.ClassList = Enum.GetValues(typeof(Classes))
                            .Cast<Classes>()
                            .Select(c => new SelectListItem {
                                Text = c.ToString(),
                                Value = ((int)c).ToString()
                            }).ToList();

            ViewBag.RaceList = Enum.GetValues(typeof(Races))
                                   .Cast<Races>()
                                   .Select(r => new SelectListItem {
                                       Text = r.ToString(),
                                       Value = ((int)r).ToString()
                                   }).ToList();

            var username = User.Identity.Name.Split('@')[0];
            if(username != null) {
                characterModel.CreatedBy = username;
            }

            if(ModelState.IsValid) {
                _context.Add(characterModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if(!ModelState.IsValid) {
                ViewBag.ValidationErrors = ModelState.Values.SelectMany(v => v.Errors)
                                                .Select(e => e.ErrorMessage)
                                                .ToList();
                return View(characterModel);
            }
            return View(characterModel);
        }

        // GET: CharacterModels/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if(id == null) {
                return NotFound();
            }

            var characterModel = await _context.CharacterModel.FindAsync(id);
            if(characterModel == null) {
                return NotFound();
            }
            return View(characterModel);
        }

        // POST: CharacterModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Class,Level,Speed,ArmorClass,HitPoints,Strength,Dexterity,Constitution,Intelligence,Wisdom,Charisma,Skills,Equipment,Inventory,Copper,Silver,Electrum,Gold,Platinum")] CharacterModel characterModel) {
            if(id != characterModel.Id) {
                return NotFound();
            }

            if(ModelState.IsValid) {
                try {
                    _context.Update(characterModel);
                    await _context.SaveChangesAsync();
                } catch(DbUpdateConcurrencyException) {
                    if(!CharacterModelExists(characterModel.Id)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(characterModel);
        }

        // GET: CharacterModels/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if(id == null) {
                return NotFound();
            }

            var characterModel = await _context.CharacterModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if(characterModel == null) {
                return NotFound();
            }

            return View(characterModel);
        }

        // POST: CharacterModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var characterModel = await _context.CharacterModel.FindAsync(id);
            if(characterModel != null) {
                _context.CharacterModel.Remove(characterModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CharacterModelExists(int id) {
            return _context.CharacterModel.Any(e => e.Id == id);
        }
    }
}