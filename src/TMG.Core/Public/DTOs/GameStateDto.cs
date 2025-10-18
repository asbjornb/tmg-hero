namespace TMG.Core.Public.DTOs;

public sealed class GameStateDto
{
    public int BuildingCount { get; init; }
    public Dictionary<string, ResourceDto> Resources { get; init; } = new();
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}