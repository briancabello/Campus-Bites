using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc2025TermProject.Data;
using mvc2025TermProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc2025TermProject.Controllers.Admin
{
    [Authorize(Roles = "Administrator")]
    public class IngredientsAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IngredientsAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: IngredientsAdmin
        [HttpGet]
        public async Task<IActionResult> Index(string? sortOrder, string? filterCriteria, string? searchString, string? statusFilter, int? pageNumber)
        {
            const int pageSize = 10;

            var ingredientList = _context.Ingredients
                .Select(i => new IngredientAdminListViewModel()
                    {
                        ID              = i.Id,
                        Name            = i.Name,
                        RequestType     = (i.IsApproved == false && i.IsPendingModification == true) ? "New Ingredient" :
                                          (i.IsApproved == true && i.IsPendingModification == true) ? "Update Request" :
                                          "Existing Ingredient",
                        CreatedBy       = i.CreatedBy,
                        CreatedDate     = i.CreatedDate,
                        ReviewStatus    = (i.IsApproved == false && i.IsPendingModification == true) ? "Pending Approval" :
                                          (i.IsApproved == true && i.IsPendingModification == false) ? "Approved" :
                                          (i.IsApproved == true && i.IsPendingModification == true) ? "Pending Update" :
                                          "Rejected"
                    }
                );

            // SORTING PARAMETERS
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : null;
            ViewData["TypeSortParam"] = (sortOrder) == "type" ? "type_desc" : "type";
            ViewData["DateSortParam"] = (sortOrder) == "date" ? "date_desc" : "date";
            ViewData["StatusSortParam"] = (sortOrder) == "status" ? "status_desc" : "status";
            ViewData["RequestTypeSortParam"] = (sortOrder) == "request" ? "request_desc" : "request";
            ViewData["CurrentSort"] = sortOrder;
            ViewData["SearchFilterCriteria"] = filterCriteria;
            ViewData["CurrentStatus"] = statusFilter;

            // Handle search string and pagination reset
            if (searchString != null)
                pageNumber = 1;
            else
                searchString = filterCriteria;

            // Apply filtering
            if (!(String.IsNullOrEmpty(filterCriteria) || String.IsNullOrWhiteSpace(filterCriteria)))
            {
                ingredientList = ingredientList.Where(i =>
                    i.Name!.Contains(filterCriteria) ||
                    i.RequestType!.Contains(filterCriteria) ||
                    i.RequestType!.Contains(filterCriteria) ||
                    i.CreatedBy!.Contains(filterCriteria) ||
                    i.ReviewStatus!.Contains(filterCriteria)
                );
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter == "Pending")
                {
                    ingredientList = ingredientList.Where(i => i.ReviewStatus == "Pending Approval" || i.ReviewStatus == "Pending Update");
                }
                else
                {
                    ingredientList = ingredientList.Where(i => i.ReviewStatus == statusFilter);
                }
            }

            // Get counts for dropdown
            var allIngredients = _context.Ingredients.AsQueryable();
            ViewData["PendingCount"] = await allIngredients.CountAsync(i =>
                (i.IsApproved == false && i.IsPendingModification == true) ||
                (i.IsApproved == true && i.IsPendingModification == true));
            ViewData["ApprovedCount"] = await allIngredients.CountAsync(i =>
                i.IsApproved == true && i.IsPendingModification == false);
            ViewData["RejectedCount"] = await allIngredients.CountAsync(i =>
                i.IsApproved == false && i.IsPendingModification == false);

            // Total count for search results
            int totalCount = await ingredientList.CountAsync();
            ViewData["TotalMatches"] = totalCount;
            ViewData["HasSearch"] = !string.IsNullOrEmpty(filterCriteria) || !string.IsNullOrEmpty(statusFilter);

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
                case "type":
                    ingredientList = ingredientList.OrderBy(i => i.RequestType);
                    break;
                case "type_desc":
                    ingredientList = ingredientList.OrderByDescending(i => i.RequestType);
                    break;
                case "status":
                    ingredientList = ingredientList.OrderBy(i => i.ReviewStatus);
                    break;
                case "status_desc":
                    ingredientList = ingredientList.OrderByDescending(i => i.ReviewStatus);
                    break;
                case "request":
                    ingredientList = ingredientList.OrderBy(i => i.RequestType);
                    break;
                case "request_desc":
                    ingredientList = ingredientList.OrderByDescending(i => i.RequestType);
                    break;
                default:
                    ingredientList = ingredientList.OrderBy(i => i.Name);
                    break;
            }

            if (pageNumber == null)
                pageNumber = 1;

            return View(await PaginatedList<IngredientAdminListViewModel>.CreateAsync(ingredientList, (int)pageNumber, pageSize));
        }

        // GET: IngredientsAdmin/Details/5
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

            var ingredientDetailsViewModel = new IngredientDetailsAdminViewModel()
            {
                ID                          = ingredient.Id,
                CurrentName                 = ingredient.Name,
                ProposedName                = ingredient.PendingName,
                CurrentType                 = ingredient.Type,
                ProposedType                = ingredient.PendingType,
                CurrentDescription          = ingredient.Description,
                ProposedDescription         = ingredient.PendingDescription,
                CurrentStatus               = ingredient.IsApproved == true ? "Approved" : "Pending",
                HasPendingModifications     = ingredient.IsPendingModification == true,
                CreatedBy                   = ingredient.CreatedBy,
                CreatedDate                 = ingredient.CreatedDate,
                LastModifiedDate            = ingredient.LastModifiedDate,
                ApprovedDate                = ingredient.ApprovedDate,
                DecidedBy                   = ingredient.DecidedBy,
                RequestType                 = (ingredient.IsApproved == false && ingredient.IsPendingModification == true) ? "New Ingredient" :
                                              (ingredient.IsApproved == true && ingredient.IsPendingModification == true) ? "Update Request" :
                                              "Existing Ingredient",

                ReviewStatus                = (ingredient.IsApproved == false && ingredient.IsPendingModification == true) ? "Pending Approval" :
                                              (ingredient.IsApproved == true && ingredient.IsPendingModification == false) ? "Approved" :
                                              (ingredient.IsApproved == true && ingredient.IsPendingModification == true) ? "Pending Update" :
                                              "Rejected",
            };

            return View(ingredientDetailsViewModel);
        }

        // POST: IngredientsAdmin/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int? id)
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

            // PENDING APPROVAL
            if (ingredient.IsApproved == false && ingredient.IsPendingModification == true)
            {
                ingredient.Name                     = ingredient.PendingName ?? ingredient.Name;
                ingredient.Type                     = ingredient.PendingType ?? ingredient.Type;
                ingredient.Description              = ingredient.PendingDescription ?? ingredient.Description;
                ingredient.IsApproved               = true;
                ingredient.IsPendingModification    = false;
                ingredient.ApprovedDate             = DateTime.Today;
                ingredient.DecidedBy                = User.Identity?.Name; // Set current admin user
            }
            // UPDATE REQUEST
            else if (ingredient.IsApproved == true && ingredient.IsPendingModification == true)
            {
                ingredient.Name                     = ingredient.PendingName ?? ingredient.Name;
                ingredient.Type                     = ingredient.PendingType ?? ingredient.Type;
                ingredient.Description              = ingredient.PendingDescription ?? ingredient.Description;
                ingredient.IsPendingModification    = false;
                ingredient.LastModifiedDate         = DateTime.Now;
                ingredient.DecidedBy                = User.Identity?.Name; // Set current admin user
            }

            ingredient.PendingName          = null;
            ingredient.PendingType          = null;
            ingredient.PendingDescription   = null;

            try
            {
                _context.Update(ingredient);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IngredientExists(ingredient.Id))
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

        // POST: IngredientsAdmin/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int? id)
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

            // PENDING APPROVAL
            if (ingredient.IsApproved == false && ingredient.IsPendingModification == true)
            {
                ingredient.IsApproved               = false;
                ingredient.IsPendingModification    = false;
            }
            // UPDATE REQUEST
            else if (ingredient.IsApproved == true && ingredient.IsPendingModification == true)
            {
                ingredient.IsPendingModification = false;
            }

            ingredient.PendingName          = null;
            ingredient.PendingType          = null;
            ingredient.PendingDescription   = null;
            ingredient.DecidedBy            = User.Identity?.Name; // Set current admin user
            ingredient.LastModifiedDate     = DateTime.Today;

            try
            {
                _context.Update(ingredient);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IngredientExists(ingredient.Id))
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

        // GET: IngredientsAdmin/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: IngredientsAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Type,Description,IsApproved,CreatedDate,CreatedBy,DecidedBy,PendingName,PendingType,PendingDescription,IsPendingModification,LastModifiedDate,ApprovedDate")] Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ingredient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ingredient);
        }

        // GET: IngredientsAdmin/Edit/5
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
            return View(ingredient);
        }

        // POST: IngredientsAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Name,Type,Description,IsApproved,CreatedDate,CreatedBy,DecidedBy,PendingName,PendingType,PendingDescription,IsPendingModification,LastModifiedDate,ApprovedDate")] Ingredient ingredient)
        {
            if (id != ingredient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ingredient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IngredientExists(ingredient.Id))
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
            return View(ingredient);
        }

        // GET: IngredientsAdmin/Delete/5
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

        // POST: IngredientsAdmin/Delete/5
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
    }
}
