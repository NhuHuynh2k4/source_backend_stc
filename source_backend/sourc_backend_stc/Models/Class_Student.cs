using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sourc_backend_stc.Models
{
    [Table("Class_Student")] // Chỉ định tên bảng chính xác
    public class Class_Student
    {
        [Key]
        public int Class_StudentID { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime UpdateDate { get; set; } = DateTime.Now;
        public bool IsDelete { get; set; } = false;
        public int ClassID { get; set; }
        public int StudentID { get; set; }
    }
}
