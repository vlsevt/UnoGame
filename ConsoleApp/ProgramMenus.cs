using Domain;
using GameEngine;
using MenuSystem;
using DAL;

namespace ConsoleApp;

public static class ProgramMenus
{
    public static Menu GetOptionsMenu(GameOptions gameOptions) =>
        new Menu("Options", new List<MenuItem>()
        {
            new MenuItem()
            {
                Shortcut = "h",
                MenuLabelFunction = () => "Hand size - " + gameOptions.HandSize,
                MethodToRun = () => OptionsChanger.ConfigureHandSize(gameOptions)
            }
        });
    
    
    public static Menu GetMainMenu(GameOptions gameOptions, Menu optionsMenu, Func<string?> newGameMethod, Func<string?> loadGameMethod) => 
        new Menu(">> U N O <<", new List<MenuItem>()
        {
            new MenuItem()
            {
                Shortcut = "s",
                MenuLabel = "Start a new game: ",
                MenuLabelFunction = () => "Start a new game: " + gameOptions,
                MethodToRun = newGameMethod
            },
            new MenuItem()
            {
                Shortcut = "l",
                MenuLabel = "Load game",
                MethodToRun = loadGameMethod
            },
            new MenuItem()
            {
                Shortcut = "o",
                MenuLabel = "Options",
                MethodToRun = () => optionsMenu.Run(EMenuLevel.Second)
            }
        });
}