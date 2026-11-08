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
        public IActionResult Attack(string move)
        {
            var game = LoadGame();

            if (game == null)
                return RedirectToAction(nameof(Index));

            if (game.GameOver || game.EnemyTurnPending || game.EnemyAttackPending)
                return RedirectToAction(nameof(Index));

            game.PreviousEnemyHealth = game.EnemyHealth;

            if (!_battleService.UseMove(game, move))
            {
                SaveGame(game);
                return RedirectToAction(nameof(Index));
            }

            game.BattleMessage = move switch
            {
                "Light" => "You used Light Attack!",
                "Heavy" => "You used Heavy Attack!",
                "Block" => "You brace for the monster's next attack!",
                "Heal" => "You restored your health!",
                "Fireball" => "You cast Fireball!",
                "Poison" => "The monster is poisoned!",
                _ => "You used a move!"
            };

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

            if (game.GameOver || !game.EnemyTurnPending || game.EnemyAttackPending)
                return RedirectToAction(nameof(Index));

            // The player's damage animation has completed. Use current health
            // as the next page's starting point so the bar cannot jump backward.
            game.PreviousEnemyHealth = game.EnemyHealth;

            game.BattleMessage = "The monster attacks!";
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
            game.PreviousEnemyHealth = game.EnemyHealth;

            _battleService.EnemyAttack(game);

            if (!game.GameOver)
                _battleService.ApplyPoisonTick(game);

            game.EnemyAttackPending = false;
            game.EnemyTurnPending = false;

            game.BattleMessage = game.PlayerHealth <= 0
                ? "Defeat!"
                : game.EnemyHealth <= 0
                    ? "Victory!"
                    : "Choose a move!";

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
            HttpContext.Session.SetString(
                GameSessionKey,
                JsonSerializer.Serialize(game));
        }
    }
}
