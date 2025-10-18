namespace TMG.Core.Public.DTOs;

public sealed class ResourceDto
{
    public string Name { get; init; } = string.Empty;
    public int Amount { get; init; }
    public int Cap { get; init; }
    public double Income { get; init; }

    public bool IsCapped => Amount >= Cap;
}