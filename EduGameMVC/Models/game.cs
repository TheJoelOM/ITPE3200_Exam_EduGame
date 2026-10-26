//Game variables are listed here


namespace EduGameMVC.Models
{
    public class GameState
    {
        // Character health
        public int PlayerHealth { get; set; } = 100;
        public int EnemyHealth { get; set; } = 100;

        // Special Points earned from correct answers
        public int SpecialPoints { get; set; } = 0;

        // Battle status
        public bool PlayerTurn { get; set; } = true;
        public bool GameOver { get; set; } = false;

        // Battle sequence
        public bool EnemyTurnPending { get; set; } = false;
        public bool EnemyAttackPending { get; set; } = false;

        // Previous health values allow the UI to animate health loss.
        public int PreviousPlayerHealth { get; set; } = 100;
        public int PreviousEnemyHealth { get; set; } = 100;

        // Message displayed during the battle.
        public string BattleMessage { get; set; } = "Choose your move!";
    }
}


