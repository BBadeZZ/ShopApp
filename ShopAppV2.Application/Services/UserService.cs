using AutoMapper;
using ShopApp.Application.Dtos;
using ShopApp.Domain.Entities;
using ShopApp.Domain.Models;
using ShopApp.Infrastructure.Repositories;
using ShopApp.Infrastructure.Security;

namespace ShopApp.Application.Services;

public class UserService : IUserService
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<User> _userRepository;

    public UserService(IGenericRepository<User> userRepository, IJwtTokenService jwtTokenService, IMapper mapper)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
    }

    public async Task<Response<UserDto>> RegisterUserAsync(UserRegisterDto userRegisterDto)
    {
        // Check if user exists
        var existingUser = await _userRepository.FirstOrDefaultAsync(u => u.Username == userRegisterDto.Username);
        if (existingUser != null) return Response<UserDto>.Fail("Username is already taken.");

        // Hash password
        var hashedPassword = PasswordHasher.HashPassword(userRegisterDto.Password);

        // Create user
        var user = new User
        {
            Username = userRegisterDto.Username,
            PasswordHash = hashedPassword,
            Role = userRegisterDto.Role,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        return Response<UserDto>.Success("User registered successfully.",
            _mapper.Map<UserDto>(await _userRepository.FirstOrDefaultAsync(u => u.Username == user.Username)));
    }

    public async Task<Response<string>> LoginAsync(UserLoginDto userLoginDto)
    {
        var user = await _userRepository.FirstOrDefaultAsync(u => u.Username == userLoginDto.Username);
        if (user == null || !PasswordHasher.VerifyPassword(userLoginDto.Password, user.PasswordHash))
            return Response<string>.Fail("Invalid username or password.");

        // Generate JWT token
        var token = _jwtTokenService.GenerateToken(user.Id, user.Username, user.Role);
        return Response<string>.Success("Ok", token);
    }
}