using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc2025TermProject.Data;
using mvc2025TermProject.Models;

namespace mvc2025TermProject.Controllers.User
{
    [Authorize(Roles = "User, Administrator")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        [HttpGet]
        public async Task<IActionResult> Index(string? sortOrder, string? filterCriteria, string? searchString, int? pageNumber)
        {
            const int pageSize = 10;

            var categoryList = _context.Categories
                .Select(c => new CategoryListViewModel()
                    {
                        ID          = c.ID,
                        Name        = c.Name,
                        Description = c.Description,
                        Status      = (c.IsApproved == false && c.IsPendingModification == true) ? "Pending Approval" :
                                      (c.IsApproved == true && c.IsPendingModification == false) ? "Approved" :
                                      (c.IsApproved == true && c.IsPendingModification == true) ? "Pending Update" :
                                      "Rejected",

                        RecipeCount = c.Recipes != null ? c.Recipes.Count : 0,
                        CreatedDate = c.CreatedDate,
                        UnderReview = (c.IsPendingModification == true) ? true : false
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
                categoryList = categoryList.Where(i =>
                    i.Name!.Contains(filterCriteria) ||
                    i.Description!.Contains(filterCriteria) ||
                    i.Status!.Contains(filterCriteria)
                );
            }

            // Total count for search results
            int totalCount = await categoryList.CountAsync();
            ViewData["TotalMatches"] = totalCount;
            ViewData["HasSearch"] = !string.IsNullOrEmpty(filterCriteria);

            switch (sortOrder)
            {
                case "date":
                    categoryList = categoryList.OrderBy(i => i.CreatedDate);
                    break;

                case "date_desc":
                    categoryList = categoryList.OrderByDescending(i => i.CreatedDate);
                    break;

                case "name_desc":
                    categoryList = categoryList.OrderByDescending(i => i.Name);
                    break;

                case "status":
                    categoryList = categoryList.OrderBy(i => i.Status);
                    break;

                case "status_desc":
                    categoryList = categoryList.OrderByDescending(i => i.Status);
                    break;

                default:
                    categoryList = categoryList.OrderBy(i => i.Name);
                    break;
            }

            if (pageNumber == null)
                pageNumber = 1;

            return View(await PaginatedList<CategoryListViewModel>.CreateAsync(categoryList, (int)pageNumber, pageSize));
        }

        // GET: Categories/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FirstOrDefaultAsync(m => m.ID == id);
            if (category == null)
            {
                return NotFound();
            }

            // Get Full Names
            var createdByUser = _context.Users
                                    .Where(u => u.Id == category.CreatedBy)
                                    .Select(u => u.FirstName + " " + u.LastName)
                                    .FirstOrDefault();

            var decidedByUser = _context.Users
                                    .Where(u => u.Id == category.DecidedBy)
                                    .Select(u => u.FirstName + " " + u.LastName)
                                    .FirstOrDefault();

            var categoryVM = new CategoryDetailViewModel
            {
                ID                      = category.ID,
                Name                    = category.Name,
                Description             = category.Description,
                Status                  = (category.IsApproved == false && category.IsPendingModification == true) ? "Pending Approval" :
                                          (category.IsApproved == true && category.IsPendingModification == false) ? "Approved" :
                                          (category.IsApproved == true && category.IsPendingModification == true) ? "Pending Update" :
                                          "Rejected",
                IsPendingModification   = category.IsPendingModification,
                ProposedName            = category.PendingName,
                ProposedDescription     = category.PendingDescription,
                CreatedDate             = category.CreatedDate,
                CreatedBy               = createdByUser,
                LastModifiedDate        = category.LastModifiedDate,
                ApprovedDate            = category.ApprovedDate,
                DecidedBy               = decidedByUser
            };

            return View(categoryVM);
        }

        // GET: Categories/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,IsApproved,CreatedDate,CreatedBy,IsPendingModification")] CategoryFormFieldsViewModel categoryVM)
        {
            if (ModelState.IsValid)
            {
                // Validation to check if the category name already exists
                if (!CategoryProposedNameExists(categoryVM.Name))
                {
                    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    Category newCategory = new Category
                    {
                        Name                    = categoryVM.Name,
                        Description             = categoryVM.Description,
                        IsApproved              = false,
                        CreatedDate             = DateTime.Today,
                        CreatedBy               = currentUserId,
                        IsPendingModification   = true
                    };

                    _context.Add(newCategory);

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Name", $"This Category Name '{categoryVM.Name}' already exists.");
                }
            }

            return View(categoryVM);
        }

        // GET: Categories/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Only for approved categories with pending modifications, show pending values
            string? displayName = category.Name;
            string? displayDescription = category.Description;

            if (category.IsApproved == true && category.IsPendingModification == true)
            {
                displayName = 
                    category.PendingName != null ? category.PendingName : category.Name;

                displayDescription = 
                    category.PendingDescription != null ? category.PendingDescription : category.Description;
            }

            var categoryVM = new CategoryDetailViewModel
            {
                ID                      = category.ID,
                Name                    = displayName,
                Description             = displayDescription,
                Status                  = (category.IsApproved == false && category.IsPendingModification == true) ? "Pending Approval" :
                                          (category.IsApproved == true && category.IsPendingModification == false) ? "Approved" :
                                          (category.IsApproved == true && category.IsPendingModification == true) ? "Pending Update" :
                                          "Rejected",
                IsPendingModification   = category.IsPendingModification,
                ProposedName            = category.PendingName,
                ProposedDescription     = category.PendingDescription,
                CreatedDate             = category.CreatedDate,
                CreatedBy               = category.CreatedBy,
                LastModifiedDate        = category.LastModifiedDate,
                ApprovedDate            = category.ApprovedDate,
                DecidedBy               = category.DecidedBy
            };

            return View(categoryVM);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("ID,Name,Description")] CategoryDetailViewModel categoryVM)
        {
            if (id != categoryVM.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCategory = await _context.Categories.FindAsync(id);
                    if (existingCategory == null)
                    {
                        return NotFound();
                    }

                    // Check if any changes were made
                    bool nameChanged = existingCategory.Name != categoryVM.Name;
                    bool descriptionChanged = existingCategory.Description != categoryVM.Description;

                    if (!nameChanged && !descriptionChanged)
                        return RedirectToAction(nameof(Index));

                    if (existingCategory.IsApproved == true)
                    {
                        // Handle approved category edits
                        if (!HandleApprovedCategoryEdit(existingCategory, categoryVM, nameChanged))
                            return View(categoryVM);
                    }
                    else
                    {
                        // Handle non-approved category edits
                        if (!HandleNonApprovedCategoryEdit(existingCategory, categoryVM, nameChanged))
                            return View(categoryVM);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(categoryVM.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(categoryVM);
        }

        private bool HandleApprovedCategoryEdit(Category existingCategory, CategoryDetailViewModel categoryVM, bool nameChanged)
        {
            // Check for duplicate name if name changed
            if (nameChanged && CategoryProposedNameExistsExcludingId(categoryVM.Name, existingCategory.ID))
            {
                ModelState.AddModelError("Name", $"The Category Name '{categoryVM.Name}' already exists.");
                return false;
            }

            // Update pending fields
            existingCategory.PendingName            = categoryVM.Name;
            existingCategory.PendingDescription     = categoryVM.Description;
            existingCategory.IsPendingModification  = true;
            existingCategory.LastModifiedDate       = DateTime.Today;

            return true;
        }

        private bool HandleNonApprovedCategoryEdit(Category existingCategory, CategoryDetailViewModel categoryVM, bool nameChanged)
        {
            // Check for duplicate name if name changed
            if (nameChanged && CategoryNameExistsExcludingId(categoryVM.Name, existingCategory.ID))
            {
                ModelState.AddModelError("Name", $"The Category Name '{categoryVM.Name}' already exists.");
                return false;
            }

            // Update main fields directly
            existingCategory.Name                   = categoryVM.Name;
            existingCategory.Description            = categoryVM.Description;
            existingCategory.IsPendingModification  = true;
            existingCategory.LastModifiedDate       = DateTime.Today;

            // Clear pending fields
            existingCategory.PendingName = null;
            existingCategory.PendingDescription = null;

            return true;
        }

        // GET: Categories/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.ID == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int? id)
        {
            return _context.Categories.Any(e => e.ID == id);
        }

        // Check if category name exists in database
        private bool CategoryNameExists(string? name)
        {
            return _context.Categories.Any(e => e.Name == name);
        }

        // Check if proposed name exists in any pending names OR approved names
        private bool CategoryProposedNameExists(string? name)
        {
            return _context.Categories.Any(e => e.Name == name || e.PendingName == name);
        }

        // Check if category name exists in database excluding current category
        private bool CategoryNameExistsExcludingId(string? name, int? excludeId)
        {
            return _context.Categories.Any(e => e.ID != excludeId && e.Name == name);
        }

        // Check if proposed name exists in any pending names OR approved names excluding current category
        private bool CategoryProposedNameExistsExcludingId(string? name, int? excludeId)
        {
            return _context.Categories.Any(e => e.ID != excludeId && (e.Name == name || e.PendingName == name));
        }
    }
}
