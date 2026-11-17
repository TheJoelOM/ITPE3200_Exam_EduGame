using EduGameMVC.Models;
using EduGameMVC.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EduGameMVC.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly ILogger<QuestionsController> _logger;

        public QuestionsController(
            IQuestionRepository questionRepository,
            ILogger<QuestionsController> logger)
        {
            _questionRepository = questionRepository;
            _logger = logger;
        }

        // READ lists the questions
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var all = await _questionRepository.GetAllAsync();
            var questions = User.IsInRole("Admin") ? all : all.Where(q => q.OwnerId == User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View(questions);
        }

        // READ shows one question
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id);

            if (question == null || !CanAccess(question))
            {
                _logger.LogWarning(
                    "Question with id {QuestionId} was not found.",
                    id);

                return NotFound();
            }

            return View(question);
        }

        // CREATE show form
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // CREATE process form
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Question question)
        {
            if (!ModelState.IsValid)
            {
                return View(question);
            }

            question.OwnerId = User.IsInRole("Admin") && question.Subject == "Datasikkerhet"
                ? null : User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _questionRepository.AddAsync(question);

            _logger.LogInformation(
                "Question {QuestionId} was created.",
                question.Id);

            return RedirectToAction(nameof(Index));
        }

        // UPDATE show edit form
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id);

            if (question == null || !CanManage(question))
            {
                _logger.LogWarning(
                    "Question with id {QuestionId} was not found for editing.",
                    id);

                return NotFound();
            }

            return View(question);
        }

        // UPDATE process edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, Question question)
        {
            if (id != question.Id)
            {
                return BadRequest();
            }

            var existing = await _questionRepository.GetByIdAsync(id);
            if (existing == null || !CanManage(existing)) return NotFound();
            question.OwnerId = existing.OwnerId;

            if (!ModelState.IsValid)
            {
                return View(question);
            }

            try
            {
                await _questionRepository.UpdateAsync(question);

                _logger.LogInformation(
                    "Question {QuestionId} was updated.",
                    question.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while updating question {QuestionId}.",
                    question.Id);

                ModelState.AddModelError(
                    "",
                    "An error occurred while updating the question.");

                return View(question);
            }

            return RedirectToAction(nameof(Index));
        }

        // DELETE confirmation page
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id);

            if (question == null || !CanManage(question))
            {
                _logger.LogWarning(
                    "Question with id {QuestionId} was not found for deletion.",
                    id);

                return NotFound();
            }

            return View(question);
        }

        // DELETE this will delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id);

            if (question == null || !CanManage(question))
            {
                return NotFound();
            }

            await _questionRepository.DeleteAsync(id);

            _logger.LogInformation(
                "Question {QuestionId} was deleted.",
                id);

            return RedirectToAction(nameof(Index));
        }

        private bool CanAccess(Question question) => CanManage(question) || question.OwnerId == null;
        private bool CanManage(Question question) => User.IsInRole("Admin") || question.OwnerId == User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
