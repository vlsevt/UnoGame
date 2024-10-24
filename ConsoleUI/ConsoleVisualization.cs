using Domain;

namespace ConsoleUI;

public static class ConsoleVisualization
{
    public static void DrawDesk(GameState state)
    {
        Console.WriteLine($"Cards in deck: {state.DeckOfCardsInPlay.Count}");

        for (var i = 0; i < state.Players.Count; i++)
        {
            Console.WriteLine(
                $"Player {i + 1} - {state.Players[i].NickName} has {state.Players[i].PlayerHand.Count} cards");
        }
        
        Console.WriteLine($"Cards in active play: " +
                          string.Join(" ", state.CardsInPlayOnTheTable.ReceivedCards.Select(c => c.ToString()).Last()));
    }

    public static void DrawPlayerHand(Player player)
    {
        Console.WriteLine("Your current hand is: " +
                          string.Join(
                              "  ",
                              player.PlayerHand.Select((c, i) => (i+1) + ": " + c)
                          )
        );
    }
}