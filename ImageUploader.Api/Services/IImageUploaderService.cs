namespace ImageUploader.Api.Services;

public interface IImageUploaderService
{
    Task<string> UploadImage(IFormFile image);
}