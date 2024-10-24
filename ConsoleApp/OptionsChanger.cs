using Domain;

namespace ConsoleApp;

public static class OptionsChanger
{
    public static string? ConfigureHandSize(GameOptions gameOptions)
    {
        while (true)
        {
            Console.Write($"Enter hand size (2-10):");
            var sizeStr = Console.ReadLine();

            if (sizeStr == null) continue;

            if (!int.TryParse(sizeStr, out var size))
            {
                Console.WriteLine("Parse error...");
                continue;
            }

            if (size < 2 || size > 10)
            {
                Console.WriteLine("Out of range...");
                continue;
            }


            gameOptions.HandSize = size;
            return null;
        }
    }

    // number of cards in one suite
    private static int GetMaxHandSize => 7;
    
}