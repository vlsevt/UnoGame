using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Domain;
using Domain.Database;
using Helpers;

namespace DAL
{
    public class GameRepositoryEf : IGameRepository
    {
        private readonly AppDbContext _ctx;

        public GameRepositoryEf(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public void Save(Guid id, GameState state)
        {
            var game = _ctx.Games.FirstOrDefault(g => g.Id == id);

            if (game == null)
            {
                game = new Game
                {
                    Id = id,
                    State = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions),
                    Players = state.Players.Select(p => new Domain.Database.Player // Specify the namespace to resolve the ambiguity
                    {
                        NickName = p.NickName,
                        PlayerType = p.PlayerType
                    }).ToList(),
                    CreatedAtDt = DateTime.Now
                };

                _ctx.Games.Add(game);
            }
            else
            {
                game.UpdatedAtDt = DateTime.Now;
                game.State = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions);
            }

            _ctx.SaveChanges();
        }

        public List<(Guid id, DateTime dt)> GetSaveGames()
        {
            return _ctx.Games
                .OrderByDescending(g => g.UpdatedAtDt)
                .Select(g => new { Id = g.Id, UpdatedAtDt = g.UpdatedAtDt })
                .AsEnumerable()
                .Select(g => (g.Id, g.UpdatedAtDt))
                .ToList();
        }

        public GameState? LoadGame(Guid id)
        {
            var game = _ctx.Games.FirstOrDefault(g => g.Id == id);

            if (game != null)
            {
                var deserializedState = JsonSerializer.Deserialize<GameState>(game.State, JsonHelpers.JsonSerializerOptions);

                if (deserializedState != null)
                {
                    return deserializedState;
                }
                else
                {
                    // Handle the case where deserialization failed
                    throw new InvalidOperationException("Deserialization failed for GameState.");
                }
            }

            // Handle not found case, return null or throw an exception
            return null;
        }



        public void Delete(Guid id)
        {
            var game = _ctx.Games.FirstOrDefault(g => g.Id == id);

            if (game != null)
            {
                _ctx.Games.Remove(game);
                _ctx.SaveChanges();
            }
        }

        public void DeleteAllGames()
        {
            var games = _ctx.Games.ToList();

            if (games.Count > 0)
            {
                _ctx.Games.RemoveRange(games);
                _ctx.SaveChanges();
            }
        }
    }
}
