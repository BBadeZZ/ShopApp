using System.Text.Json.Serialization;
using ImageUploader.Api.Models;
using ImageUploader.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImageUploader.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImageController : ControllerBase
{
    private readonly IImageUploaderService _imageUploaderService;

    public ImageController(IImageUploaderService imageUploaderService)
    {
        _imageUploaderService = imageUploaderService;
    }

    [HttpPost("Upload")]
    public async Task<Response<UploadImageResponse>> UploadImage([FromForm] IFormFile image)
    {
        if (image == null || image.Length == 0) return Response<UploadImageResponse>.Fail("Image is empty!");

        try
        {
            var imagePath = await _imageUploaderService.UploadImage(image);
            return Response<UploadImageResponse>.Success("Ok!", new UploadImageResponse { ImagePath = imagePath });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error uploading image: {ex.Message}");
            return Response<UploadImageResponse>.Fail("An error occured!");
        }
    }

    public class UploadImageResponse
    {
        [JsonPropertyName("imagePath")] public string ImagePath { get; set; } = string.Empty;
    }
}