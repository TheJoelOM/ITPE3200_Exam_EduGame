
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

            // Reject actions while the enemy's turn is in progress.
            if (game.GameOver || game.EnemyTurnPending || game.EnemyAttackPending)
                return RedirectToAction(nameof(Index));

            game.PreviousEnemyHealth = game.EnemyHealth;

            bool succeeded = attackType switch
            {
                "Light" => _battleService.LightAttack(game),
                "Heavy" => _battleService.HeavyAttack(game),
                _ => false
            };

            if (!succeeded)
            {
                SaveGame(game);
                return RedirectToAction(nameof(Index));
            }

            game.BattleMessage = attackType == "Light"
                ? "You used Light Attack!"
                : "You used Heavy Attack!";

            if (game.GameOver)
            {
                game.BattleMessage = "Victory!";
            }
            else
            {
                game.EnemyTurnPending = true;
            }

            SaveGame(game);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnemyTurn()
        {
            var game = LoadGame();

            if (game == null)
                return RedirectToAction(nameof(Index));

            // Only announce an enemy move after a valid player action.
            if (game.GameOver || !game.EnemyTurnPending || game.EnemyAttackPending)
                return RedirectToAction(nameof(Index));

            game.BattleMessage = "The enemy uses Basic Attack!";
            game.EnemyAttackPending = true;

            SaveGame(game);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResolveEnemyAttack()
        {
            var game = LoadGame();

            if (game == null)
                return RedirectToAction(nameof(Index));

            if (game.GameOver || !game.EnemyTurnPending || !game.EnemyAttackPending)
                return RedirectToAction(nameof(Index));

            game.PreviousPlayerHealth = game.PlayerHealth;

            _battleService.EnemyAttack(game);

            game.EnemyAttackPending = false;
            game.EnemyTurnPending = false;

            game.BattleMessage = game.GameOver
                ? "Defeat!"
                : "Choose your move!";

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

