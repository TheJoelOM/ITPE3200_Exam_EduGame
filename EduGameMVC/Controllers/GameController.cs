
using System.Text.Json;
using EduGameMVC.Models;
using EduGameMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace EduGameMVC.Controllers
{
    public class GameController : Controller
    {
        private readonly BattleService _battleService;
        private const string GameSessionKey = "CurrentGame";

        public GameController(BattleService battleService)
        {
            _battleService = battleService;
        }

        public IActionResult Index()
        {
            var game = LoadGame();

            if (game == null)
            {
                game = _battleService.StartBattle();
                SaveGame(game);
            }

            return View(game);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Attack(string attackType)
        {
            var game = LoadGame();

            if (game == null)
                return RedirectToAction(nameof(Index));

            if (attackType == "Light")
            {
                _battleService.LightAttack(game);
            }
            else if (attackType == "Heavy")
            {
                _battleService.HeavyAttack(game);
            }

            // If the enemy was defeated, it cannot attack.
            if (!game.GameOver)
            {
                _battleService.EnemyAttack(game);
            }

            SaveGame(game);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Restart()
        {
            var game = _battleService.StartBattle();
            SaveGame(game);

            return RedirectToAction(nameof(Index));
        }

        private GameState? LoadGame()
        {
            var json = HttpContext.Session.GetString(GameSessionKey);

            if (string.IsNullOrEmpty(json))
                return null;

            return JsonSerializer.Deserialize<GameState>(json);
        }

        private void SaveGame(GameState game)
        {
            var json = JsonSerializer.Serialize(game);
            HttpContext.Session.SetString(GameSessionKey, json);
        }
    }
}

