namespace TMG.Core.Public.DTOs;

public sealed class BuildingDto
{
    public string Name { get; init; } = string.Empty;
    public Dictionary<string, int> Cost { get; init; } = new();
    public Dictionary<string, double> Production { get; init; } = new();
    public int? Population { get; init; }

    public bool IsHousing => Population.HasValue;
}