namespace Domain;

public class GameCard
{
    public ECardColor CardColor { get; set; }
    public ECardValue CardValue { get; set; }

    public override string ToString()
    {
        return CardColorToString + " " + CardValue.ToString();
    }

    private string CardColorToString =>
        CardColor switch
        {
            ECardColor.Blue => "Blue",
            ECardColor.Green => "Green",
            ECardColor.Red => "Red",
            ECardColor.Yellow => "Yellow",
            ECardColor.Wild => "Wild",
            _ => "-"
        };
}