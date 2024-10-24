using System.Runtime.Serialization;
using System.Text.Json;
using Domain;
using Helpers;

namespace DAL;

public class GameRepositoryFileSystem : IGameRepository
{
    // TODO: figure out system dependent location - maybe Path.GetTempPath()
    private const string SaveLocation = "C:/Users/user/uno_games";

    public void Save(Guid id, GameState state)
    {
        var content = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions);

        var fileName = Path.ChangeExtension(id.ToString(), ".json");

        if (!Path.Exists(SaveLocation))
        {
            Directory.CreateDirectory(SaveLocation);
        }

        File.WriteAllText(Path.Combine(SaveLocation, fileName), content);
    }

    public List<(Guid id, DateTime dt)> GetSaveGames()
    {
        var data = Directory.EnumerateFiles(SaveLocation);
        var res = data
            .Select(
                path => (
                    Guid.Parse(Path.GetFileNameWithoutExtension(path)),
                    File.GetLastWriteTime(path)
                )
            ).ToList();
        
        return res;
    }

    public GameState LoadGame(Guid id)
    {
        var fileName = Path.ChangeExtension(id.ToString(), ".json");

        var jsonStr = File.ReadAllText(Path.Combine(SaveLocation, fileName));
        var res = JsonSerializer.Deserialize<GameState>(jsonStr, JsonHelpers.JsonSerializerOptions);
        if (res == null) throw new SerializationException($"Cannot deserialize {jsonStr}");

        return res;
    }

    public void Delete(Guid id)
    {
        var fileName = Path.ChangeExtension(id.ToString(), ".json");
        var filePath = Path.Combine(SaveLocation, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
    
    public void DeleteAllGames()
    {
        var gameFiles = Directory.EnumerateFiles(SaveLocation);

        foreach (var file in gameFiles)
        {
            File.Delete(file);
        }
    }

}
