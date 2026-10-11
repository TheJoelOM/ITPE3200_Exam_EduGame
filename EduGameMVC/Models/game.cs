//Game variables are listed here

namespace subapp2_Api.Models
{
    public class GameState
    {
        //Character Meters:
        public int PlayerHealth {get; set;}=100;
        public int EnemyHealth {get; set;}=100;
        public int SpecialPoint {get; set;}= 0; // Starts at zero, builds up after answering correct questions

        //Attacks are in Actions

        // Game states:
        public Boolean PlayerTurn {get; set;}= true; //will be used to switch between player and enemy
        public Boolean GameOver {get; set;}= false; //will determine if game is over or not


    }
}

