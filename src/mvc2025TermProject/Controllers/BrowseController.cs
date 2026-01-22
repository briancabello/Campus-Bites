using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using mvc2025TermProject.Data;
using mvc2025TermProject.Models;
using EmailService;

namespace mvc2025TermProject.Controllers
{
    public class BrowseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public BrowseController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // GET: Browse
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var viewModel = new BrowseViewModel();

            // GET LATEST RECIPES (6 most recent)
            viewModel.LatestRecipes = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Images)
                .Where(r => r.Status == "Public")
                .OrderByDescending(r => r.CreatedDate)
                .Take(6)
                .Select(r => new RecipeCardViewModel
                {
                    ID              = r.ID,
                    Name            = r.Name,
                    CategoryName    = r.Category != null ? r.Category.Name : "Uncategorized",
                    TotalTime       = FormatTotalTime(r.PreparationTime, r.CookTime),
                    Servings        = r.Servings,
                    OwnerId         = r.OwnerId,
                    OwnerName       = _context.Users.Where(u => u.Id == r.OwnerId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault(),
                    CreatedDate     = r.CreatedDate,
                    Images          = r.Images!.Where(img => img.IsApproved == true && img.IsMainImage == true).ToList()
                })
                .ToListAsync();

            // GET CATEGORIES WITH RECIPE COUNTS
            viewModel.FeaturedCategories = await _context.Categories
                //.Where(c => c.IsApproved == true) // APPROVED CATEGORIES
                .Select(c => new CategoryCardViewModel
                {
                    ID          = c.ID,
                    Name        = c.Name,
                    Description = c.Description,
                    //RecipeCount = _context.Recipes.Count(r => r.CategoryId == c.ID && r.Status == "Public") // APPROVED CATEGORIES
                    Status      = (c.IsApproved == false && c.IsPendingModification == true) ? "Pending Approval" :
                                  (c.IsApproved == true && c.IsPendingModification == false) ? "Approved" :
                                  (c.IsApproved == true && c.IsPendingModification == true) ? "Pending Update" :
                                  "Rejected",
                    RecipeCount = _context.Recipes.Count(r => r.CategoryId == c.ID && r.Status == "Public")
                })
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(viewModel);
        }

        // GET: Browse/Category/5
        [HttpGet]
        public async Task<IActionResult> Category(int? id, string? filterCriteria, int? pageNumber)
        {
            if (id == null)
            {
                return NotFound();
            }

            const int pageSize = 10;

            // GET CATEGORY DETAILS
            //var category = await _context.Categories.FirstOrDefaultAsync(c => c.ID == id && c.IsApproved == true); // ONLY APPROVED CATEGORIES
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.ID == id);
            if (category == null)
            {
                return View("NotFound");
            }

            ViewData["CategoryName"] = category.Name;
            ViewData["CategoryDescription"] = category.Description;
            ViewData["SearchFilterCriteria"] = filterCriteria;

            // QUERY
            var recipeQuery = _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Images)
                .Include(r => r.RecipeIngredients!)
                    .ThenInclude(ri => ri.Ingredient)
                .Where(r => r.Status == "Public" && r.CategoryId == id)
                .OrderBy(r => r.Name)
                .AsQueryable();

            // SEARCH FILTER
            if (!string.IsNullOrEmpty(filterCriteria))
            {
                recipeQuery = recipeQuery.Where(r =>
                    r.Name!.Contains(filterCriteria) ||
                    r.OwnerId!.Contains(filterCriteria) ||
                    r.RecipeIngredients!.Any(ri => ri.Ingredient!.Name!.Contains(filterCriteria))
                );
            }

            // Total count for search results
            int totalCount = await recipeQuery.CountAsync();
            ViewData["TotalMatches"] = totalCount;
            ViewData["HasSearch"] = !string.IsNullOrEmpty(filterCriteria);

            var recipeList = recipeQuery.Select(r => new RecipeOverviewViewModel()
            {
                ID              = r.ID,
                Name            = r.Name,
                CategoryName    = r.Category != null ? r.Category.Name : "Uncategorized",
                TotalTime       = FormatTotalTime(r.PreparationTime, r.CookTime),
                Servings        = r.Servings + " serving(s)",
                Status          = r.Status,
                CreatedDate     = r.CreatedDate,
                OwnerId         = r.OwnerId,
                OwnerName       = _context.Users.Where(u => u.Id == r.OwnerId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault(),
                Images          = r.Images!.Where(img => img.IsApproved == true).ToList()
            });

            var recipes = await PaginatedList<RecipeOverviewViewModel>.CreateAsync(recipeList, pageNumber ?? 1, pageSize);
            return View(recipes);
        }

        // GET: Browse/Recipes
        [HttpGet]
        public async Task<IActionResult> Recipes(string? keyword, int? categoryId, DateTime? startDate, DateTime? endDate, int pageNumber = 1)
        {
            const int pageSize = 6;

            var viewModel = new BrowseViewModel
            {
                Keyword = keyword,
                SelectedCategoryId = categoryId,
                StartDate = startDate,
                EndDate = endDate,
                CurrentPage = pageNumber
            };

            // Get categories for dropdown
            viewModel.Category = await _context.Categories
                //.Where(c => c.IsApproved == true) // ONLY APPROVE
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value   = c.ID.ToString(),
                    Text    = (c.IsApproved == false && c.IsPendingModification == true) ? $"{c.Name} (Pending Approval)" :
                              (c.IsApproved == true && c.IsPendingModification == false) ? $"{c.Name}" :
                              (c.IsApproved == true && c.IsPendingModification == true) ? $"{c.Name} (Pending Update)" :
                              $"{c.Name} (Rejected)"
                })
                .ToListAsync();

            // QUERY - ALL PUBLIC RECIPES
            var recipeQuery = _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Images)
                .Include(r => r.RecipeIngredients!)
                    .ThenInclude(ri => ri.Ingredient)
                .Where(r => r.Status == "Public")
                .OrderBy(r => r.Name)
                .AsQueryable();

            // SEARCH FILTER - ACROSS ALL CATEGORIES
            if (!string.IsNullOrEmpty(keyword))
            {
                recipeQuery = recipeQuery.Where(r =>
                    r.Name!.Contains(keyword) ||
                    r.OwnerId!.Contains(keyword) ||
                    r.RecipeIngredients!.Any(ri => ri.Ingredient!.Name!.Contains(keyword)) ||
                    r.Category!.Name!.Contains(keyword)
                );
            }

            // CATEGORY FILTER
            if (categoryId.HasValue)
            {
                recipeQuery = recipeQuery.Where(r => r.CategoryId == categoryId);
            }

            // DATE RANGE FILTER
            if (startDate.HasValue && endDate.HasValue)
            {
                recipeQuery = 

                    recipeQuery.Where(r => r.CreatedDate.HasValue &&
                                           r.CreatedDate.Value.Date >= startDate.Value.Date &&
                                           r.CreatedDate.Value.Date <= endDate.Value.Date);
            }
            else if (startDate.HasValue)
            {
                recipeQuery = recipeQuery.Where(r => r.CreatedDate.HasValue && r.CreatedDate.Value.Date >= startDate.Value.Date);
            }
            else if (endDate.HasValue)
            {
                recipeQuery = recipeQuery.Where(r => r.CreatedDate.HasValue && r.CreatedDate.Value.Date <= endDate.Value.Date);
            }

            var recipeList = recipeQuery.Select(r => new RecipeCardViewModel
            {
                ID              = r.ID,
                Name            = r.Name,
                CategoryName    = r.Category != null ? r.Category.Name : "Uncategorized",
                TotalTime       = FormatTotalTime(r.PreparationTime, r.CookTime),
                Servings        = r.Servings,
                OwnerId         = r.OwnerId,
                OwnerName       = _context.Users.Where(u => u.Id == r.OwnerId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault(),
                CreatedDate     = r.CreatedDate,
                Images          = r.Images!.Where(img => img.IsApproved == true).ToList()
            });

            // Total count for search results
            int totalCount = await recipeList.CountAsync();
            ViewData["TotalMatches"] = totalCount;
            ViewData["HasSearch"] = !string.IsNullOrEmpty(keyword) || categoryId.HasValue || startDate.HasValue || endDate.HasValue;

            // PAGINATION
            var searchResults = await PaginatedList<RecipeCardViewModel>.CreateAsync(recipeList, pageNumber, pageSize);

            viewModel.SearchResults = searchResults;
            viewModel.TotalItemCount = searchResults.TotalItemCount;
            viewModel.TotalPages = searchResults.TotalPages;

            return View(viewModel);
        }

        // GET: Browse/Recipe/5 (Public recipe details)
        [HttpGet]
        public async Task<IActionResult> Recipe(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Images)
                .Include(r => r.RecipeIngredients!)
                    .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (recipe == null || recipe.Status != "Public")
            {
                return View("NotFound");
            }

            var recipeVM = new RecipeDetailsViewModel
            {
                ID                      = recipe.ID,
                Name                    = recipe.Name,
                CategoryId              = recipe.CategoryId,
                CategoryName            = recipe.Category != null ? recipe.Category.Name : null,
                Instructions            = recipe.Instructions,
                Tips                    = recipe.Tips,
                PreparationTime         = FormatTimeDisplay(recipe.PreparationTime),
                CookTime                = FormatTimeDisplay(recipe.CookTime),
                TotalTime               = FormatTotalTime(recipe.PreparationTime, recipe.CookTime),
                TemperatureFahrenheit   = recipe.TemperatureFahrenheit != null ? recipe.TemperatureFahrenheit + " °F" : "N/A",
                Servings                = recipe.Servings + " people",
                Status                  = recipe.Status,
                OwnerId                 = recipe.OwnerId,
                OwnerName               = _context.Users.Where(u => u.Id == recipe.OwnerId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault(),
                CreatedDate             = recipe.CreatedDate?.ToString("MMMM d, yyyy"),
                LastModifiedDate        = recipe.LastModifiedDate?.ToString("MMMM d, yyyy"),
                NutritionInfo           = recipe.NutritionInfo,
                SpecialEquipment        = recipe.SpecialEquipment,
                YouTubeVideoLink        = recipe.YouTubeVideoLink,
                Images                  = recipe.Images?.Where(img => img.IsApproved == true).ToList(),
                Ingredients             = recipe.RecipeIngredients?
                                            .Select(ri => new RecipeIngredientDetailViewModel
                                            {
                                                IngredientId    = ri.IngredientId,
                                                RecipeId        = ri.RecipeId,
                                                IngredientName  = ri.Ingredient?.Name,
                                                IngredientType  = ri.Ingredient?.Type,
                                                Quantity        = FormatAmountDisplay(ri.Quantity),
                                                Unit            = ri.Unit,
                                                Notes           = ri.Notes
                                            })
                                            .ToList()
            };

            return View(recipeVM);
        }

        // POST: Browse/Share
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Share(ShareRecipeViewModel shareVM)
        {
            if (ModelState.IsValid)
            {
                var existingRecipe = await _context.Recipes.FindAsync(shareVM.RecipeId);
                if (existingRecipe == null || existingRecipe.Status != "Public")
                {
                    return NotFound();
                }

                // Ensure message is not null
                var emailContent = shareVM.Message ?? $"Check out this recipe: {shareVM.RecipeName}";
                var emailSubject = shareVM.Subject ?? $"Recipe: {shareVM.RecipeName}";

                var shareMessage = new EmailMessage(
                    new string[] { shareVM.RecipientEmail ?? "" },
                    emailSubject,
                    emailContent
                );

                _emailSender.SendEmail(shareMessage);

                TempData["Success"] = "Recipe shared successfully!";
                return RedirectToAction("Recipe", new { id = shareVM.RecipeId });
            }

            return await ReloadRecipeWithFormData(shareVM.RecipeId, shareVM, null);
        }

        // POST: Browse/Report
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Report(ReportRecipeViewModel reportVM)
        {
            if (ModelState.IsValid)
            {
                var existingRecipe = await _context.Recipes.FindAsync(reportVM.RecipeId);
                if (existingRecipe == null || existingRecipe.Status != "Public")
                {
                    return NotFound();
                }

                var contactReport = new Contact
                {
                    Name            = reportVM.ReporterName ?? "Anonymous",
                    Email           = reportVM.ReporterEmail ?? "no-email@example.com",
                    Topic           = ContactTopic.Report,
                    Subject         = $"Recipe Report: {existingRecipe.Name} (ID: {reportVM.RecipeId})",
                    Message         = $"Recipe URL: {reportVM.RecipeUrl ?? "N/A"}\n\nReason for Report:\n{reportVM.Reason ?? "No reason provided"}",
                    SubmittedAt     = DateTime.Now,
                    IsRead          = false
                };

                _context.Contacts.Add(contactReport);
                await _context.SaveChangesAsync();

                // Send email to admin
                var adminEmail = "nbcc-softwaredev@nbcc.ca";
                var reportSubject = $"Recipe Report: {existingRecipe.Name} (ID: {reportVM.RecipeId})";
                var reportMessage = $@"
RECIPE REPORT DETAILS:
=====================
Recipe: {existingRecipe.Name}
Recipe ID: {reportVM.RecipeId}
Recipe URL: {reportVM.RecipeUrl ?? "N/A"}
Reported By: {reportVM.ReporterName ?? "Anonymous"} ({reportVM.ReporterEmail ?? "no-email"})
Contact Record ID: {contactReport.Id}
Report Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}

REASON FOR REPORT:
=================
{reportVM.Reason ?? "No reason provided"}

Please review this recipe and take appropriate action.
";

                var reportEmail = new EmailMessage(
                    new string[] { adminEmail },
                    reportSubject,
                    reportMessage
                );

                _emailSender.SendEmail(reportEmail);

                TempData["Success"] = "Report submitted successfully. An administrator will review it.";
                return RedirectToAction("Recipe", new { id = reportVM.RecipeId });
            }

            return await ReloadRecipeWithFormData(reportVM.RecipeId, null, reportVM);
        }

        // GET: Browse/Print/5
        [HttpGet]
        public async Task<IActionResult> Print(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Images)
                .Include(r => r.RecipeIngredients!)
                    .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (recipe == null || recipe.Status != "Public")
            {
                return View("RecipeNotFound");
            }

            var recipeVM = new RecipeDetailsViewModel
            {
                ID                      = recipe.ID,
                Name                    = recipe.Name,
                CategoryName            = recipe.Category != null ? recipe.Category.Name : null,
                Instructions            = recipe.Instructions,
                Tips                    = recipe.Tips,
                PreparationTime         = FormatTimeDisplay(recipe.PreparationTime),
                CookTime                = FormatTimeDisplay(recipe.CookTime),
                TotalTime               = FormatTotalTime(recipe.PreparationTime, recipe.CookTime),
                TemperatureFahrenheit   = recipe.TemperatureFahrenheit != null ? recipe.TemperatureFahrenheit + " °F" : "N/A",
                Servings                = recipe.Servings + " people",
                OwnerName               = _context.Users.Where(u => u.Id == recipe.OwnerId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault(),
                CreatedDate             = recipe.CreatedDate?.ToString("MMMM d, yyyy"),
                NutritionInfo           = recipe.NutritionInfo,
                SpecialEquipment        = recipe.SpecialEquipment,
                Images                  = recipe.Images?.Where(img => img.IsApproved == true && img.IsMainImage == true).ToList(),
                Ingredients             = recipe.RecipeIngredients?
                                            .Select(ri => new RecipeIngredientDetailViewModel
                                            {
                                                IngredientName  = ri.Ingredient?.Name,
                                                Quantity        = FormatAmountDisplay(ri.Quantity),
                                                Unit            = ri.Unit,
                                                Notes           = ri.Notes
                                            })
                                            .ToList()
            };

            return View(recipeVM);
        }

        private async Task<IActionResult> ReloadRecipeWithFormData(int? recipeId, ShareRecipeViewModel? shareVM, ReportRecipeViewModel? reportVM)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Images)
                .Include(r => r.RecipeIngredients!)
                    .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(m => m.ID == recipeId);

            if (recipe == null || recipe.Status != "Public")
            {
                return NotFound();
            }

            var recipeVM = new RecipeDetailsViewModel
            {
                ID                      = recipe.ID,
                Name                    = recipe.Name,
                CategoryId              = recipe.CategoryId,
                CategoryName            = recipe.Category?.Name,
                Instructions            = recipe.Instructions,
                Tips                    = recipe.Tips,
                PreparationTime         = FormatTimeDisplay(recipe.PreparationTime),
                CookTime                = FormatTimeDisplay(recipe.CookTime),
                TotalTime               = FormatTotalTime(recipe.PreparationTime, recipe.CookTime),
                TemperatureFahrenheit   = recipe.TemperatureFahrenheit != null ? recipe.TemperatureFahrenheit + " °F" : "N/A",
                Servings                = recipe.Servings + " people",
                Status                  = recipe.Status,
                OwnerId                 = recipe.OwnerId,
                OwnerName               = _context.Users.Where(u => u.Id == recipe.OwnerId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault(),
                CreatedDate             = recipe.CreatedDate?.ToString("MMMM d, yyyy"),
                LastModifiedDate        = recipe.LastModifiedDate?.ToString("MMMM d, yyyy"),
                NutritionInfo           = recipe.NutritionInfo,
                SpecialEquipment        = recipe.SpecialEquipment,
                YouTubeVideoLink        = recipe.YouTubeVideoLink,
                Images                  = recipe.Images?.Where(img => img.IsApproved == true).ToList(),
                Ingredients             = recipe.RecipeIngredients?
                                            .Select(ri => new RecipeIngredientDetailViewModel
                                            {
                                                IngredientId    = ri.IngredientId,
                                                RecipeId        = ri.RecipeId,
                                                IngredientName  = ri.Ingredient?.Name,
                                                IngredientType  = ri.Ingredient?.Type,
                                                Quantity        = FormatAmountDisplay(ri.Quantity),
                                                Unit            = ri.Unit,
                                                Notes           = ri.Notes
                                            })
                                            .ToList()
            };

            ViewBag.ShareFormData = shareVM ?? new ShareRecipeViewModel();
            ViewBag.ReportFormData = reportVM ?? new ReportRecipeViewModel();

            return View("Recipe", recipeVM);
        }

        private static string FormatTimeDisplay(int? minutes)
        {
            if (minutes == null || minutes == 0)
            {
                return "N/A";
            }

            if (minutes < 60)
            {
                return $"{minutes} min(s)";
            }
            else
            {
                var hours = minutes.Value / 60;
                var mins = minutes.Value % 60;

                if (mins == 0)
                {
                    return $"{hours} hr(s)";
                }
                else
                {
                    return $"{hours} hr(s) {mins} min(s)";
                }
            }
        }

        private static string FormatTotalTime(int? prepTime, int? cookTime)
        {
            var totalMinutes = (prepTime ?? 0) + (cookTime ?? 0);

            if (totalMinutes == 0)
            {
                return "N/A";
            }

            if (totalMinutes < 60)
            {
                return $"{totalMinutes} min(s)";
            }
            else
            {
                var hours = totalMinutes / 60;
                var minutes = totalMinutes % 60;

                if (minutes == 0)
                {
                    return $"{hours} hr(s)";
                }
                else
                {
                    return $"{hours} hr(s) {minutes} min(s)";
                }
            }
        }

        private static string FormatAmountDisplay(decimal? amount)
        {
            if (amount == null || amount == 0)
                return "0";

            decimal value = amount.Value;

            // Handle common fractions
            if (value == 0.25m)
                return "¼";

            if (value == 0.33m)
                return "⅓";

            if (value == 0.50m)
                return "½";

            if (value == 0.66m)
                return "⅔";

            if (value == 0.75m)
                return "¾";

            // Handle mixed numbers (like 1.5 → 1½)
            if (value > 1 && value != Math.Floor(value))
            {
                var wholePart = (int)Math.Floor(value);
                var fractionalPart = value - wholePart;

                if (fractionalPart == 0.25m)
                    return $"{wholePart} ¼";

                if (fractionalPart == 0.33m)
                    return $"{wholePart} ⅓";

                if (fractionalPart == 0.50m)
                    return $"{wholePart} ½";

                if (fractionalPart == 0.66m)
                    return $"{wholePart} ⅔";

                if (fractionalPart == 0.75m)
                    return $"{wholePart} ¾";
            }

            // For whole numbers
            if (value == Math.Floor(value))
                return ((int)value).ToString();

            // For other decimals
            return value.ToString("0.##");
        }
    }
}
