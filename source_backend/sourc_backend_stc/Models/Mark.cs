namespace sourc_backend_stc.Models
{
    public class Mark
    {
        public int MarkID { get; set; } // Primary Key
        public float Result { get; set; } // Không null
        public float PassingScore { get; set; } // Không null
        public int StudentID { get; set; } // Không null
        public int ExamID { get; set; } // Không null (khóa ngoại đến bảng Exam)
        public int TestID { get; set; } // Không null (khóa ngoại đến bảng Test)
        public DateTime UpdateDate { get; set; } = DateTime.Now; // Mặc định là thời gian hiện tại
        public DateTime CreateDate { get; set; } = DateTime.Now; // Mặc định là thời gian hiện tại
        public bool IsDelete { get; set; } = false; // Mặc định là false
    }
}
