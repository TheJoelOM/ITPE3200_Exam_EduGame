using Microsoft.AspNetCore.Mvc;
using EduGameMVC.Models;

namespace EduGameMVC.Controllers;
public class GameController : Controller
{  public static GameController currentGame = new GameController(); // change this later this is just temporary.

    
    public IActionResult lightAttack()
    {
        int damage = Random.Shared.Next(6, 12);

        //currentGame.EnemyHealth -= damage; will add later it is giving error for now.
        return View();
    }

        public IActionResult HeavyAttack()
    {
        int damage = Random.Shared.Next(12, 19);

        //currentGame.EnemyHealth -= damage; will add later it is giving error for now.
        return View();
    }
}