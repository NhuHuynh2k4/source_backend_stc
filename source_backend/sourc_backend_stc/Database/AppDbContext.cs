using Microsoft.EntityFrameworkCore;
using sourc_backend_stc.Models;

namespace sourc_backend_stc.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<ClassStudent> ClassStudents { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClassStudent>()
                .HasKey(cs => cs.Class_StudentID); // Định nghĩa khóa chính
            // Ánh xạ tên bảng ClassStudent thành Class_Student
            modelBuilder.Entity<ClassStudent>().ToTable("Class_Student");

            // Ánh xạ cột Class_StudentID với cột trong cơ sở dữ liệu
            modelBuilder.Entity<ClassStudent>()
                .Property(cs => cs.Class_StudentID)
                .HasColumnName("Class_StudentID");
            
            modelBuilder.Entity<QuestionType>()
                .ToTable("QuestionType"); // Cấu hình tên bảng tại đây
        }
        public DbSet<QuestionType> QuestionType { get; set; }

        public DbSet<Student> Student{ get; set; }
    }
}
