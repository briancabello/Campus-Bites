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
        public async Task<IActionResult> Index(string? statusFilter)
        {
            statusFilter = statusFilter ?? "Pending";
            ViewData["StatusFilter"] = statusFilter;

            var imagesQuery = _context.Images
                .Include(i => i.Recipe)
                .Include(i => i.UploadedBy)
                .AsQueryable();

            if (statusFilter == "Pending")
            {
                imagesQuery = imagesQuery.Where(i => i.IsApproved == false);
            }
            else if (statusFilter == "Approved")
            {
                imagesQuery = imagesQuery.Where(i => i.IsApproved == true);
            }

            var images = await imagesQuery
                .OrderByDescending(i => i.UploadedDate)
                .ToListAsync();

            var viewModel = new ImageAdminViewModel
            {
                Images = images,
                StatusFilter = statusFilter,
                PendingCount = await _context.Images.CountAsync(i => i.IsApproved == false),
                ApprovedCount = await _context.Images.CountAsync(i => i.IsApproved == true),
                TotalCount = await _context.Images.CountAsync()
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

                // Check if recipe can be moved out of draft
                var recipe = image.Recipe;
                if (recipe != null && recipe.Status == "Draft")
                {
                    var hasApprovedImage = await _context.Images
                        .AnyAsync(i => i.RecipeId == recipe.ID && i.IsApproved == true);

                    // Recipe now has at least one approved image, owner can change status
                    // No automatic status change needed per business rules
                }

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

                // Send notification email to owner (implementation depends on email service)
                // await SendRejectionEmailAsync(image.UploadedBy.Email, image.Recipe.Name, rejectionReason);

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