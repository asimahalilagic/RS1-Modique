namespace Modique.Application.DTOs.Brands;

public class BrandDto
{
    public int BrandId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string LogoURL { get; set; } = string.Empty;
}

public class CreateBrandDto
{
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string LogoURL { get; set; } = string.Empty;
}

public class UpdateBrandDto
{
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string LogoURL { get; set; } = string.Empty;
}
