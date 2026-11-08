using EduGameMVC.Models;

namespace EduGameMVC.Services
{
    public class BattleService
    {
        private const int MaxHealth = 100;
        private const int PoisonDamagePerTurn = 6;
        private const int PoisonDuration = 5;

        public GameState StartBattle()
        {
            return new GameState();
        }

        public bool UseMove(GameState game, string move)
        {
            if (!CanPlayerAct(game))
                return false;

            switch (move)
            {
                case "Light":
                    DamageEnemy(game, Random.Shared.Next(6, 13));
                    break;

                case "Heavy":
                    if (!SpendSpecialPoints(game, 3))
                        return false;

                    DamageEnemy(game, Random.Shared.Next(12, 19));
                    break;

                case "Block":
                    game.BlockNextAttack = true;
                    break;

                case "Heal":
                    if (game.PlayerHealth >= MaxHealth ||
                        !SpendSpecialPoints(game, 12))
                    {
                        return false;
                    }

                    game.PlayerHealth = Math.Min(
                        MaxHealth,
                        game.PlayerHealth + (int)(MaxHealth * 0.40));
                    break;

                case "Fireball":
                    if (!SpendSpecialPoints(game, 10))
                        return false;

                    DamageEnemy(game, 30);
                    break;

                case "Poison":
                    if (!SpendSpecialPoints(game, 8))
                        return false;

                    // Reapplying Poison refreshes its duration without stacking damage.
                    game.EnemyPoisonTurnsRemaining = PoisonDuration;
                    break;

                default:
                    return false;
            }

            return true;
        }

        public void EnemyAttack(GameState game)
        {
            if (game.GameOver)
                return;

            int damage = Random.Shared.Next(5, 11);

            if (game.BlockNextAttack)
            {
                damage = (damage + 1) / 2;
                game.BlockNextAttack = false;
            }

            game.PlayerHealth -= damage;
            CheckGameOver(game);
        }

        public void ApplyPoisonTick(GameState game)
        {
            if (game.GameOver || game.EnemyPoisonTurnsRemaining <= 0)
                return;

            game.EnemyHealth -= PoisonDamagePerTurn;
            game.EnemyPoisonTurnsRemaining--;

            CheckGameOver(game);
        }

        public void AwardSpecialPoints(GameState game, int amount)
        {
            if (game.GameOver || amount <= 0)
                return;

            game.SpecialPoints += amount;
        }

        private static bool CanPlayerAct(GameState game)
        {
            return !game.GameOver
                && game.PlayerTurn
                && game.PlayerHealth > 0
                && game.EnemyHealth > 0;
        }

        private static bool SpendSpecialPoints(GameState game, int cost)
        {
            if (game.SpecialPoints < cost)
                return false;

            game.SpecialPoints -= cost;
            return true;
        }

        private static void DamageEnemy(GameState game, int damage)
        {
            game.EnemyHealth -= damage;
            CheckGameOver(game);
        }

        private static void CheckGameOver(GameState game)
        {
            if (game.PlayerHealth <= 0)
            {
                game.PlayerHealth = 0;
                game.GameOver = true;
            }
            else if (game.EnemyHealth <= 0)
            {
                game.EnemyHealth = 0;
                game.GameOver = true;
            }
        }
    }
}

