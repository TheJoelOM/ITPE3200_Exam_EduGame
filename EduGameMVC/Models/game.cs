namespace EduGameMVC.Models
{
    public class GameState
    {
        public int PlayerHealth { get; set; } = 100;
        public int EnemyHealth { get; set; } = 100;
        public int SpecialPoints { get; set; } = 0;

        public bool PlayerTurn { get; set; } = true;
        public bool GameOver { get; set; } = false;
        public bool EnemyTurnPending { get; set; } = false;
        public bool EnemyAttackPending { get; set; } = false;

        public bool BlockNextAttack { get; set; } = false;
        public int EnemyPoisonTurnsRemaining { get; set; } = 0;

        public int PreviousPlayerHealth { get; set; } = 100;
        public int PreviousEnemyHealth { get; set; } = 100;

        public string BattleMessage { get; set; } = "Choose a move!";
    }
}
