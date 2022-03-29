using Fitness.Models;
using Fitness.Models.Data;
using Fitness.ViewModels.Trainers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Fitness.Controllers
{
    [Authorize(Roles = "admin, registeredUser")]
    public class TrainersController : Controller
    {
        private readonly AppCtx _context;
        private readonly UserManager<User> _userManager;

        public TrainersController(AppCtx context, UserManager<User> user)
        {
            _context = context;
            _userManager = user;
        }

       
        // GET: Trainers
        public async Task<IActionResult> Index(string LastName, string FirstName, string Patronymic,
            int page = 1,
            TrainersSortState sortOrder = TrainersSortState.LastNameAsc)
        {
            int pageSize = 15;

            //фильтрация
            IQueryable<Trainer> Trainerses = _context.Trainerses;    


            if (!String.IsNullOrEmpty(LastName))
            {
                Trainerses = Trainerses.Where(p => p.LastName.Contains(LastName));
            }
            if (!String.IsNullOrEmpty(FirstName))
            {
                Trainerses = Trainerses.Where(p => p.FirstName.Contains(FirstName));
            }
            if (!String.IsNullOrEmpty(Patronymic))
            {
                Trainerses = Trainerses.Where(p => p.Patronymic.Contains(Patronymic));
            }


            // сортировка
            switch (sortOrder)
            {
                case TrainersSortState.LastNameDesc:
                    Trainerses = Trainerses.OrderByDescending(s => s.LastName);
                    break;
                case TrainersSortState.FirstNameAsc:
                    Trainerses = Trainerses.OrderBy(s => s.FirstName);
                    break;
                case TrainersSortState.FirstNameDesc:
                    Trainerses = Trainerses.OrderByDescending(s => s.FirstName);
                    break;
                case TrainersSortState.PatronymicAsc:
                    Trainerses = Trainerses.OrderBy(s => s.Patronymic);
                    break;
                case TrainersSortState.PatronymicDesc:
                    Trainerses = Trainerses.OrderByDescending(s => s.Patronymic);
                    break;
                default:
                    Trainerses = Trainerses.OrderBy(s => s.LastName);
                    break;
            }

            // пагинация
            var count = await Trainerses.CountAsync();
            var items = await Trainerses.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // формируем модель представления
            IndexTrainersViewModel viewModel = new()
            {
                PageViewModel = new(count, page, pageSize),
                SortTrainersViewModel = new(sortOrder),
                FilterTrainersViewModel = new(LastName, FirstName, Patronymic),
                Trainers = items
            };
            return View(viewModel);
        }
       

        // GET: Trainers/Create
        public async Task<IActionResult> CreateAsync()
        {            
            return View();
        }

        // POST: Trainers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTrainersViewModel model)
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (_context.Trainerses
                .Where(f => f.LastName== model.LastName &&
                    f.FirstName == model.FirstName &&
                    f.Patronymic == model.Patronymic)
                .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Введеный тренер уже существует");
            }

            if (ModelState.IsValid)
            {
                // если введены корректные данные,
                // то создается экземпляр класса модели Specialty, т.е. формируется запись в таблицу Specialties
                Trainer trainer = new()
                {
                    LastName= model.LastName,
                    FirstName = model.FirstName,
                    Patronymic=model.Patronymic
                };

                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Trainers/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainerses.FindAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }

            EditTrainersViewModel model = new()
            {
                Id = trainer.Id,
                LastName= trainer.LastName,
                FirstName= trainer.FirstName,
                Patronymic = trainer.Patronymic
            };            
            
            return View(model);
        }

        // POST: Trainers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, EditTrainersViewModel model)
        {
            Trainer trainer = await _context.Trainerses.FindAsync(id);

            if (id != trainer.Id)
            {
                return NotFound();
            }
            
            if (_context.Trainerses
                .Where(f => f.LastName == model.LastName &&
                    f.FirstName == model.FirstName &&
                    f.Patronymic == model.Patronymic)
                .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Введеный тренер уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    trainer.LastName = model.LastName;
                    trainer.FirstName = model.FirstName;
                    trainer.Patronymic = model.Patronymic;
                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerExists(trainer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Trainers/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainerses                
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // POST: Trainers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var trainer = await _context.Trainerses.FindAsync(id);
            _context.Trainerses.Remove(trainer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Specialties/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainerses                
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        private bool TrainerExists(short id)
        {
            return _context.Trainerses.Any(e => e.Id == id);
        }
    }
}
