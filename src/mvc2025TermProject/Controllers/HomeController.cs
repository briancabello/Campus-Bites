using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc2025TermProject.Data;
using mvc2025TermProject.Models;
using System.Diagnostics;

namespace mvc2025TermProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task <IActionResult> Index()
        {
            var browseVM = new BrowseViewModel();

            // SECTION 1: Tasty on Budget
            browseVM.TastyOnBudget = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Images)
                .Include(r => r.RecipeIngredients!)
                    .ThenInclude(ri => ri.Ingredient)
                .Where(r => r.Status == "Public")
                .Where(r => r.PreparationTime + r.CookTime <= 30) // Quick recipes
                .Where(r => r.Category == null || !r.Category.Name.ToLower().Contains("dessert"))
                .OrderBy(r => r.PreparationTime + r.CookTime)
                .Take(3)
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

            // SECTION 2: Sweet & Savory Savings
            browseVM.SweetSavorySavings = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Images)
                .Where(r => r.Status == "Public")
                .Where(r => r.Category != null && (r.Category.Name.Contains("Dessert") || r.Category.Name.Contains("Main")))
                .OrderByDescending(r => r.CreatedDate)
                .Take(3)
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

            // SECTION 3: Popular Categories (Most recipes)
            browseVM.PopularCategories = await _context.Categories
                .Where(c => c.IsApproved == true)
                .Select(c => new CategoryCardViewModel
                {
                    ID          = c.ID,
                    Name        = c.Name,
                    Description = c.Description,
                    RecipeCount = _context.Recipes.Count(r => r.CategoryId == c.ID && r.Status == "Public")
                })
                .Where(c => c.RecipeCount > 0)
                .OrderByDescending(c => c.RecipeCount)
                .Take(6)
                .ToListAsync();

            // SECTION 4: Chef's Pick (Recently added) exluding Desset
            browseVM.ChefsPick = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Images)
                .Where(r => r.Status == "Public")
                .Where(r => r.CreatedDate >= DateTime.Now.AddDays(-30))
                .Where(r => r.Category == null || !r.Category.Name.ToLower().Contains("dessert"))
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

            // SECTION 5: Latest Recipes
            browseVM.LatestRecipes = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Images)
                .Where(r => r.Status == "Public")
                .OrderByDescending(r => r.CreatedDate)
                .Take(16)
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





            return View(browseVM);
        }

        public async Task<IActionResult> About()
        {
            var browseVM = new BrowseViewModel();

            browseVM.OurPicks = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Images)
                .Where(r => r.Status == "Public")
                .OrderByDescending(r => r.CreatedDate) // Most recent first
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

            return View(browseVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
