using mvc2025TermProject.Data;
using mvc2025TermProject.Models;

namespace mvc2025TermProject.Helpers
{
    public static class ImageFileHelper
    {
        public static async Task MoveApprovedImagesAsync(ApplicationDbContext context, IWebHostEnvironment environment, Recipe recipe)
        {
            if (recipe.Images == null || !recipe.Images.Any())
            {
                return;
            }
                

            bool updated = false;
            var webRootPath = environment.WebRootPath;

            foreach (var image in recipe.Images
                .Where(i => i.IsApproved == true &&
                            i.FilePath != null &&
                            i.FilePath.StartsWith("/img/temp/", StringComparison.OrdinalIgnoreCase)))
            {
                string currentPhysicalPath = Path.Combine(
                    webRootPath,
                    image.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
                );

                string approvedFolder = Path.Combine(webRootPath, "img", "approved");
                if (!Directory.Exists(approvedFolder))
                {
                    Directory.CreateDirectory(approvedFolder);
                }

                string fileName = Path.GetFileName(currentPhysicalPath);
                string newPhysicalPath = Path.Combine(approvedFolder, fileName);

                if (File.Exists(currentPhysicalPath))
                {
                    File.Move(currentPhysicalPath, newPhysicalPath, overwrite: true);

                    image.FilePath = "/img/approved/" + fileName;
                    updated = true;
                }
            }

            if (updated)
            {
                await context.SaveChangesAsync();
            }
        }
    }
}
