using System.Text.Json;
using EduGameMVC.Models;
using EduGameMVC.Repositories;
using EduGameMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace EduGameMVC.Controllers
{
    public class GameController : Controller
    {
        private readonly BattleService _battleService;
        private readonly IQuestionRepository _questionRepository;
        private const string GameSessionKey = "CurrentGame";

        public GameController(BattleService battleService, IQuestionRepository questionRepository)
        {
            _battleService = battleService;
            _questionRepository = questionRepository;
        }

        public async Task<IActionResult> Index()
        {
            var game = LoadGame();

            if (game == null)
            {
                game = _battleService.StartBattle();
                SaveGame(game);
            }

            if (game.CurrentQuestionId == null && !game.GameOver && !game.EnemyTurnPending)
            {
                var question = await _questionRepository.GetRandomAsync();
                game.CurrentQuestionId = question?.Id;
                game.QuestionAnswered = question == null;
                SaveGame(game);
            }

            ViewBag.CurrentQuestion = game.CurrentQuestionId is int questionId
                ? await _questionRepository.GetByIdAsync(questionId)
                : null;

            return View(game);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Answer(string option)
        {
            var game = LoadGame();
            if (game == null || game.GameOver || game.EnemyTurnPending || game.QuestionAnswered || game.CurrentQuestionId == null)
                return RedirectToAction(nameof(Index));

            var question = await _questionRepository.GetByIdAsync(game.CurrentQuestionId.Value);
            if (question == null)
                return RedirectToAction(nameof(Index));

            game.LastAnswerWasCorrect = string.Equals(option, question.CorrectOption, StringComparison.OrdinalIgnoreCase);
            if (game.LastAnswerWasCorrect)
                _battleService.AwardCorrectAnswer(game, question.Difficulty);

            game.QuestionAnswered = true;
            game.BattleMessage = game.LastAnswerWasCorrect
                ? $"Correct! You earned SP ({question.Difficulty}). Choose a move."
                : "Incorrect. You earned 0 SP, but can still choose a move.";
            SaveGame(game);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Attack(string move)
        {
            var game = LoadGame();

            if (game == null)
                return RedirectToAction(nameof(Index));

            if (game.GameOver || game.EnemyTurnPending || game.EnemyAttackPending || !game.QuestionAnswered)
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
            game.CurrentQuestionId = null;
            game.QuestionAnswered = false;
            game.LastAnswerWasCorrect = false;

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
