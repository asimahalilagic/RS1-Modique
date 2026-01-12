using AutoMapper;
using Modique.Application.DTOs.Auth;
using Modique.Application.DTOs.Products;
using Modique.Domain.Entities;

namespace Modique.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Auth mappings
        CreateMap<User, UserDto>();

        // Product mappings
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
            .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : null));

        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();
    }
}




