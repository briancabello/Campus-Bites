using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc2025TermProject.Data;
using mvc2025TermProject.Models;
using System.Security.Claims;

namespace mvc2025TermProject.Controllers.Admin
{
    [Authorize(Roles = "Administrator")]
    public class ImagesAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<CampusBitesUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public ImagesAdminController(
            ApplicationDbContext context,
            UserManager<CampusBitesUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        // GET: ImagesAdmin
        public async Task<IActionResult> Index(
            string? statusFilter,
            string? sortOrder,
            string? filterCriteria,
            string? searchString,
            int? pageNumber)
        {
            const int pageSize = 12; // 12 images per page (3x4 grid)

            // Set default status filter to Pending
            statusFilter = statusFilter ?? "Pending";

            // Set up sorting parameters
            ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : null;
            ViewData["DateSortParam"] = sortOrder == "date" ? "date_desc" : "date";
            ViewData["StatusSortParam"] = sortOrder == "status" ? "status_desc" : "status";
            ViewData["CurrentSort"] = sortOrder;
            ViewData["StatusFilter"] = statusFilter;
            ViewData["SearchFilterCriteria"] = filterCriteria;

            // Handle search string and pagination reset
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = filterCriteria;
            }

            // Build the base query
            var imagesQuery = _context.Images
                .Include(i => i.Recipe)
                .Include(i => i.UploadedBy)
                .AsQueryable();

            // Apply status filter
            if (statusFilter == "Pending")
            {
                imagesQuery = imagesQuery.Where(i => i.IsApproved == false);
            }
            else if (statusFilter == "Approved")
            {
                imagesQuery = imagesQuery.Where(i => i.IsApproved == true);
            }
            // "All" shows everything - no filter applied

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString) && !string.IsNullOrWhiteSpace(searchString))
            {
                imagesQuery = imagesQuery.Where(i =>
                    i.Recipe!.Name!.Contains(searchString) ||
                    i.UploadedBy!.FirstName!.Contains(searchString) ||
                    i.UploadedBy!.LastName!.Contains(searchString) ||
                    i.FileName!.Contains(searchString)
                );
            }

            // Get counts for the status tabs
            int pendingCount = await _context.Images.CountAsync(i => i.IsApproved == false);
            int approvedCount = await _context.Images.CountAsync(i => i.IsApproved == true);
            int totalCount = await _context.Images.CountAsync();

            // Total count for search results
            int searchTotalCount = await imagesQuery.CountAsync();
            ViewData["TotalMatches"] = searchTotalCount;
            ViewData["HasSearch"] = !string.IsNullOrEmpty(filterCriteria);

            // Apply sorting
            switch (sortOrder)
            {
                case "date":
                    imagesQuery = imagesQuery.OrderBy(i => i.UploadedDate);
                    break;
                case "date_desc":
                    imagesQuery = imagesQuery.OrderByDescending(i => i.UploadedDate);
                    break;
                case "name_desc":
                    imagesQuery = imagesQuery.OrderByDescending(i => i.Recipe!.Name);
                    break;
                case "status":
                    imagesQuery = imagesQuery.OrderBy(i => i.IsApproved);
                    break;
                case "status_desc":
                    imagesQuery = imagesQuery.OrderByDescending(i => i.IsApproved);
                    break;
                default:
                    imagesQuery = imagesQuery.OrderBy(i => i.Recipe!.Name);
                    break;
            }

            // Set page number
            if (pageNumber == null)
            {
                pageNumber = 1;
            }

            // Create paginated list
            var paginatedImages = await PaginatedList<Image>.CreateAsync(
                imagesQuery,
                pageNumber.Value,
                pageSize);

            // Create view model
            var viewModel = new ImageAdminViewModel
            {
                Images = paginatedImages.ToList(),
                StatusFilter = statusFilter,
                PendingCount = pendingCount,
                ApprovedCount = approvedCount,
                TotalCount = totalCount,
                PaginatedList = paginatedImages
            };

            return View(viewModel);
        }

        // POST: ImagesAdmin/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var image = await _context.Images
                .Include(i => i.Recipe)
                .Include(i => i.UploadedBy)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (image == null)
            {
                return NotFound();
            }

            try
            {
                // Move image from temp to main folder
                string tempPath = Path.Combine(_environment.WebRootPath, "img", "temp", image.FileName);
                string mainPath = Path.Combine(_environment.WebRootPath, "img", "recipes");

                if (!Directory.Exists(mainPath))
                {
                    Directory.CreateDirectory(mainPath);
                }

                string destinationPath = Path.Combine(mainPath, image.FileName);

                // Move the physical file
                if (System.IO.File.Exists(tempPath))
                {
                    System.IO.File.Move(tempPath, destinationPath, overwrite: true);
                }

                // Update database record
                image.FilePath = $"/img/recipes/{image.FileName}";
                image.IsApproved = true;
                image.ApprovedById = User.FindFirstValue(ClaimTypes.NameIdentifier);
                image.ApprovedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["Success"] = $"Image approved successfully! Owner has been notified via email.";
                return RedirectToAction(nameof(Index), new { statusFilter = "Pending" });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error approving image: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ImagesAdmin/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string? rejectionReason)
        {
            var image = await _context.Images
                .Include(i => i.Recipe)
                .Include(i => i.UploadedBy)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (image == null)
            {
                return NotFound();
            }

            try
            {
                // Delete physical file from temp folder
                string tempPath = Path.Combine(_environment.WebRootPath, "img", "temp", image.FileName);

                if (System.IO.File.Exists(tempPath))
                {
                    System.IO.File.Delete(tempPath);
                }

                // Remove from database
                _context.Images.Remove(image);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Image rejected and deleted. Owner has been notified via email.";
                return RedirectToAction(nameof(Index), new { statusFilter = "Pending" });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error rejecting image: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: ImagesAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images
                .Include(i => i.Recipe)
                .Include(i => i.UploadedBy)
                .Include(i => i.ApprovedBy)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }
    }
}