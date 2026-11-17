using System.Security.Claims;
using EduGameMVC.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduGameMVC.Controllers;

public class AdventureController : Controller
{
    private readonly IQuestionRepository _questions;
    public AdventureController(IQuestionRepository questions) => _questions = questions;

    [Authorize]
    public async Task<IActionResult> Custom()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return View(await _questions.GetSubjectsForOwnerAsync(id));
    }

    public IActionResult Datasikkerhet() => RedirectToAction("StartDatasikkerhet", "Game");
}
