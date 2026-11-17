using EduGameMVC.Models;

namespace EduGameMVC.Repositories
{
    public interface IQuestionRepository
    {
        Task<List<Question>> GetAllAsync();
        Task<Question?> GetByIdAsync(int id);
        Task<Question?> GetRandomForAdventureAsync(string subject, string? ownerId, bool shared);
        Task<List<string>> GetSubjectsForOwnerAsync(string ownerId);
        Task AddAsync(Question question);
        Task UpdateAsync(Question question);
        Task DeleteAsync(int id);
    }
}
