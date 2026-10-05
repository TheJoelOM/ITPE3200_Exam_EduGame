using EduGameMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace EduGameMVC.Data
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