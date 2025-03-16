using ShopApp.Application.Dtos;
using ShopApp.Domain.Models;

namespace ShopApp.Application.Services;

public interface IUserService
{
    Task<Response<UserDto>> RegisterUserAsync(UserRegisterDto userRegisterDto);
    Task<Response<string>> LoginAsync(UserLoginDto userLogin);
}