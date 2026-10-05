using EduGameMVC.Models;
using EduGameMVC.Repositories;
using Microsoft.AspNetCore.Mvc;

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

        // READ list all of the questions
        public async Task<IActionResult> Index()
        {
            var questions = await _questionRepository.GetAllAsync();
            return View(questions);
        }

        // READ shows one question
        public async Task<IActionResult> Details(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id);

            if (question == null)
            {
                _logger.LogWarning(
                    "Question with id {QuestionId} was not found.",
                    id);

                return NotFound();
            }

            return View(question);
        }

        // CREATE show form
        public IActionResult Create()
        {
            return View();
        }

        // CREATE process form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Question question)
        {
            if (!ModelState.IsValid)
            {
                return View(question);
            }

            await _questionRepository.AddAsync(question);

            _logger.LogInformation(
                "Question {QuestionId} was created.",
                question.Id);

            return RedirectToAction(nameof(Index));
        }

        // UPDATE show edit form
        public async Task<IActionResult> Edit(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id);

            if (question == null)
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
        public async Task<IActionResult> Edit(int id, Question question)
        {
            if (id != question.Id)
            {
                return BadRequest();
            }

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
        public async Task<IActionResult> Delete(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id);

            if (question == null)
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            await _questionRepository.DeleteAsync(id);

            _logger.LogInformation(
                "Question {QuestionId} was deleted.",
                id);

            return RedirectToAction(nameof(Index));
        }
    }
}