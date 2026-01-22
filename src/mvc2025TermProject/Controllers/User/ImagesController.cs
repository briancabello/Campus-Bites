using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

using mvc2025TermProject.Data;
using mvc2025TermProject.Models;
using mvc2025TermProject.Helpers;

namespace mvc2025TermProject.Controllers.User
{
    [Authorize(Roles = "User, Administrator")]
    public class ImagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<CampusBitesUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public ImagesController(ApplicationDbContext context, UserManager<CampusBitesUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        // GET: Upload
        [HttpGet]
        public async Task<IActionResult> Upload(int? id)
        {

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await LoadRecipesDropdown(id);

            var userRecipes = await _context.Recipes
                 .Where(r => r.OwnerId == currentUserId)
                 .OrderBy(r => r.Name)
                 .Select(r => new { r.ID, r.Name })
                 .ToListAsync();

            ViewData["RecipeId"] = new SelectList(userRecipes, "ID", "Name", id);

            // If no recipes exist, return empty view
            if (!userRecipes.Any())
            {
                return View(new ImageUploadViewModel());
            }

            // If recipe is selected, load its image gallery
            if (id.HasValue)
            {
                var selectedRecipe = await _context.Recipes
                    .Include(r => r.Images)
                    .Include(r => r.Owner)
                    .FirstOrDefaultAsync(r => r.ID == id && r.OwnerId == currentUserId);

                if (selectedRecipe != null)
                {
                    var imageUploadVM = new ImageUploadViewModel
                    {
                        RecipeId        = selectedRecipe.ID,
                        RecipeName      = selectedRecipe.Name,
                        OwnerId         = selectedRecipe.OwnerId,
                        OwnerName       = selectedRecipe.Owner != null ? $"{selectedRecipe.Owner.FirstName} {selectedRecipe.Owner.LastName}" : "Unknown User",
                        Images          = selectedRecipe.Images?.ToList() ?? new List<Image>(),
                        CurrentUserId   = currentUserId,
                        RecipeStatus    = selectedRecipe.Status
                    };

                    return View(imageUploadVM);
                }
            }

            return View(new ImageUploadViewModel());
        }

        // POST: Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(int? id, IFormFile? imageFile, string? altText, string? description, bool isMainImage)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            const int fileSizeLimit = 2097152; // 2 MB

            // Get the recipe
            var existingRecipe = await _context.Recipes.FirstOrDefaultAsync(r => r.ID == id);

            if (existingRecipe == null)
            {
                return View("NotFound");
            }

            // ONLY OWNER OR ADMIN CAN UPLOAD IMAGES
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (existingRecipe.OwnerId != currentUserId && !User.IsInRole("Administrator"))
            {
                return Forbid();
            }

            var imageUploadVM = new ImageUploadViewModel
            {
                RecipeId        = id.Value,
                RecipeName      = existingRecipe.Name,
                OwnerId         = existingRecipe.OwnerId,
                OwnerName       = existingRecipe.Owner != null ? $"{existingRecipe.Owner.FirstName} {existingRecipe.Owner.LastName}" : "Unknown User",
                Images          = existingRecipe.Images?.ToList() ?? new List<Image>(),
                CurrentUserId   = currentUserId
            };

            if (existingRecipe.Status == "Private")
            {
                ModelState.AddModelError("", "Cannot upload images to private recipes. Change recipe status to draft first.");
                await LoadRecipesDropdown(id.Value);
                return View();
            }

            // SHOULD ALSO BLOCK Public and Unlisted
            if (existingRecipe.Status != "Draft" && existingRecipe.Status != "Initial")
            {
                ModelState.AddModelError("", "Cannot upload images. Recipe must be in Draft status to modify images.");
                await LoadRecipesDropdown(id.Value);
                return View(imageUploadVM);
            }

            // CHECK MAX IMAGE COUNT
            if (existingRecipe.Images != null && existingRecipe.Images.Count >= 7)
            {
                ModelState.AddModelError("imageFile", "Maximum of 7 images per recipe reached. Please delete some images before uploading new ones.");
                await LoadRecipesDropdown(id.Value);
                return View(imageUploadVM);
            }

            // VALIDATE REQUIRED
            if (imageFile == null || imageFile.Length == 0)
            {
                ModelState.AddModelError("imageFile", "Please choose a file.");
                await LoadRecipesDropdown(id.Value);
                return View(imageUploadVM);
            }

            // VALIDATE FILE SIZE
            if (imageFile.Length > fileSizeLimit)
            {
                ModelState.AddModelError("imageFile", $"File exceeds maximum size of {fileSizeLimit / 1024 / 1024}MB.");
                await LoadRecipesDropdown(id.Value);
                return View(imageUploadVM);
            }

            // VALIDATE FILE TYPE
            string fileType = imageFile.ContentType;
            if (fileType != "image/jpeg" && fileType != "image/png" && fileType != "image/gif")
            {
                ModelState.AddModelError("imageFile", $"Unsupported file type: {fileType}. Please upload JPG, PNG, or GIF files.");
                await LoadRecipesDropdown(id.Value);
                return View(imageUploadVM);
            }

            // VALIDATION PASSED -> proceed with upload
            try
            {
                // Create temporary folder for unapproved images
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "temp");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Generate unique filename to prevent conflicts
                string fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                string fileName = Path.GetFileName(uniqueFileName);

                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // HANDLE MAIN IMAGE OVERRIDE
                if (isMainImage)
                {
                    var existingMainImages = await _context.Images
                        .Where(i => i.RecipeId == id && i.IsMainImage == true)
                        .ToListAsync();

                    foreach (var existingImage in existingMainImages)
                    {
                        existingImage.IsMainImage = false;
                    }
                }

                // Create Image in DB
                var image = new Image
                {
                    FileName        = fileName,
                    FilePath        = $"/img/temp/{fileName}",
                    FileSize        = imageFile.Length,
                    FileType        = fileExtension,
                    IsApproved      = false,
                    IsMainImage     = isMainImage,
                    RecipeId        = id.Value,
                    UploadedById    = currentUserId,
                    UploadedDate    = DateTime.Today,
                    AltText         = string.IsNullOrEmpty(altText) ? existingRecipe.Name : altText,
                    Description     = description
                };

                await _context.Images.AddAsync(image);
                await _context.SaveChangesAsync();

                if (existingRecipe.Status == "Initial")
                {
                    existingRecipe.Status           = "Draft";
                    existingRecipe.LastModifiedDate = DateTime.Today;

                    await _context.SaveChangesAsync();
                }

                // WIP : SUCCESS MSG
                TempData["Success"] = "Image uploaded successfully! It will be visible after admin approval.";
                return RedirectToAction(nameof(Upload), new { recipeId = id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error uploading image: {ex.Message}");
                await LoadRecipesDropdown(id.Value);
                return View(imageUploadVM);
            }
        }

        private async Task LoadRecipesDropdown(int? selectedRecipeId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRecipes = await _context.Recipes
                .Where(r => r.OwnerId == currentUserId)
                .OrderBy(r => r.Name)
                .Select(r => new { r.ID, r.Name })
                .ToListAsync();

            ViewData["RecipeId"] = new SelectList(userRecipes, "ID", "Name", selectedRecipeId);
        }

        // POST: Images/SetAsMain/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetAsMain(int id)
        {
            var image = await _context.Images
                .Include(i => i.Recipe)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (image == null)
            {
                return NotFound();
            }

            // Authorization check
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (image.Recipe.OwnerId != currentUserId && !User.IsInRole("Administrator"))
            {
                return Forbid();
            }

            if (image.Recipe.Status != "Draft" && image.Recipe.Status != "Initial")
            {
                TempData["Error"] = "Cannot modify main image. Recipe must be in Draft status.";
                return RedirectToAction(nameof(Upload), new { id = image.RecipeId });
            }

            // Reset all images for this recipe to not be main
            var recipeImages = await _context.Images
                .Where(i => i.RecipeId == image.RecipeId)
                .ToListAsync();

            foreach (var recipeImage in recipeImages)
            {
                recipeImage.IsMainImage = false;
            }

            // Set the selected image as main
            image.IsMainImage = true;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Main image updated successfully!";
            return RedirectToAction(nameof(Upload), new { id = image.RecipeId });
        }

        // POST: Images/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var image = await _context.Images
                .Include(i => i.Recipe)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (image == null)
            {
                return NotFound();
            }

            // Authorization check
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (image.Recipe.OwnerId != currentUserId && !User.IsInRole("Administrator"))
            {
                return Forbid();
            }

            if (image.Recipe.Status != "Draft" && image.Recipe.Status != "Initial")
            {
                TempData["Error"] = "Cannot delete images. Recipe must be in Draft status.";
                return RedirectToAction(nameof(Upload), new { id = image.RecipeId });
            }

            try
            {
                // DELETE PHYSICAL FILE
                string filePath = Path.Combine(this._environment.WebRootPath, image.FilePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                _context.Images.Remove(image);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Image deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting image: {ex.Message}";
            }

            return RedirectToAction(nameof(Upload), new { id = image.RecipeId });
        }
    }
}
