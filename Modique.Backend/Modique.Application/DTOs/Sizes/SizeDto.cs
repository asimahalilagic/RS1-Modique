namespace Modique.Application.DTOs.Sizes;

public class SizeDto
{
    public int SizeId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateSizeDto
{
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateSizeDto
{
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}
