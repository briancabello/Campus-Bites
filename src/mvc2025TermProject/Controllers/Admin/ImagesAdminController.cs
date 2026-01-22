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

        // GET: ImagesAdmin - Recipe Cards with Pending Images
        public async Task<IActionResult> Index(string? statusFilter, int? pageNumber)
        {
            const int pageSize = 12;

            // Default to Pending
            statusFilter = statusFilter ?? "Pending";
            ViewData["CurrentStatus"] = statusFilter;

            // Get recipes that have images in the selected status
            var recipesQuery = _context.Recipes
                .Include(r => r.Images!)
                    .ThenInclude(i => i.UploadedBy)
                .Include(r => r.Owner)
                .Where(r => r.Images!.Any()) // Has images
                .AsQueryable();

            // Filter by image status
            if (statusFilter == "Pending")
            {
                recipesQuery = recipesQuery.Where(r => r.Images!.Any(i => i.IsApproved == false));
            }
            else if (statusFilter == "Approved")
            {
                recipesQuery = recipesQuery.Where(r => r.Images!.Any(i => i.IsApproved == true));
            }

            // Get counts for filter tabs
            var allRecipes = _context.Recipes.Include(r => r.Images!).Where(r => r.Images!.Any());
            ViewData["PendingCount"] = await allRecipes.CountAsync(r => r.Images!.Any(i => i.IsApproved == false));
            ViewData["ApprovedCount"] = await allRecipes.CountAsync(r => r.Images!.Any(i => i.IsApproved == true));
            int totalCount = await allRecipes.CountAsync();

            // Create view models
            var recipeListQuery = recipesQuery.Select(r => new RecipeImageReviewViewModel
            {
                RecipeId = r.ID,
                RecipeName = r.Name,
                RecipeOwnerName = r.Owner != null ? $"{r.Owner.FirstName} {r.Owner.LastName}" : "Unknown",
                PendingImageCount = r.Images!.Count(i => i.IsApproved == false),
                ApprovedImageCount = r.Images!.Count(i => i.IsApproved == true),
                TotalImageCount = r.Images!.Count(),
                MainImagePath = r.Images!.FirstOrDefault(i => i.IsMainImage == true) != null
                    ? r.Images!.First(i => i.IsMainImage == true).FilePath
                    : r.Images!.FirstOrDefault() != null
                        ? r.Images!.First().FilePath
                        : null,
                LatestUploadDate = r.Images!.Max(i => i.UploadedDate)
            });

            // Sort by latest upload date
            recipeListQuery = recipeListQuery.OrderByDescending(r => r.LatestUploadDate);

            // Pagination
            var paginatedList = await PaginatedList<RecipeImageReviewViewModel>
                .CreateAsync(recipeListQuery, pageNumber ?? 1, pageSize);

            // View model
            var viewModel = new ImageAdminIndexViewModel
            {
                Recipes = paginatedList,
                StatusFilter = statusFilter,
                PendingCount = (int)ViewData["PendingCount"]!,
                ApprovedCount = (int)ViewData["ApprovedCount"]!,
                TotalCount = totalCount
            };

            return View(viewModel);
        }

        // GET: ImagesAdmin/Review/5 - Review images for a specific recipe
        public async Task<IActionResult> Review(int? id, int? currentImageIndex)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Images!)
                    .ThenInclude(i => i.UploadedBy)
                .Include(r => r.Owner)
                .FirstOrDefaultAsync(r => r.ID == id);

            if (recipe == null || recipe.Images == null || !recipe.Images.Any())
            {
                return NotFound();
            }

            // Get all images for this recipe, ordered by upload date
            var images = recipe.Images.OrderBy(i => i.UploadedDate).ToList();

            // Set current image index (default to 0)
            int index = currentImageIndex ?? 0;
            if (index < 0 || index >= images.Count)
            {
                index = 0;
            }

            var currentImage = images[index];

            // Create view model
            var viewModel = new ImageReviewViewModel
            {
                RecipeId = recipe.ID!.Value,
                RecipeName = recipe.Name,
                RecipeOwnerName = recipe.Owner != null ? $"{recipe.Owner.FirstName} {recipe.Owner.LastName}" : "Unknown",
                CurrentImage = currentImage,
                CurrentImageIndex = index,
                TotalImages = images.Count,
                HasPreviousImage = index > 0,
                HasNextImage = index < images.Count - 1,
                AllImageThumbnails = images
            };

            return View(viewModel);
        }

        // POST: ImagesAdmin/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, int recipeId, int currentImageIndex)
        {
            var image = await _context.Images
                .Include(i => i.Recipe)
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

                TempData["Success"] = $"Image approved successfully!";

                // Check if there are more pending images for this recipe
                var remainingPendingImages = await _context.Images
                    .Where(i => i.RecipeId == recipeId && i.IsApproved == false)
                    .CountAsync();

                if (remainingPendingImages > 0)
                {
                    // Stay on Review page, move to next image
                    return RedirectToAction(nameof(Review), new { id = recipeId, currentImageIndex = currentImageIndex });
                }
                else
                {
                    // All images reviewed, go back to Index
                    TempData["Success"] = $"All images for this recipe have been reviewed!";
                    return RedirectToAction(nameof(Index), new { statusFilter = "Pending" });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error approving image: {ex.Message}";
                return RedirectToAction(nameof(Review), new { id = recipeId, currentImageIndex = currentImageIndex });
            }
        }

        // POST: ImagesAdmin/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, int recipeId, int currentImageIndex, string? rejectionReason)
        {
            var image = await _context.Images
                .Include(i => i.Recipe)
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

                // TODO: Send rejection email with reason

                TempData["Success"] = $"Image rejected and deleted.";

                // Check if there are more pending images for this recipe
                var remainingPendingImages = await _context.Images
                    .Where(i => i.RecipeId == recipeId && i.IsApproved == false)
                    .CountAsync();

                if (remainingPendingImages > 0)
                {
                    // Stay on Review page, adjust index if needed
                    int nextIndex = currentImageIndex;
                    if (nextIndex >= remainingPendingImages)
                    {
                        nextIndex = remainingPendingImages - 1;
                    }
                    return RedirectToAction(nameof(Review), new { id = recipeId, currentImageIndex = nextIndex });
                }
                else
                {
                    // All images reviewed, go back to Index
                    TempData["Success"] = $"All images for this recipe have been reviewed!";
                    return RedirectToAction(nameof(Index), new { statusFilter = "Pending" });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error rejecting image: {ex.Message}";
                return RedirectToAction(nameof(Review), new { id = recipeId, currentImageIndex = currentImageIndex });
            }
        }

        // POST: ImagesAdmin/SetMainImage/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetMainImage(int id, int recipeId, int currentImageIndex)
        {
            var image = await _context.Images.FindAsync(id);

            if (image == null)
            {
                return NotFound();
            }

            try
            {
                // Reset all images for this recipe to not be main
                var recipeImages = await _context.Images
                    .Where(i => i.RecipeId == recipeId)
                    .ToListAsync();

                foreach (var img in recipeImages)
                {
                    img.IsMainImage = false;
                }

                // Set the selected image as main
                image.IsMainImage = true;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Main image updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error setting main image: {ex.Message}";
            }

            return RedirectToAction(nameof(Review), new { id = recipeId, currentImageIndex = currentImageIndex });
        }
    }
}
