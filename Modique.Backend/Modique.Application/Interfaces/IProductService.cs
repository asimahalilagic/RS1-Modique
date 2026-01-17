using Modique.Application.DTOs.Common;
using Modique.Application.DTOs.Products;

namespace Modique.Application.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetAllAsync(int page = 1, int pageSize = 20);
    Task<PagedResult<ProductDto>> GetAllForAdminAsync(int page = 1, int pageSize = 20);
    Task<ProductDto?> GetByIdAsync(int id);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto);
    Task<bool> DeleteAsync(int id);
}

