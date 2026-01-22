using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using mvc2025TermProject.Data;
using mvc2025TermProject.Models;
using mvc2025TermProject.Helpers;

namespace mvc2025TermProject.Controllers.User
{
    [Authorize(Roles = "User, Administrator")]
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public RecipesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Recipes (Browse All Recipes - Public)
        [HttpGet]
        public async Task<IActionResult> Index(string? sortOrder, string? filterCriteria, string? searchString, int? pageNumber)
        {
            const int pageSize = 10;

            var recipeList = _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Owner)
                .Where(r => r.Status == "Public")
                .Select(r => new RecipeOverviewViewModel()
                {
                    ID              = r.ID,
                    Name            = r.Name,
                    CategoryName    = r.Category != null ? r.Category.Name : "Uncategorized",
                    TotalTime       = FormatTotalTime(r.PreparationTime, r.CookTime),
                    Servings        = r.Servings + " people",
                    Status          = r.Status,
                    CreatedDate     = r.CreatedDate,
                    OwnerId         = r.OwnerId,
                    OwnerName       = r.Owner != null ? $"{r.Owner.FirstName} {r.Owner.LastName}" : "Unknown User"
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
                recipeList = recipeList.Where(i =>
                    i.Name!.Contains(filterCriteria) ||
                    i.CategoryName!.Contains(filterCriteria) ||
                    i.Status!.Contains(filterCriteria) ||
                    i.OwnerId!.Contains(filterCriteria)
                );
            }

            // Total count for search results
            int totalCount = await recipeList.CountAsync();
            ViewData["TotalMatches"] = totalCount;
            ViewData["HasSearch"] = !string.IsNullOrEmpty(filterCriteria);

            switch (sortOrder)
            {
                case "date":
                    recipeList = recipeList.OrderBy(i => i.CreatedDate);
                    break;

                case "date_desc":
                    recipeList = recipeList.OrderByDescending(i => i.CreatedDate);
                    break;

                case "name_desc":
                    recipeList = recipeList.OrderByDescending(i => i.Name);
                    break;

                case "status":
                    recipeList = recipeList.OrderBy(i => i.Status);
                    break;

                case "status_desc":
                    recipeList = recipeList.OrderByDescending(i => i.Status);
                    break;

                default:
                    recipeList = recipeList.OrderBy(i => i.Name);
                    break;
            }

            if (pageNumber == null)
                pageNumber = 1;

            return View(await PaginatedList<RecipeOverviewViewModel>.CreateAsync(recipeList, (int)pageNumber, pageSize));
        }

        // GET: Recipes/MyRecipes (User's own recipes)
        [HttpGet]
        public async Task<IActionResult> MyRecipes(string? sortOrder, string? filterCriteria, string? searchString, int? pageNumber)
        {
            const int pageSize = 6;

            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var recipeList = _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Owner)
                .Where(r => r.OwnerId == userId)
                .Select(r => new RecipeOverviewViewModel()
                {
                    ID              = r.ID,
                    Name            = r.Name,
                    CategoryName    = r.Category != null ? r.Category.Name : "Uncategorized",
                    TotalTime       = FormatTotalTime(r.PreparationTime, r.CookTime),
                    Servings        = r.Servings + " people",
                    Status          = r.Status,
                    CreatedDate     = r.CreatedDate,
                    OwnerName       = r.Owner != null ? $"{r.Owner.FirstName} {r.Owner.LastName}" : "Unknown User",
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
                recipeList = recipeList.Where(i =>
                    i.Name!.Contains(filterCriteria) ||
                    i.CategoryName!.Contains(filterCriteria) ||
                    i.Status!.Contains(filterCriteria) ||
                    i.OwnerId!.Contains(filterCriteria)
                );
            }

            // Total count for search results
            int totalCount = await recipeList.CountAsync();
            ViewData["TotalMatches"] = totalCount;
            ViewData["HasSearch"] = !string.IsNullOrEmpty(filterCriteria);

            switch (sortOrder)
            {
                case "date":
                    recipeList = recipeList.OrderBy(i => i.CreatedDate);
                    break;

                case "date_desc":
                    recipeList = recipeList.OrderByDescending(i => i.CreatedDate);
                    break;

                case "name_desc":
                    recipeList = recipeList.OrderByDescending(i => i.Name);
                    break;

                case "status":
                    recipeList = recipeList.OrderBy(i => i.Status);
                    break;

                case "status_desc":
                    recipeList = recipeList.OrderByDescending(i => i.Status);
                    break;

                default:
                    recipeList = recipeList.OrderBy(i => i.Name);
                    break;
            }

            if (pageNumber == null)
                pageNumber = 1;

            return View(await PaginatedList<RecipeOverviewViewModel>.CreateAsync(recipeList, (int)pageNumber, pageSize));
        }

