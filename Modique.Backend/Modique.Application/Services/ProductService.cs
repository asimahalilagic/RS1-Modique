using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modique.Application.DTOs.Common;
using Modique.Application.DTOs.Products;
using Modique.Application.Interfaces;
using Modique.Domain.Entities;
using Modique.Infrastructure.Data;

namespace Modique.Application.Services;

public class ProductService : IProductService
{
    private readonly ModiqueDbContext _db;
    private readonly ILogger<ProductService> _logger;

    public ProductService(ModiqueDbContext db, ILogger<ProductService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PagedResult<ProductDto>> GetAllAsync(int page = 1, int pageSize = 20)
    {
        var query = _db.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Images)
            .Where(p => p.IsActive)
            .AsQueryable();

        var totalCount = await query.CountAsync();

        var products = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var productDtos = products.Select(p => new ProductDto
        {
            ProductId = p.ProductId,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            CreatedAt = p.CreatedAt,
            IsActive = p.IsActive,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name,
            BrandId = p.BrandId,
            BrandName = p.Brand?.Name,
            Images = p.Images.OrderBy(i => i.Order).Select(i => new DTOs.Products.ProductImageDto
            {
                ProductImageId = i.ProductImageId,
                ImageUrl = i.ImageUrl,
                Order = i.Order,
                IsMain = i.IsMain
            }).ToList()
        }).ToList();

        return new PagedResult<ProductDto>
        {
            Items = productDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<ProductDto>> GetAllForAdminAsync(int page = 1, int pageSize = 20)
    {
        var query = _db.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Images)
            .AsQueryable();

        var totalCount = await query.CountAsync();

        var products = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var productDtos = products.Select(p => new ProductDto
        {
            ProductId = p.ProductId,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            CreatedAt = p.CreatedAt,
            IsActive = p.IsActive,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name,
            BrandId = p.BrandId,
            BrandName = p.Brand?.Name,
            Images = p.Images.OrderBy(i => i.Order).Select(i => new DTOs.Products.ProductImageDto
            {
                ProductImageId = i.ProductImageId,
                ImageUrl = i.ImageUrl,
                Order = i.Order,
                IsMain = i.IsMain
            }).ToList()
        }).ToList();

        return new PagedResult<ProductDto>
        {
            Items = productDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _db.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null)
            return null;

        return new ProductDto
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CreatedAt = product.CreatedAt,
            IsActive = product.IsActive,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name,
            BrandId = product.BrandId,
            BrandName = product.Brand?.Name,
            Images = product.Images.OrderBy(i => i.Order).Select(i => new DTOs.Products.ProductImageDto
            {
                ProductImageId = i.ProductImageId,
                ImageUrl = i.ImageUrl,
                Order = i.Order,
                IsMain = i.IsMain
            }).ToList()
        };
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            CategoryId = dto.CategoryId,
            BrandId = dto.BrandId,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        if (dto.ImageUrls != null && dto.ImageUrls.Any())
        {
            for (int i = 0; i < dto.ImageUrls.Count; i++)
            {
                var image = new Domain.Entities.ProductImage
                {
                    ProductId = product.ProductId,
                    ImageUrl = dto.ImageUrls[i],
                    Order = i,
                    IsMain = i == 0
                };
                _db.ProductImages.Add(image);
            }
            await _db.SaveChangesAsync();
        }

        _logger.LogInformation("Product created: {ProductId} - {ProductName}", product.ProductId, product.Name);

        return await GetByIdAsync(product.ProductId) ?? throw new InvalidOperationException("Failed to retrieve created product");
    }

    public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _db.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.ProductId == id);
        if (product == null)
            return null;

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.CategoryId = dto.CategoryId;
        product.BrandId = dto.BrandId;
        product.IsActive = dto.IsActive;

        if (dto.ImageUrls != null && dto.ImageUrls.Any())
        {
            var existingImages = product.Images.ToList();
            _db.ProductImages.RemoveRange(existingImages);

            for (int i = 0; i < dto.ImageUrls.Count; i++)
            {
                var image = new Domain.Entities.ProductImage
                {
                    ProductId = product.ProductId,
                    ImageUrl = dto.ImageUrls[i],
                    Order = i,
                    IsMain = i == 0
                };
                _db.ProductImages.Add(image);
            }
        }

        await _db.SaveChangesAsync();

        _logger.LogInformation("Product updated: {ProductId}", product.ProductId);

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
            return false;

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Product deleted: {ProductId}", id);

        return true;
    }
}




