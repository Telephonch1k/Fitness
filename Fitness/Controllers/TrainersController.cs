using Fitness.Models;
using Fitness.Models.Data;
using Fitness.ViewModels.Trainers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

       /*
        // GET: Trainers
        public async Task<IActionResult> Index(string LastName, string FirstName, string Patronymic,
            int page = 1,
            TrainersSortState sortOrder = TrainersSortState.LastNameAsc)
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            int pageSize = 15;

            //фильтрация
            IQueryable<Trainers> Trainerses = _context.Trainerses
                .Include(s => s.Patronymic)                    
                .Where(w => w.Patronymic.IdUser == user.Id);    


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
                Trainers = (System.Collections.Generic.IEnumerable<TrainersSortState>)items
            };
            return View(viewModel);
        }
       */

        // GET: Trainers/Create
        public async Task<IActionResult> CreateAsync()
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            // при отображении страницы заполняем элемент "выпадающий список" формами обучения
            // при этом указываем, что в качестве идентификатора используется поле "Id"
            // а отображать пользователю нужно поле "FormOfEdu" - название формы обучения
            ViewData["IdFormOfStudy"] = new SelectList(_context.FormsOfStudy
                .Where(w => w.IdUser == user.Id), "Id", "FormOfEdu");
            return View();
        }

        // POST: Trainers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSpecialtyViewModel model)
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (_context.Specialties
                .Where(f => f.FormOfStudy.IdUser == user.Id &&
                    f.Code == model.Code &&
                    f.Name == model.Name &&
                    f.IdFormOfStudy == model.IdFormOfStudy)
                .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Введеная специальность уже существует");
            }

            if (ModelState.IsValid)
            {
                // если введены корректные данные,
                // то создается экземпляр класса модели Specialty, т.е. формируется запись в таблицу Specialties
                Specialty specialty = new()
                {
                    Code = model.Code,
                    Name = model.Name,

                    // с помощью свойства модели получим идентификатор выбранной формы обучения пользователем
                    IdFormOfStudy = model.IdFormOfStudy
                };

                _context.Add(specialty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdFormOfStudy"] = new SelectList(
                _context.FormsOfStudy.Where(w => w.IdUser == user.Id),
                "Id", "FormOfEdu", model.IdFormOfStudy);
            return View(model);
        }

        // GET: Trainers/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null)
            {
                return NotFound();
            }
            EditSpecialtyViewModel model = new()
            {
                Id = specialty.Id,
                Code = specialty.Code,
                Name = specialty.Name,
                IdFormOfStudy = specialty.IdFormOfStudy
            };

            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            // в списке в качестве текущего элемента устанавливаем значение из базы данных,
            // указываем параметр specialty.IdFormOfStudy
            ViewData["IdFormOfStudy"] = new SelectList(
                _context.FormsOfStudy.Where(w => w.IdUser == user.Id),
                "Id", "FormOfEdu", specialty.IdFormOfStudy);
            return View(model);
        }

        // POST: Trainers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, EditSpecialtyViewModel model)
        {
            Specialty specialty = await _context.Specialties.FindAsync(id);

            if (id != specialty.Id)
            {
                return NotFound();
            }

            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (_context.Specialties
                .Where(f => f.FormOfStudy.IdUser == user.Id &&
                    f.Code == model.Code &&
                    f.Name == model.Name &&
                    f.IdFormOfStudy == model.IdFormOfStudy)
                .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Введеная специальность уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    specialty.Code = model.Code;
                    specialty.Name = model.Name;
                    specialty.IdFormOfStudy = model.IdFormOfStudy;
                    _context.Update(specialty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialtyExists(specialty.Id))
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

            var specialty = await _context.Specialties
                .Include(s => s.FormOfStudy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specialty == null)
            {
                return NotFound();
            }

            return View(specialty);
        }

        // POST: Trainers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            _context.Specialties.Remove(specialty);
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

            var specialty = await _context.Specialties
                .Include(s => s.FormOfStudy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specialty == null)
            {
                return NotFound();
            }

            return View(specialty);
        }

        private bool SpecialtyExists(short id)
        {
            return _context.Specialties.Any(e => e.Id == id);
        }
    }
}