        // GET: Recipes/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                            .Include(r => r.Category)
                            .Include(r => r.Owner)
                            .Include(r => r.RecipeIngredients!)
                                .ThenInclude(ri => ri.Ingredient)
                            .Include(r => r.Images!)
                            .FirstOrDefaultAsync(m => m.ID == id);

            if (recipe == null)
            {
                return View("NotFound");
            }

            // AUTHORIZATION CHECK
            if (!CanViewRecipe(recipe, User))
            {
                return View("AccessDenied");
            }

            await ImageFileHelper.MoveApprovedImagesAsync(_context, _environment, recipe);

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
                Status                  = recipe.Status,
                OwnerId                 = recipe.OwnerId,
                OwnerName               = recipe.Owner != null ? $"{recipe.Owner.FirstName} {recipe.Owner.LastName}" : "Unknown User",
                CreatedDate             = recipe.CreatedDate?.ToString("MMMM d, yyyy"),
                LastModifiedDate        = recipe.LastModifiedDate?.ToString("MMMM d, yyyy"),
                NutritionInfo           = recipe.NutritionInfo,
                SpecialEquipment        = recipe.SpecialEquipment,
                YouTubeVideoLink        = recipe.YouTubeVideoLink,
                Images                  = recipe.Images?.ToList() ?? new List<Image>(),
                Ingredients             = recipe.RecipeIngredients?
                                            .OrderBy(ri => ri.SortOrder)
                                            .Select(ri => new RecipeIngredientDetailViewModel
                                                {
                                                    IngredientId    = ri.IngredientId,
                                                    RecipeId        = ri.RecipeId,
                                                    IngredientName  = ri.Ingredient?.Name,
                                                    IngredientType  = ri.Ingredient?.Type,
                                                    Quantity        = FormatAmountDisplay(ri.Quantity),
                                                    Unit            = ri.Unit,
                                                    Notes           = ri.Notes
                                                }
                                            )
                                            .ToList()
            };


            // Get IDs of ingredients already in this recipe
            var existingIngredientIds = recipe.RecipeIngredients?
                .Select(ri => ri.IngredientId)
                .ToList() ?? new List<int?>();

            // GET AVAILABLE INGREDIENTS
            var availableIngredients = _context.Ingredients
                //.Where(i => i.IsApproved == true && i.IsPendingModification == false && !existingIngredientIds.Contains(i.Id))
                .Where(i => !existingIngredientIds.Contains(i.Id))
                .OrderBy(i => i.Name)
                .Select(i => new
                {
                    i.Id,
                    DisplayText = (i.IsApproved == false && i.IsPendingModification == true) ? $"{i.Name} ({i.Type}) : [Pending Approval]" :
                                  (i.IsApproved == true && i.IsPendingModification == false) ? $"{i.Name} ({i.Type})" :
                                  (i.IsApproved == true && i.IsPendingModification == true) ? $"{i.Name} ({i.Type}) : [Pending Update]" :
                                  $"{i.Name} ({i.Type}) : [Rejected]"
                })
                .ToList();

            // PASS DATA TO THE VIEW (for Add Ingredient modal)
            ViewData["RecipeId"] = id;
            ViewData["AvailableIngredients"] = new SelectList(availableIngredients, "Id", "DisplayText");
            ViewData["CurrentUserId"] = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["CanPublish"] = recipe.Images?.Any(i => i.IsApproved == true && i.IsMainImage == true) == true;

            return View(recipeVM);
        }

        // POST: Recipes/AddIngredient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddIngredient(int? recipeId, RecipeIngredientViewModel ingredientVM)
        {
            if (ModelState.IsValid)
            {

                // Check for duplicate before adding
                bool ingredientExists = await _context.RecipeIngredients
                    .AnyAsync(ri => ri.RecipeId == recipeId && ri.IngredientId == ingredientVM.IngredientId);

                if (ingredientExists)
                {
                    ModelState.AddModelError("IngredientId", "This ingredient is already added to the recipe.");
                }
                else
                {
                    // Get existing ingredients to determine next SortOrder
                    var existingIngredients = await _context.RecipeIngredients.Where(ri => ri.RecipeId == recipeId).ToListAsync();

                    var recipeIngredient = new RecipeIngredient
                    {
                        RecipeId        = recipeId,
                        IngredientId    = ingredientVM.IngredientId,
                        Quantity        = ingredientVM.Quantity,
                        Unit            = ingredientVM.Unit,
                        Notes           = ingredientVM.Notes,
                        SortOrder       = existingIngredients.Count + 1
                    };

                    _context.RecipeIngredients.Add(recipeIngredient);

                    // Update Recipe status to 'Draft' if it was 'Initial'
                    var recipeEntity = await _context.Recipes.FindAsync(recipeId);

                    if (recipeEntity != null && recipeEntity.Status == "Initial")
                    {
                        recipeEntity.Status             = "Draft";
                        recipeEntity.LastModifiedDate   = DateTime.Today;
                    }

                    await _context.SaveChangesAsync();

                    return RedirectToAction("Details", new { id = recipeId });
                }
            }

            // If validation fails OR duplicate exists, return Details view with filtered ingredients
            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.RecipeIngredients!)
                    .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.Images!)
                .FirstOrDefaultAsync(m => m.ID == recipeId);

            if (recipe == null)
            {
                return NotFound();
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
                Status                  = recipe.Status,
                OwnerId                 = recipe.OwnerId,
                OwnerName               = recipe.Owner != null ? $"{recipe.Owner.FirstName} {recipe.Owner.LastName}" : "Unknown User",
                CreatedDate             = recipe.CreatedDate?.ToString("MMMM d, yyyy"),
                LastModifiedDate        = recipe.LastModifiedDate?.ToString("MMMM d, yyyy"),
                NutritionInfo           = recipe.NutritionInfo,
                SpecialEquipment        = recipe.SpecialEquipment,
                YouTubeVideoLink        = recipe.YouTubeVideoLink,
                Images                  = recipe.Images?.ToList() ?? new List<Image>(),
                Ingredients             = recipe.RecipeIngredients?
                                            .OrderBy(ri => ri.SortOrder)
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

            // Get IDs of ingredients already in this recipe
            var existingIngredientIds = recipe.RecipeIngredients?
                .Select(ri => ri.IngredientId)
                .ToList() ?? new List<int?>();

            // GET AVAILABLE INGREDIENTS
            var availableIngredients = _context.Ingredients
                //.Where(i => i.IsApproved == true && i.IsPendingModification == false && !existingIngredientIds.Contains(i.Id))
                .Where(i => !existingIngredientIds.Contains(i.Id))
                .OrderBy(i => i.Name)
                .Select(i => new
                {
                    i.Id,
                    DisplayText = (i.IsApproved == false && i.IsPendingModification == true) ? $"{i.Name} ({i.Type}) : [Pending Approval]" :
                                  (i.IsApproved == true && i.IsPendingModification == false) ? $"{i.Name} ({i.Type})" :
                                  (i.IsApproved == true && i.IsPendingModification == true) ? $"{i.Name} ({i.Type}) : [Pending Update]" :
                                  $"{i.Name} ({i.Type}) : [Rejected]"
                })
                .ToList();

            // PASS DATA TO THE VIEW (for Add Ingredient modal)
            ViewData["RecipeId"] = recipeId;
            ViewData["AvailableIngredients"] = new SelectList(availableIngredients, "Id", "DisplayText");

            return View("Details", recipeVM);
        }

        // POST: Recipes/RemoveIngredient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveIngredient(int? recipeId, int? ingredientId)
        {
            var recipeIngredient = await _context.RecipeIngredients
                .FirstOrDefaultAsync(ri => ri.RecipeId == recipeId && ri.IngredientId == ingredientId);

            if (recipeIngredient != null)
            {

                _context.RecipeIngredients.Remove(recipeIngredient);
                await _context.SaveChangesAsync();

                // If no ingredients remain, set Recipe status back to 'Initial'
                var remainingIngredients = await _context.RecipeIngredients
                    .CountAsync(ri => ri.RecipeId == recipeId);

                if (remainingIngredients == 0)
                {
                    var recipeEntity = await _context.Recipes.FindAsync(recipeId);

                    if (recipeEntity != null)
                    {
                        recipeEntity.Status             = "Initial";
                        recipeEntity.LastModifiedDate   = DateTime.Today;

                        await _context.SaveChangesAsync();
                    }
                }

            }

            return RedirectToAction("Details", new { id = recipeId });
        }

        // GET: Recipes/Create
        [HttpGet]
        public IActionResult Create()
        {
            // Only show approved categories that are not pending modification
            //var availableCategories = _context.Categories
            //        .Where(c => c.IsApproved == true && c.IsPendingModification == false)
            //        .OrderBy(c => c.Name)
            //        .ToList();

            var availableCategories = _context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    c.ID,
                    DisplayText = (c.IsApproved == false && c.IsPendingModification == true) ? $"{c.Name} (Pending Approval)" :
                                  (c.IsApproved == true && c.IsPendingModification == false) ? $"{c.Name}" :
                                  (c.IsApproved == true && c.IsPendingModification == true) ? $"{c.Name} (Pending Update)" :
                                  $"{c.Name} (Rejected)"
                })
                .ToList();

            ViewData["CategoryId"] = new SelectList(availableCategories, "ID", "DisplayText");
            //ViewData["CategoryId"] = new SelectList(availableCategories, "ID", "Name");
            //ViewBag.CategoryId = new SelectList(availableCategories, "ID", "Name");

            return View();
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,Name,Instructions,Tips,PreparationTime,CookTime,TemperatureFahrenheit,Servings,NutritionInfo,SpecialEquipment,YouTubeVideoLink")] RecipeCreateViewModel recipeVM)
        { 
            if (ModelState.IsValid)
            {

                if (!RecipeNameExists(recipeVM.Name))
                {
                    Recipe newRecipe = new Recipe
                    {
                        Name                    = recipeVM.Name,
                        CategoryId              = recipeVM.CategoryId,
                        Instructions            = recipeVM.Instructions,
                        Tips                    = recipeVM.Tips,
                        PreparationTime         = recipeVM.PreparationTime,
                        CookTime                = recipeVM.CookTime,
                        Servings                = recipeVM.Servings,
                        TemperatureFahrenheit   = recipeVM.TemperatureFahrenheit,
                        NutritionInfo           = recipeVM.NutritionInfo,
                        SpecialEquipment        = recipeVM.SpecialEquipment,
                        YouTubeVideoLink        = recipeVM.YouTubeVideoLink,
                        Status                  = "Initial",
                        OwnerId                 = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        CreatedDate             = DateTime.Today,
                        LastModifiedDate        = DateTime.Today
                    };

                    _context.Add(newRecipe);
                    await _context.SaveChangesAsync();
                    //return RedirectToAction(nameof(MyRecipes));
                    return RedirectToAction("Details", new { id = newRecipe.ID });
                }
                else
                {
                    ModelState.AddModelError("Name", $"The Recipe Name '{recipeVM.Name}' already exists.");
                }
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "ID", "Name", recipeVM.CategoryId);
            //ViewBag.CategoryId = new SelectList(_context.Categories, "ID", "Name", recipeVM.CategoryId);

            return View(recipeVM);
        }

        // GET: Recipes/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.ID == id);

            if (recipe == null)
            {
                return View("NotFound");
            }

            // AUTHORIZATION CHECK
            if (!CanEditRecipe(recipe, User))
            {
                return View("AccessDenied");
            }

            var recipeVM = new RecipeEditViewModel
            {
                ID                      = recipe.ID,
                Name                    = recipe.Name,
                CategoryId              = recipe.CategoryId,
                Instructions            = recipe.Instructions,
                Tips                    = recipe.Tips,
                PreparationTime         = recipe.PreparationTime,
                CookTime                = recipe.CookTime,
                TemperatureFahrenheit   = recipe.TemperatureFahrenheit,
                Servings                = recipe.Servings,
                NutritionInfo           = recipe.NutritionInfo,
                SpecialEquipment        = recipe.SpecialEquipment,
                YouTubeVideoLink        = recipe.YouTubeVideoLink,
                Status                  = recipe.Status,
                OwnerId                 = recipe.OwnerId,
                CreatedDate             = recipe.CreatedDate,
                LastModifiedDate        = recipe.LastModifiedDate
            };

            var availableCategories = _context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    c.ID,
                    DisplayText = (c.IsApproved == false && c.IsPendingModification == true) ? $"{c.Name} (Pending Approval)" :
                                  (c.IsApproved == true && c.IsPendingModification == false) ? $"{c.Name}" :
                                  (c.IsApproved == true && c.IsPendingModification == true) ? $"{c.Name} (Pending Update)" :
                                  $"{c.Name} (Rejected)"
                })
                .ToList();

            ViewData["CategoryId"] = new SelectList(availableCategories, "ID", "DisplayText", recipe.CategoryId);

            return View(recipeVM);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("ID,Name,CategoryId,Instructions,Tips,PreparationTime,CookTime,TemperatureFahrenheit,Servings,NutritionInfo,SpecialEquipment,YouTubeVideoLink")] RecipeEditViewModel recipeVM)
        {
            if (id != recipeVM.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!RecipeProposedExistsExcludingId(recipeVM.Name!, recipeVM.ID))
                    {
                        var existingRecipe = await _context.Recipes.FindAsync(id);

                        if (existingRecipe == null)
                        {
                            return View("NotFound");
                        }

                        existingRecipe.Name                     = recipeVM.Name;
                        existingRecipe.CategoryId               = recipeVM.CategoryId;
                        existingRecipe.Instructions             = recipeVM.Instructions;
                        existingRecipe.Tips                     = recipeVM.Tips;
                        existingRecipe.PreparationTime          = recipeVM.PreparationTime;
                        existingRecipe.CookTime                 = recipeVM.CookTime;
                        existingRecipe.TemperatureFahrenheit    = recipeVM.TemperatureFahrenheit;
                        existingRecipe.Servings                 = recipeVM.Servings;
                        existingRecipe.NutritionInfo            = recipeVM.NutritionInfo;
                        existingRecipe.SpecialEquipment         = recipeVM.SpecialEquipment;
                        existingRecipe.YouTubeVideoLink         = recipeVM.YouTubeVideoLink;
                        existingRecipe.LastModifiedDate         = DateTime.Today;

                        _context.Update(existingRecipe);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Details", new { id = recipeVM.ID });
                    }
                    else
                    {
                        ModelState.AddModelError("Name", $"Another recipe with name '{recipeVM.Name}' is already exists!");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(recipeVM.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // WHEN VALIDATION FAILS - REPOPULATE DISPLAY FIELDS
            var existingRecipeForDisplay = await _context.Recipes.FindAsync(id);

            if (existingRecipeForDisplay != null)
            {
                recipeVM.Status             = existingRecipeForDisplay.Status;
                recipeVM.OwnerId            = existingRecipeForDisplay.OwnerId;
                recipeVM.CreatedDate        = existingRecipeForDisplay.CreatedDate;
                recipeVM.LastModifiedDate   = existingRecipeForDisplay.LastModifiedDate;
            }

            var availableCategories = _context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    c.ID,
                    DisplayText = (c.IsApproved == false && c.IsPendingModification == true) ? $"{c.Name} (Pending Approval)" :
                                  (c.IsApproved == true && c.IsPendingModification == false) ? $"{c.Name}" :
                                  (c.IsApproved == true && c.IsPendingModification == true) ? $"{c.Name} (Pending Update)" :
                                  $"{c.Name} (Rejected)"
                })
                .ToList();

            ViewData["CategoryId"] = new SelectList(availableCategories, "ID", "DisplayText", recipeVM.CategoryId);

            return View(recipeVM);
        }

        // POST: Recipes/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var recipe = await _context.Recipes.FindAsync(id);

            if (recipe == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (recipe.OwnerId != currentUserId && !User.IsInRole("Administrator"))
            {
                return Forbid();
            }

            // Validate the new status
            var validStatuses = new[] { "Draft", "Public", "Private", "Unlisted" };
            if (!validStatuses.Contains(newStatus))
            {
                TempData["Error"] = "Invalid status selected.";
                return RedirectToAction("Details", new { id });
            }

            if (newStatus == "Public")
            {
                // Load the recipe 
                var recipeWithDetails = await _context.Recipes
                    .Include(r => r.Images)
                    .Include(r => r.Category)
                    .Include(r => r.RecipeIngredients!)
                        .ThenInclude(ri => ri.Ingredient)
                    .FirstOrDefaultAsync(r => r.ID == id);

                if (recipeWithDetails == null)
                {
                    return NotFound();
                }

                List<string> errors = new List<string>();

                // 1. Check for approved main image
                bool hasApprovedMainImage = recipeWithDetails.Images?.Any(i => i.IsApproved == true && i.IsMainImage == true) == true;
                if (!hasApprovedMainImage)
                {
                    errors.Add("Cannot publish recipe. You need at least one approved main image.");
                }

                // 2. Check category approval
                if (recipeWithDetails.Category?.IsApproved != true)
                {
                    errors.Add("The selected category is still pending approval.");
                }

                // 3. Check if recipe has any ingredients at all
                if (recipeWithDetails.RecipeIngredients == null || !recipeWithDetails.RecipeIngredients.Any())
                {
                    errors.Add("Cannot publish recipe. You need to add at least one ingredient.");
                }
                else
                {
                    // 4. Check ingredients approval
                    var pendingIngredients = recipeWithDetails.RecipeIngredients
                        .Where(ri => ri.Ingredient?.IsApproved != true)
                        .Select(ri => ri.Ingredient?.Name)
                        .Where(name => !string.IsNullOrEmpty(name))
                        .ToList();

                    if (pendingIngredients.Any())
                    {
                        errors.Add($"The following ingredients are pending approval: {string.Join(", ", pendingIngredients)}");
                    }
                }

                if (errors.Any())
                {
                    TempData["Error"] = string.Join("<br/>", errors);
                    return RedirectToAction("Details", new { id });
                }

                // Use the fully loaded recipe for the update
                recipe = recipeWithDetails;
            }

            // Update the status
            recipe.Status = newStatus;
            recipe.LastModifiedDate = DateTime.Today;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Recipe status updated to {newStatus} successfully!";
            return RedirectToAction("Details", new { id });
        }

        private bool RecipeExists(int? id)
        {
            return _context.Recipes.Any(e => e.ID == id);
        }

        private bool RecipeNameExists(string? name)
        {
            return _context.Recipes.Any(e => e.Name == name);
        }

        private bool RecipeProposedExistsExcludingId(string? name, int? excludeId)
        {
            return _context.Recipes.Any(i => i.ID != excludeId && i.Name == name);
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

        private bool CanViewRecipe(Recipe recipe, ClaimsPrincipal user)
        {
            // Admins can view everything
            if (user.IsInRole("Administrator"))
                return true;

            var currentUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            switch (recipe.Status)
            {
                case "Public":
                    return true;                                // Everyone can view public recipes
                case "Draft":
                case "Initial":
                    return currentUserId == recipe.OwnerId;     // Only owner
                case "Private":
                    return currentUserId == recipe.OwnerId;     // Only owner
                case "Unlisted":
                    return true;                                // Anyone with the link can view
                default:
                    return false;
            }
        }

        private bool CanEditRecipe(Recipe recipe, ClaimsPrincipal user)
        {
            // Admins can edit everything
            if (user.IsInRole("Administrator"))
                return true;

            // Only owner can edit their own recipes, regardless of status
            var currentUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return currentUserId == recipe.OwnerId;
        }
    }
}
