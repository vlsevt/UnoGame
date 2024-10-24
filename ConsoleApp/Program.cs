// See https://aka.ms/new-console-template for more information

using ConsoleApp;
using ConsoleUI;
using DAL;
using Domain;
using GameEngine;
using Microsoft.EntityFrameworkCore;

// ================== SETUP =====================
var gameOptions = new GameOptions();

// state saving functionality, can be either file system based or db based. uses the same interface for both
// IGameRepository gameRepository = new GameRepositoryFileSystem();

var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite("Data Source=app.db")
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging()
    .Options;
using var db = new AppDbContext(contextOptions);
// apply all the migrations
db.Database.Migrate();
IGameRepository gameRepository = new GameRepositoryEf(db);


var mainMenu = ProgramMenus.GetMainMenu(
    gameOptions,
    ProgramMenus.GetOptionsMenu(gameOptions),
    NewGame,
    LoadGame
);

// ================== MAIN =====================
mainMenu.Run();

// ================ THE END ==================
return;


// ================== NEW GAME =====================
string? NewGame()
{
    // game logic, shared between console and web
    var gameEngine = new UnoGameEngine(gameOptions);

    // set up players
    PlayerSetup.ConfigurePlayers(gameEngine);

    // set up the table
    gameEngine.ShuffleAndDistributeCards();

    // console controller for game loop
    var gameController = new GameController(gameEngine, gameRepository);

    gameController.Run();
    return null;
}

// ================== LOAD GAME =====================
string? LoadGame()
{
    Console.WriteLine("Saved games");
    var saveGameList = gameRepository.GetSaveGames();
    var saveGameListDisplay = saveGameList.Select((s, i) => (i + 1) + " - " + s).ToList();

    if (saveGameListDisplay.Count == 0) return null;

    Guid gameId;
    while (true)
    {
        Console.WriteLine(string.Join("\n", saveGameListDisplay));
        Console.Write($"Select game to load (1..{saveGameListDisplay.Count}):");
        var userChoiceStr = Console.ReadLine();
        if (int.TryParse(userChoiceStr, out var userChoice))
        {
            if (userChoice < 1 || userChoice > saveGameListDisplay.Count)
            {
                Console.WriteLine("Not in range...");
                continue;
            }

            gameId = saveGameList[userChoice - 1].id;
            Console.WriteLine($"Loading file: {gameId}");

            break;
        }

        Console.WriteLine("Parse error...");
    }


    var gameState = gameRepository.LoadGame(gameId);

    var gameEngine = new UnoGameEngine(gameOptions)
    {
        State = gameState
    };
    
    var gameController = new GameController(gameEngine, gameRepository);

    gameController.Run();

    return null;
}