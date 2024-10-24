using DAL;
using Domain;
using GameEngine;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace ConsoleUI;

public class GameController
{
    private readonly UnoGameEngine _engine;
    private readonly IGameRepository _repository;
    
    public GameController(UnoGameEngine engine, IGameRepository repository)
    {
        _engine = engine;
        _repository = repository;
    }

    public void Run()
    {
        while (true)
        {
            if (_engine.State.DeckOfCardsInPlay.First().CardColor != ECardColor.Wild
                && _engine.State.DeckOfCardsInPlay.First().CardValue != ECardValue.Skip
                && _engine.State.DeckOfCardsInPlay.First().CardValue != ECardValue.DrawTwo
                && _engine.State.DeckOfCardsInPlay.First().CardValue != ECardValue.Reverse)
            {
                _engine.State.CardsInPlayOnTheTable.ReceivedCards.Add(_engine.State.DeckOfCardsInPlay.First());
                _engine.State.DeckOfCardsInPlay.Remove(_engine.State.DeckOfCardsInPlay.First());
                break;
            }
            _engine.State.DeckOfCardsInPlay.Add(_engine.State.DeckOfCardsInPlay.First());
            _engine.State.DeckOfCardsInPlay.Remove(_engine.State.DeckOfCardsInPlay.First());
        }

        Console.Clear();
        string minCardPlayer = "";
        while (true)
        {
            bool zeroCards = false;
            foreach (var player in _engine.State.Players)
            {
                if (player.PlayerHand.Count == 0)
                {
                    zeroCards = true;
                }
            }
            if (_engine.State.DeckOfCardsInPlay.Count == 0 || zeroCards)
            {
                var min = 108;
                foreach (var player in _engine.State.Players)
                {
                    if (player.PlayerHand.Count < min)
                    {
                        min = player.PlayerHand.Count;
                        minCardPlayer = player.NickName;
                    }
                    else if (player.PlayerHand.Count == min)
                    {
                        Console.WriteLine("Draw. Friendship wins!");
                        Environment.Exit(1);
                    }
                }
                Console.WriteLine($"The winner has been decided! Player {minCardPlayer} won!");
                _repository.Delete(_engine.State.Id);
                Environment.Exit(1);
            }
            
            // one move in loop
            Console.WriteLine($"Player {_engine.State.ActivePlayerNo + 1} - {_engine.State.Players[_engine.State.ActivePlayerNo].NickName}");
            Console.Write("Your turn, make sure you are alone looking at screen! Press enter to continue...");
            Console.ReadLine();
            
            Console.Clear();
            
            Console.WriteLine($"Player {_engine.State.ActivePlayerNo + 1} - {_engine.State.Players[_engine.State.ActivePlayerNo].NickName}");
            ConsoleVisualization.DrawDesk(_engine.State);

            while (true)
            {
                bool cardToMakeMove = false;
                foreach (var card in _engine.State.Players[_engine.State.ActivePlayerNo].PlayerHand)
                {
                    if (card.CardColor == _engine.State.CardsInPlayOnTheTable.ReceivedCards.Last().CardColor || 
                        card.CardColor == ECardColor.Wild || 
                        card.CardValue == _engine.State.CardsInPlayOnTheTable.ReceivedCards.Last().CardValue ||
                        _engine.State.CardsInPlayOnTheTable.ReceivedCards.Last().CardColor == ECardColor.Wild)
                    {
                        cardToMakeMove = true;
                    }
                }

                if (cardToMakeMove == false)
                {
                    _engine.State.Players[_engine.State.ActivePlayerNo].PlayerHand.Add(_engine.State.DeckOfCardsInPlay.Last());
                    _engine.State.DeckOfCardsInPlay.Remove(_engine.State.DeckOfCardsInPlay.Last());
                    Console.WriteLine("You don't have cards to play");
                    break;
                }

                string? playerChoice;
                while (true)
                {
                    ConsoleVisualization.DrawPlayerHand(_engine.State.Players[_engine.State.ActivePlayerNo]);
                    Console.Write($"Choose card to play 1-{_engine.State.Players[_engine.State.ActivePlayerNo].PlayerHand.Count} or type 0 to take a card from deck: ");
                    playerChoice = Console.ReadLine();
                    if (int.TryParse(playerChoice, out _))
                    {
                        if (int.Parse(playerChoice) <= _engine.State.Players[_engine.State.ActivePlayerNo].PlayerHand.Count)
                        {
                            break;
                        }
                        Console.WriteLine("Number not in range!");
                    }
                    Console.WriteLine("Parse error");
                }
                
                _engine.MakePlayerMove(int.Parse(playerChoice));
                break;
            }
            
            ConsoleVisualization.DrawDesk(_engine.State);
            

            
            _engine.NextPlayerTurn();
            
            _repository.Save(_engine.State.Id, _engine.State);
            
            Console.Write("State saved. Continue (Y/N)[Y]?");
            var continueAnswer = Console.ReadLine()?.Trim().ToLower();
            
            if (continueAnswer is "n") break;
        }
    }
    
}