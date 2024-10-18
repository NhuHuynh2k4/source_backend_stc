using Microsoft.EntityFrameworkCore;
using sourc_backend_stc.Models;

namespace sourc_backend_stc.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
         public DbSet<QuestionType> QuestionTypes { get; set; }
        public DbSet<ClassStudent> ClassStudents { get; set; }
    }
}
