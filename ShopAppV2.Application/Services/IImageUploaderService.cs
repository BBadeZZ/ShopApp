using Microsoft.AspNetCore.Http;

namespace ShopApp.Application.Services;

public interface IImageUploaderService
{
    Task<string> UploadImage(IFormFile image);
}