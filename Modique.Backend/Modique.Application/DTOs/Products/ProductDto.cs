namespace Modique.Application.DTOs.Products;

public class ProductImageDto
{
    public int ProductImageId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsMain { get; set; }
}

public class ProductDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int BrandId { get; set; }
    public string? BrandName { get; set; }
    public List<ProductImageDto> Images { get; set; } = new();
    public List<int> ColorIds { get; set; } = new();
    public List<int> SizeIds { get; set; } = new();
}

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    public bool IsActive { get; set; } = true;
    public List<string> ImageUrls { get; set; } = new();
    public List<int> ColorIds { get; set; } = new();
    public List<int> SizeIds { get; set; } = new();
}

public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    public bool IsActive { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<int> ColorIds { get; set; } = new();
    public List<int> SizeIds { get; set; } = new();
}




