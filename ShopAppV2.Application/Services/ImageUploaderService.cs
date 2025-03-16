using Microsoft.AspNetCore.Http;

namespace ShopApp.Application.Services;

public class ImageUploaderService : IImageUploaderService
{
    public async Task<string> UploadImage(IFormFile image)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(uploadsFolder);

        // Generate a unique file name
        var uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        // Save the file to the server
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(fileStream);
        }

        return $"{Directory.GetCurrentDirectory()}/wwwroot/uploads/{uniqueFileName}";
    }
}