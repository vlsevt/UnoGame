using Domain;

namespace GameEngine;

public class UnoGameEngine
{
    private readonly Random _rnd = new Random();

    private readonly GameOptions _gameOptions;
    
    public const int MaxAmountOfPlayers = 10;

    public GameState State { get; set; } = new GameState();

    public int DrawsTwo;
    
    public int DrawsFour = 0;

    public bool Reverse = false;

    public bool WrongMove = false;

    public UnoGameEngine(GameOptions gameOptions)
    {
        _gameOptions = gameOptions;
    }

    public void MakePlayerMove(int playerChoice)
    {
        if (playerChoice == 0)
        {
            State.Players[State.ActivePlayerNo].PlayerHand.Add(State.DeckOfCardsInPlay.Last());
            State.DeckOfCardsInPlay.Remove(State.DeckOfCardsInPlay.Last());
        }
        var cardsToRemove = new List<GameCard>();
        if (State.Players[State.ActivePlayerNo].PlayerHand[playerChoice - 1].CardColor != State.CardsInPlayOnTheTable.ReceivedCards.Last().CardColor && 
            State.Players[State.ActivePlayerNo].PlayerHand[playerChoice - 1].CardColor != ECardColor.Wild && 
            State.Players[State.ActivePlayerNo].PlayerHand[playerChoice - 1].CardValue != State.CardsInPlayOnTheTable.ReceivedCards.Last().CardValue &&
            State.CardsInPlayOnTheTable.ReceivedCards.Last().CardColor != ECardColor.Wild)
        {
            WrongMove = true;
            return;
        }
        
        State.CardsInPlayOnTheTable.ReceivedCards.Add(
            State.Players[State.ActivePlayerNo].PlayerHand[playerChoice - 1]
        );
        
        cardsToRemove.Add(State.Players[State.ActivePlayerNo].PlayerHand[playerChoice - 1]);
        
        if (State.Players[State.ActivePlayerNo].PlayerHand[playerChoice - 1].CardValue == ECardValue.Skip || (State.Players[State.ActivePlayerNo].PlayerHand[playerChoice - 1].CardValue == ECardValue.Reverse && State.Players.Count == 2))
        {
            State.Players[State.ActivePlayerNo].PlayerHand.Remove(State.CardsInPlayOnTheTable.ReceivedCards.Last());
        } else if (State.Players[State.ActivePlayerNo].PlayerHand[playerChoice - 1].CardValue == ECardValue.DrawTwo)
        {
            DrawsTwo += 1;
        } else if (State.Players[State.ActivePlayerNo].PlayerHand[playerChoice - 1].CardValue == ECardValue.DrawFour)
        {
            DrawsFour += 1;
        } else if (State.Players[State.ActivePlayerNo].PlayerHand[playerChoice - 1].CardValue == ECardValue.Reverse && State.Players.Count > 2)
        {
            State.Players[State.ActivePlayerNo].PlayerHand.Remove(State.CardsInPlayOnTheTable.ReceivedCards.Last());
            Reverse = !Reverse;
        }

        cardsToRemove.ForEach(g => State.Players[State.ActivePlayerNo].PlayerHand.Remove(g));
        if (State.CardsInPlayOnTheTable.ReceivedCards.Last().CardValue == ECardValue.DrawFour ||
            State.CardsInPlayOnTheTable.ReceivedCards.Last().CardValue == ECardValue.ChangeColor)
        {
            ChangeColor();
        } else if (State.CardsInPlayOnTheTable.ReceivedCards.Last().CardValue == ECardValue.Reverse && State.CardsInPlayOnTheTable.ReceivedCards.Count != 1 && State.Players.Count > 2)
        {
            return;
        }

        if (State.CardsInPlayOnTheTable.ReceivedCards.Last().CardValue == ECardValue.DrawTwo || 
            State.CardsInPlayOnTheTable.ReceivedCards.Last().CardValue == ECardValue.DrawFour ||
            State.CardsInPlayOnTheTable.ReceivedCards.Last().CardValue == ECardValue.Skip ||
            State.CardsInPlayOnTheTable.ReceivedCards.Last().CardValue == ECardValue.Reverse && State.Players.Count == 2)
        {
            NextPlayerTurn();
        }

    }

    public void ChangeColor()
    {
        ECardColor cardColor;
        bool validInput = false;

        while (!validInput)
        {
            Console.WriteLine("Choose color: Blue / Yellow / Red / Green");
            string? input = Console.ReadLine();

            if (Enum.TryParse(input, out cardColor))
            {
                State.CardsInPlayOnTheTable.ReceivedCards.Last().CardColor = cardColor;
                validInput = true;
            }
            else
            {
                Console.WriteLine("Invalid color choice. Please try again.");
            }
        }
    }

    // public bool ValidatePlayerMove(IEnumerable<int> playerChoices)
    // {
    //     foreach (var playerChoice in playerChoices)
    //     {
    //         // no such card
    //         if (playerChoice > State.Players[State.ActivePlayerNo].PlayerHand.Count || playerChoice < 1) return false;
    //
    //         // card values have to match
    //         if (State.Players[State.ActivePlayerNo].PlayerHand[playerChoice - 1].CardValue !=
    //             State.Players[State.ActivePlayerNo].PlayerHand[playerChoices.First() - 1].CardValue) return false;
    //     }
    //
    //     return true;
    // }

