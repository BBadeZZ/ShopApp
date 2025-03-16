using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using ShopApp.Application.Services;
using ShopApp.Domain.Models;

namespace ShopApp.Infrastructure.Services;

public class RemoteImageUploaderService : IImageUploaderService
{
    private readonly HttpClient _httpClient;

    public RemoteImageUploaderService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> UploadImage(IFormFile image)
    {
        using var content = new MultipartFormDataContent();
        using var stream = image.OpenReadStream();
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);

        content.Add(fileContent, "image", image.FileName);

        var response = await _httpClient.PostAsync("/api/image/upload", content);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<Response<UploadImageResponse>>();
        return result?.Data?.ImagePath ?? throw new Exception("Failed to upload image.");
    }

    private class UploadImageResponse
    {
        [JsonPropertyName("imagePath")] public string ImagePath { get; } = string.Empty;
    }
}