namespace Domain;

public class GameState
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public GameCard FirstCard { get; set; } = new GameCard();
    
    public List<GameCard> DeckOfCardsInPlay { get; set; } = new List<GameCard>();
    public CardsInPlayOnTheTable CardsInPlayOnTheTable { get; set; } = new CardsInPlayOnTheTable();
    public int ActivePlayerNo { get; set; } = 0;

    public EReceiverDecision ReceiverDecision { get; set; } = EReceiverDecision.NoneYet;

    public List<Player> Players { get; set; } = new List<Player>();

}