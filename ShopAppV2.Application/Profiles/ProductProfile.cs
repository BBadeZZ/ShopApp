using AutoMapper;
using ShopApp.Application.Dtos;
using ShopApp.Domain.Entities;

namespace ShopApp.Application.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<Product, ProductFormDto>().ReverseMap();
    }
}