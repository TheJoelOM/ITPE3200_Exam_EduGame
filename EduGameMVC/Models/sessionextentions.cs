using System.Text.Json;

namespace EduGameMVC.Models
{
    public static class SessionExtensions
    {
        public static GameState GetGame(this ISession session)
        {
            var json = session.GetString("game");
            return json == null ? new GameState() : JsonSerializer.Deserialize<GameState>(json)!;
        }

        public static void SaveGame(this ISession session, GameState game)
            => session.SetString("game", JsonSerializer.Serialize(game));
    }
}