using Microsoft.EntityFrameworkCore;
using sourc_backend_stc.Models;

namespace sourc_backend_stc.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Định nghĩa các DbSet cho các thực thể trong ứng dụng
        public DbSet<Student> Students { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Question> Questions { get; set; }

        // Tùy chỉnh cấu hình Entity Framework thông qua phương thức OnModelCreating
    }
}
