using GamificationMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace GamificationMVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Question> Questions => Set<Question>();
    }
}