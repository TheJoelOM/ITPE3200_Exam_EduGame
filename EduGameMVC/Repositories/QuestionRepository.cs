using EduGameMVC.Data;
using EduGameMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace EduGameMVC.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApplicationDbContext _context;

        public QuestionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Question>> GetAllAsync()
        {
            return await _context.Questions.ToListAsync();
        }

        public async Task<Question?> GetByIdAsync(int id)
        {
            return await _context.Questions.FindAsync(id);
        }

        public async Task<Question?> GetRandomForAdventureAsync(string subject, string? ownerId, bool shared)
        {
            var query = _context.Questions.Where(q => q.Subject == subject);
            query = shared ? query.Where(q => q.OwnerId == null) : query.Where(q => q.OwnerId == ownerId);
            var questions = await query.ToListAsync();
            return questions.Count == 0 ? null : questions[Random.Shared.Next(questions.Count)];
        }

        public Task<List<string>> GetSubjectsForOwnerAsync(string ownerId) =>
            _context.Questions.Where(q => q.OwnerId == ownerId).Select(q => q.Subject).Distinct().OrderBy(s => s).ToListAsync();

        public async Task AddAsync(Question question)
        {
            await _context.Questions.AddAsync(question);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var question = await _context.Questions.FindAsync(id);

            if (question == null)
            {
                return;
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }
    }
}
