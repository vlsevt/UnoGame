namespace Domain;

public class GameOptions
{
    // max hand size during gameplay
    public int HandSize { get; set; } = 7;

    public override string ToString() => $"hand size: {HandSize}";
}