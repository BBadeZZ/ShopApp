using Microsoft.AspNetCore.Mvc;
using ShopApp.Application.Dtos;
using ShopApp.Application.Services;
using ShopApp.Domain.Models;

namespace ShopAppV2.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    // POST: api/register
    [HttpPost("Register")]
    public async Task<Response<UserDto>> Register(UserRegisterDto userRegisterDto)
    {
        var response = await _userService.RegisterUserAsync(userRegisterDto);
        return response;
    }

    // POST: api/login
    [HttpPost("Login")]
    public async Task<Response<string>> Login(UserLoginDto userLoginDto)
    {
        var response = await _userService.LoginAsync(userLoginDto);
        return response;
    }
}