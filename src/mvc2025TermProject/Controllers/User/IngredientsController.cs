using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using mvc2025TermProject.Data;
using mvc2025TermProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc2025TermProject.Controllers.User
{
    [Authorize(Roles = "User, Administrator")]
    public class IngredientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IngredientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ingredients
        [HttpGet]
        public async Task<IActionResult> Index(string? sortOrder, string? filterCriteria, string? searchString, int? pageNumber)
        {
            const int pageSize = 10;

            var ingredientList = _context.Ingredients
                .Select(i => new IngredientListViewModel()
                    {
                        ID              = i.Id,
                        Name            = i.Name,
                        Type            = i.Type,
                        Description     = i.Description,
                        Status          = (i.IsApproved == false && i.IsPendingModification == true) ? "Pending Approval" :
                                          (i.IsApproved == true && i.IsPendingModification == false) ? "Approved" :
                                          (i.IsApproved == true && i.IsPendingModification == true) ? "Pending Update" :
                                          "Rejected",
                        CreatedDate     = i.CreatedDate,
                        UnderReview     = (i.IsPendingModification == true) ? true : false
                    }
                );

            // SORTING PARAMETERS
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : null;
            ViewData["DateSortParam"] = (sortOrder) == "date" ? "date_desc" : "date";
            ViewData["StatusSortParam"] = (sortOrder) == "status" ? "status_desc" : "status";
            ViewData["CurrentSort"] = sortOrder;
            ViewData["SearchFilterCriteria"] = filterCriteria;

            // Handle search string and pagination reset
            if (searchString != null)
                pageNumber = 1;
            else
                searchString = filterCriteria;

            if (!(String.IsNullOrEmpty(filterCriteria) || String.IsNullOrWhiteSpace(filterCriteria)))
            {
                ingredientList = ingredientList.Where(i =>
                    i.Name!.Contains(filterCriteria) ||
                    i.Description!.Contains(filterCriteria) ||
                    i.Type!.Contains(filterCriteria) ||
                    i.Status!.Contains(filterCriteria)
                );
            }

            // Total count for search results
            int totalCount = await ingredientList.CountAsync();
            ViewData["TotalMatches"] = totalCount;
            ViewData["HasSearch"] = !string.IsNullOrEmpty(filterCriteria);

            switch (sortOrder)
            {
                case "date":
                    ingredientList = ingredientList.OrderBy(i => i.CreatedDate);
                    break;

                case "date_desc":
                    ingredientList = ingredientList.OrderByDescending(i => i.CreatedDate);
                    break;

                case "name_desc":
                    ingredientList = ingredientList.OrderByDescending(i => i.Name);
                    break;

                case "status":
                    ingredientList = ingredientList.OrderBy(i => i.Status);
                    break;

                case "status_desc":
                    ingredientList = ingredientList.OrderByDescending(i => i.Status);
                    break;

                default:
                    ingredientList = ingredientList.OrderBy(i => i.Name);
                    break;
            }

            if (pageNumber == null)
                pageNumber = 1;

            return View(await PaginatedList<IngredientListViewModel>.CreateAsync(ingredientList, (int)pageNumber, pageSize));
        }

        // GET: Ingredients/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ingredient == null)
            {
                return NotFound();
            }

            var ingredientVM = new IngredientDetailViewModel
            {
                ID                      = ingredient.Id,
                Name                    = ingredient.Name,
                Type                    = ingredient.Type,
                Description             = ingredient.Description,
                Status                  = (ingredient.IsApproved == false && ingredient.IsPendingModification == true) ? "Pending Approval" : 
                                          (ingredient.IsApproved == true && ingredient.IsPendingModification == false) ? "Approved" :
                                          (ingredient.IsApproved == true && ingredient.IsPendingModification == true) ? "Pending Update" :
                                          "Rejected",
                IsPendingModification   = ingredient.IsPendingModification,
                ProposedName            = ingredient.PendingName,
                ProposedType            = ingredient.PendingType,
                ProposedDescription     = ingredient.PendingDescription,
                CreatedDate             = ingredient.CreatedDate,
                CreatedBy               = ingredient.CreatedBy,
                LastModifiedDate        = ingredient.LastModifiedDate,
                ApprovedDate            = ingredient.ApprovedDate,
                DecidedBy               = ingredient.DecidedBy
            };

            return View(ingredientVM);
        }

        // GET: Ingredients/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ingredients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Type,Description")] IngredientFormFieldsViewModel ingredientVM)
        {
            if (ModelState.IsValid)
            {
                // Validatioin to check for existing ingredient with same name and type
                if (!IngredientProposedExists(ingredientVM.Name!, ingredientVM.Type!))
                {
                    var newIngredient = new Ingredient
                    {
                        Name                    = ingredientVM.Name,
                        Type                    = ingredientVM.Type,
                        Description             = ingredientVM.Description,
                        IsApproved              = false,
                        CreatedDate             = DateTime.Today,
                        CreatedBy               = User.Identity!.Name!,
                        IsPendingModification   = true
                    };
                    _context.Add(newIngredient);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Name", $"Ingredient '{ingredientVM.Name} ({ingredientVM.Type})' already exists or is pending approval!");
                }
            }

            return View(ingredientVM);
        }

        // GET: Ingredients/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            // Only for approved ingredients with pending modifications, show pending values
            string? displayName = ingredient.Name;
            string? displayType = ingredient.Type;
            string? displayDescription = ingredient.Description;

            if (ingredient.IsApproved == true && ingredient.IsPendingModification == true)
            {
                displayName =
                    ingredient.PendingName != null ? ingredient.PendingName : ingredient.Name;
                displayType =
                    ingredient.PendingType != null ? ingredient.PendingType : ingredient.Type;
                displayDescription =
                    ingredient.PendingDescription != null ? ingredient.PendingDescription : ingredient.Description;
            }

            var ingredientVM = new IngredientDetailViewModel
            {
                ID                      = ingredient.Id,
                Name                    = displayName,
                Type                    = displayType,
                Description             = displayDescription,
                Status                  = (ingredient.IsApproved == false && ingredient.IsPendingModification == true) ? "Pending Approval" :
                                          (ingredient.IsApproved == true && ingredient.IsPendingModification == false) ? "Approved" :
                                          (ingredient.IsApproved == true && ingredient.IsPendingModification == true) ? "Pending Update" :
                                          "Rejected",
                IsPendingModification   = ingredient.IsPendingModification,
                ProposedName            = ingredient.PendingName,
                ProposedType            = ingredient.PendingType,
                ProposedDescription     = ingredient.PendingDescription,
                CreatedDate             = ingredient.CreatedDate,
                CreatedBy               = ingredient.CreatedBy,
                LastModifiedDate        = ingredient.LastModifiedDate,
                ApprovedDate            = ingredient.ApprovedDate,
                DecidedBy               = ingredient.DecidedBy
            };

            return View(ingredientVM);
        }

        // POST: Ingredients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("ID,Name,Type,Description")] IngredientDetailViewModel ingredientVM)
        {
            if (id != ingredientVM.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!IngredientProposedExistsExcludingId(ingredientVM.Name!, ingredientVM.Type!, ingredientVM.ID))
                    {
                        var existingIngredient = await _context.Ingredients.FindAsync(id);
                        if (existingIngredient == null)
                        {
                            return NotFound();
                        }

                        if (existingIngredient.IsApproved == true)
                        {
                            // For approved ingredients: store changes in pending fields
                            existingIngredient.PendingName = ingredientVM.Name;
                            existingIngredient.PendingType = ingredientVM.Type;
                            existingIngredient.PendingDescription = ingredientVM.Description;
                            existingIngredient.IsPendingModification = true;
                            existingIngredient.LastModifiedDate = DateTime.Today;
                        }
                        else
                        {
                            // For non-approved ingredients: update main fields directly
                            existingIngredient.Name = ingredientVM.Name;
                            existingIngredient.Type = ingredientVM.Type;
                            existingIngredient.Description = ingredientVM.Description;
                            existingIngredient.IsPendingModification = true;
                            existingIngredient.LastModifiedDate = DateTime.Today;

                            // Clear pending fields since we're updating the main fields
                            existingIngredient.PendingName = null;
                            existingIngredient.PendingType = null;
                            existingIngredient.PendingDescription = null;
                        }

                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("Name", $"Another ingredient with name '{ingredientVM.Name}' and type '{ingredientVM.Type}' already exists!");
                    }
                        
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IngredientExists(ingredientVM.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(ingredientVM);
        }

        // GET: Ingredients/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        // POST: Ingredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient != null)
            {
                _context.Ingredients.Remove(ingredient);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IngredientExists(int? id)
        {
            return _context.Ingredients.Any(e => e.Id == id);
        }

        private bool IngredientExists(string? name, string? type)
        {
            return _context.Ingredients.Any(i => i.Name == name && i.Type == type);
        }

        private bool IngredientProposedExists(string? name, string? type)
        {
            return _context.Ingredients.Any(i =>
                (i.Name == name && i.Type == type) ||
                (i.PendingName == name && i.PendingType == type)
            );
        }

        private bool IngredientProposedExistsExcludingId(string? name, string? type, int? excludeId)
        {
            return _context.Ingredients.Any(i =>
                i.Id != excludeId &&
                ((i.Name == name && i.Type == type) ||
                 (i.PendingName == name && i.PendingType == type))
            );
        }
    }
}
