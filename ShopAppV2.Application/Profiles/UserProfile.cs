using AutoMapper;
using ShopApp.Application.Dtos;
using ShopApp.Domain.Entities;

namespace ShopApp.Application.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, UserRegisterDto>().ReverseMap();
        CreateMap<User, UserLoginDto>().ReverseMap();
    }
}