    public void NextPlayerTurn()
    {
        
        foreach (var player in State.Players)
        {
            
            // while (player.PlayerHand.Count < 6)
            // {
            //     if (State.DeckOfCardsInPlay.Count <= 0) break;
            //     player.PlayerHand.Add(State.DeckOfCardsInPlay.Last());
            //     State.DeckOfCardsInPlay.RemoveAt(State.DeckOfCardsInPlay.Count - 1);
            // }
            
            // resort the hands
            player.PlayerHand = player.PlayerHand
                .OrderBy(c => c.CardValue)
                .ThenBy(c => c.CardColor)
                .ToList();
        }

        // Смена ходящего игрока
        if (WrongMove)
        {
            State.ActivePlayerNo = State.ActivePlayerNo;
            WrongMove = false;
            Console.WriteLine("Wrong Move");
        }
        else if (Reverse == false)
        {
            State.ActivePlayerNo++;
            if (State.ActivePlayerNo >= State.Players.Count) State.ActivePlayerNo = 0;
        }
        else if (Reverse)
        {
            State.ActivePlayerNo--;
            if (State.ActivePlayerNo < 0) State.ActivePlayerNo = State.Players.Count - 1;
        }

        for (int i = 0; i < DrawsTwo; i++)
        {
            State.Players[State.ActivePlayerNo].PlayerHand.Add(State.DeckOfCardsInPlay.Last());
            State.DeckOfCardsInPlay.Remove(State.DeckOfCardsInPlay.Last());
            State.Players[State.ActivePlayerNo].PlayerHand.Add(State.DeckOfCardsInPlay.Last());
            State.DeckOfCardsInPlay.Remove(State.DeckOfCardsInPlay.Last());

        }
        DrawsTwo = 0;

        for (int i = 0; i < DrawsFour; i++)
        {
            State.Players[State.ActivePlayerNo].PlayerHand.Add(State.DeckOfCardsInPlay.Last());
            State.DeckOfCardsInPlay.Remove(State.DeckOfCardsInPlay.Last());
            State.Players[State.ActivePlayerNo].PlayerHand.Add(State.DeckOfCardsInPlay.Last());
            State.DeckOfCardsInPlay.Remove(State.DeckOfCardsInPlay.Last());
            State.Players[State.ActivePlayerNo].PlayerHand.Add(State.DeckOfCardsInPlay.Last());
            State.DeckOfCardsInPlay.Remove(State.DeckOfCardsInPlay.Last());
            State.Players[State.ActivePlayerNo].PlayerHand.Add(State.DeckOfCardsInPlay.Last());
            State.DeckOfCardsInPlay.Remove(State.DeckOfCardsInPlay.Last());
        }
        DrawsFour = 0;
    }

    public void ShuffleAndDistributeCards()
    {
        // create the cards in deck
        for (int cardColor = 0; cardColor <= (int) ECardColor.Green; cardColor++)
        {
            for (int cardValue = 1; cardValue <= (int)ECardValue.DrawTwo; cardValue++)
            {
                State.DeckOfCardsInPlay.Add(new GameCard()
                {
                    CardColor = (ECardColor)cardColor,
                    CardValue = (ECardValue)cardValue,
                });
                State.DeckOfCardsInPlay.Add(new GameCard()
                {
                    CardColor = (ECardColor)cardColor,
                    CardValue = (ECardValue)cardValue,
                });
            }
        }

        for (int i = 0; i < 4; i++)
        {
            State.DeckOfCardsInPlay.Add(new GameCard()
            {
                CardColor = ECardColor.Wild ,
                CardValue = ECardValue.DrawFour ,
            });
            State.DeckOfCardsInPlay.Add(new GameCard()
            {
                CardColor = ECardColor.Wild ,
                CardValue = ECardValue.ChangeColor,
            });
        }
        State.DeckOfCardsInPlay.Add(new GameCard()
        {
            CardColor = ECardColor.Red ,
            CardValue = ECardValue.Zero ,
        });
        State.DeckOfCardsInPlay.Add(new GameCard()
        {
            CardColor = ECardColor.Yellow ,
            CardValue = ECardValue.Zero ,
        });
        State.DeckOfCardsInPlay.Add(new GameCard()
        {
            CardColor = ECardColor.Green ,
            CardValue = ECardValue.Zero ,
        });
        State.DeckOfCardsInPlay.Add(new GameCard()
        {
            CardColor = ECardColor.Blue ,
            CardValue = ECardValue.Zero ,
        });

        // shuffle
        var randomDeck = new List<GameCard>();
        while (State.DeckOfCardsInPlay.Count > 0)
        {
            var randomPositionInDeck = _rnd.Next(State.DeckOfCardsInPlay.Count);
            randomDeck.Add(State.DeckOfCardsInPlay[randomPositionInDeck]);
            State.DeckOfCardsInPlay.RemoveAt(randomPositionInDeck);
        }

        State.DeckOfCardsInPlay = randomDeck;

        // distribute to the players
        for (var playerNo = 0; playerNo < State.Players.Count; playerNo++)
        {
            for (var i = 0; i < _gameOptions.HandSize; i++)
            {
                State.Players[playerNo].PlayerHand.Add(State.DeckOfCardsInPlay.Last());
                State.DeckOfCardsInPlay.RemoveAt(State.DeckOfCardsInPlay.Count - 1);
            }

            State.Players[playerNo].PlayerHand = State.Players[playerNo].PlayerHand
                .OrderBy(c => c.CardValue)
                .ThenBy(c => c.CardColor)
                .ToList();
        }

        // get first card
        State.FirstCard = State.DeckOfCardsInPlay.First();
    }
}