using AutoMapper;
using ShopApp.Application.Dtos;
using ShopApp.Domain.Entities;

namespace ShopApp.Application.Profiles;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Category, CategoryFormDto>().ReverseMap();
    }
}