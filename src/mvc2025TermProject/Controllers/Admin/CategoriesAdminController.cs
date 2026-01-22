using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc2025TermProject.Data;
using mvc2025TermProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace mvc2025TermProject.Controllers.Admin
{
    [Authorize(Roles = "Administrator")]
    public class CategoriesAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CategoriesAdmin
        [HttpGet]
        public async Task<IActionResult> Index(string? sortOrder, string? filterCriteria, string? searchString, string? statusFilter, int? pageNumber)
        {
            const int pageSize = 10;

            var categoryList = _context.Categories
                .Select(c => new CategoryAdminListViewModel()
                    {
                        ID              = c.ID,
                        Name            = c.Name,
                        RequestType     = (c.IsApproved == false && c.IsPendingModification == true) ? "New Category" :
                                          (c.IsApproved == true && c.IsPendingModification == true) ? "Update Request" :
                                          "Existing Category",
                        CreatedBy       = _context.Users.Where(u => u.Id == c.CreatedBy).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault(),
                        CreatedDate     = c.CreatedDate,
                        ReviewStatus    = (c.IsApproved == false && c.IsPendingModification == true) ? "Pending Approval" :
                                          (c.IsApproved == true && c.IsPendingModification == false) ? "Approved" :
                                          (c.IsApproved == true && c.IsPendingModification == true) ? "Pending Update" :
                                          "Rejected"
                    }
                );

            // SORTING PARAMETERS
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : null;
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
                categoryList = categoryList.Where(i =>
                    i.Name!.Contains(filterCriteria) ||
                    i.RequestType!.Contains(filterCriteria) ||
                    i.CreatedBy!.Contains(filterCriteria) ||
                    i.ReviewStatus!.Contains(filterCriteria)
                );
            }

            // Apply status filter from pills
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter == "Pending")
                {
                    categoryList = categoryList.Where(i => i.ReviewStatus == "Pending Approval" || i.ReviewStatus == "Pending Update");
                }
                else
                {
                    categoryList = categoryList.Where(i => i.ReviewStatus == statusFilter);
                }
            }

            // Total count for search results
            int totalCount = await categoryList.CountAsync();
            ViewData["TotalMatches"] = totalCount;
            ViewData["HasSearch"] = !string.IsNullOrEmpty(filterCriteria) || !string.IsNullOrEmpty(statusFilter);

            // Get counts for each status pill
            var allCategories = _context.Categories.AsQueryable();
            ViewData["PendingCount"] = await allCategories.CountAsync(c =>
                (c.IsApproved == false && c.IsPendingModification == true) ||
                (c.IsApproved == true && c.IsPendingModification == true));
            ViewData["ApprovedCount"] = await allCategories.CountAsync(c =>
                c.IsApproved == true && c.IsPendingModification == false);
            ViewData["RejectedCount"] = await allCategories.CountAsync(c =>
                c.IsApproved == false && c.IsPendingModification == false);

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
                    categoryList = categoryList.OrderBy(i => i.ReviewStatus);
                    break;
                case "status_desc":
                    categoryList = categoryList.OrderByDescending(i => i.ReviewStatus);
                    break;
                case "request":
                    categoryList = categoryList.OrderBy(i => i.RequestType);
                    break;
                case "request_desc":
                    categoryList = categoryList.OrderByDescending(i => i.RequestType);
                    break;
                default:
                    categoryList = categoryList.OrderBy(i => i.Name);
                    break;
            }

            if (pageNumber == null)
                pageNumber = 1;

            return View(await PaginatedList<CategoryAdminListViewModel>.CreateAsync(categoryList, (int)pageNumber, pageSize));
        }

        // GET: CategoriesAdmin/Details/5
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

            var categoryDetailsViewModel = new CategoryAdminDetailsViewModel()
            {
                ID                      = category.ID,
                CurrentName             = category.Name,
                ProposedName            = category.PendingName,
                CurrentDescription      = category.Description,
                ProposedDescription     = category.PendingDescription,
                CurrentStatus           = category.IsApproved == true ? "Approved" : "Pending",
                HasPendingModifications = category.IsPendingModification == true,
                CreatedBy               = createdByUser,
                CreatedDate             = category.CreatedDate,
                LastModifiedDate        = category.LastModifiedDate,
                ApprovedDate            = category.ApprovedDate,
                DecidedBy               = decidedByUser,
                RequestType             = (category.IsApproved == false && category.IsPendingModification == true) ? "New" :
                                          (category.IsApproved == true && category.IsPendingModification == true) ? "Update" :
                                          "Existing",

                ReviewStatus            = (category.IsApproved == false && category.IsPendingModification == true) ? "Pending Approval" :
                                          (category.IsApproved == true && category.IsPendingModification == false) ? "Approved" :
                                          (category.IsApproved == true && category.IsPendingModification == true) ? "Pending Update" :
                                          "Rejected",

                RecipeCount             = category.Recipes != null ? category.Recipes.Count : 0,
            };

            return View(categoryDetailsViewModel);
        }

        // POST: CategoriesAdmin/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int? id)
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

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // If it's a new category (pending approval)
            if (category.IsApproved == false && category.IsPendingModification == true)
            {
                category.Name                   = category.PendingName ?? category.Name;
                category.Description            = category.PendingDescription ?? category.Description;
                category.IsApproved             = true;
                category.IsPendingModification  = false;
                category.ApprovedDate           = DateTime.Today;
                category.DecidedBy              = currentUserId; // Set current admin user
            }
            // If it's an update request
            else if (category.IsApproved == true && category.IsPendingModification == true)
            {
                category.Name                   = category.PendingName ?? category.Name;
                category.Description            = category.PendingDescription ?? category.Description;
                category.IsPendingModification  = false;
                category.LastModifiedDate       = DateTime.Now;
                category.DecidedBy              = currentUserId; // Set current admin user
            }

            category.PendingName = null;
            category.PendingDescription = null;

            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.ID))
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

        // POST: CategoriesAdmin/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int? id)
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

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // If it's a new category (pending approval) - reject it completely
            if (category.IsApproved == false && category.IsPendingModification == true)
            {
                category.IsApproved = false;
                category.IsPendingModification = false;
            }
            // If it's an update request - just clear the pending modifications
            else if (category.IsApproved == true && category.IsPendingModification == true)
            {
                category.IsPendingModification = false;
            }

            category.PendingName        = null;
            category.PendingDescription = null;
            category.DecidedBy          = currentUserId; // Set current admin user
            category.LastModifiedDate   = DateTime.Today;

            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.ID))
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

        // GET: CategoriesAdmin/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategoriesAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Description,IsApproved,CreatedDate,CreatedBy,DecidedBy,PendingName,PendingDescription,IsPendingModification,LastModifiedDate,ApprovedDate")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: CategoriesAdmin/Edit/5
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
            return View(category);
        }

        // POST: CategoriesAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("ID,Name,Description,IsApproved,CreatedDate,CreatedBy,DecidedBy,PendingName,PendingDescription,IsPendingModification,LastModifiedDate,ApprovedDate")] Category category)
        {
            if (id != category.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.ID))
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
            return View(category);
        }

        // GET: CategoriesAdmin/Delete/5
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

        // POST: CategoriesAdmin/Delete/5
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
    }
}
