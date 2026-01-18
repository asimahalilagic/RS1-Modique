namespace Modique.Application.DTOs.Colors;

public class ColorDto
{
    public int ColorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ColorCode { get; set; } = "#FFFFFF";
}

public class CreateColorDto
{
    public string Name { get; set; } = string.Empty;
    public string ColorCode { get; set; } = "#FFFFFF";
}

public class UpdateColorDto
{
    public string Name { get; set; } = string.Empty;
    public string ColorCode { get; set; } = "#FFFFFF";
}
