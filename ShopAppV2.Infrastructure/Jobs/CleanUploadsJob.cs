using Microsoft.AspNetCore.Hosting;
using Quartz;
using ShopApp.Domain.Entities;
using ShopApp.Infrastructure.Repositories;

namespace ShopApp.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class CleanUploadsJob : IJob
{
    private readonly IWebHostEnvironment _environment;
    private readonly IProductRepository _productRepository;

    public CleanUploadsJob(IWebHostEnvironment environment, IProductRepository productRepository)
    {
        _environment = environment;
        _productRepository = productRepository;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");

        if (!Directory.Exists(uploadsPath))
        {
            Console.WriteLine($"Directory not found: {uploadsPath}");
            return;
        }

        var allFiles = Directory.GetFiles(uploadsPath);
        var usedFiles =
            (await _productRepository.FindAsync(p => p.Status == Status.Active && !string.IsNullOrEmpty(p.ImagePath)))
            .Select(p => Path.GetFileName(p.ImagePath));

        foreach (var file in allFiles)
        {
            var fileName = Path.GetFileName(file);

            if (!usedFiles.Contains(fileName))
                try
                {
                    File.Delete(file);
                    Console.WriteLine($"Deleted file: {fileName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting file {fileName}: {ex.Message}");
                }
        }
    }
}