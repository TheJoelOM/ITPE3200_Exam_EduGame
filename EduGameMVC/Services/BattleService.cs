
using EduGameMVC.Models;

namespace EduGameMVC.Services
{
    public class BattleService
    {
        private const int HeavyAttackCost = 3;

        public GameState StartBattle()
        {
            return new GameState();
        }

        public void LightAttack(GameState game)
        {
            if (!CanPlayerAct(game))
                return;

            int damage = Random.Shared.Next(6, 12);
            game.EnemyHealth -= damage;

            CheckGameOver(game);
        }

        public bool HeavyAttack(GameState game) 
        {
            if (!CanPlayerAct(game))
                return false;

            if (game.SpecialPoints < HeavyAttackCost) // this move requires sp
                return false;

            game.SpecialPoints -= HeavyAttackCost;

            int damage = Random.Shared.Next(12, 19);
            game.EnemyHealth -= damage;

            CheckGameOver(game);

            return true;
        }

        public void EnemyAttack(GameState game)
        {
            if (game.GameOver)
                return;

            int damage = Random.Shared.Next(5, 11);
            game.PlayerHealth -= damage;

            CheckGameOver(game);
        }

        private bool CanPlayerAct(GameState game) 
        {
            return !game.GameOver
                && game.PlayerTurn
                && game.PlayerHealth > 0
                && game.EnemyHealth > 0;
        }

        private void CheckGameOver(GameState game)
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
