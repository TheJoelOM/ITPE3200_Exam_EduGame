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
    }
}


