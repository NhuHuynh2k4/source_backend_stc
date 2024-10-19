namespace sourc_backend_stc.Models
{
    public class Exam
    {
        public int ExamID { get; set; } // Primary Key
        public string ExamCode { get; set; } // Không null
        public string ExamName { get; set; } // Không null
        public DateTime ExamDate { get; set; } // Không null
        public int Duration { get; set; } // Không null
        public int NumberOfQuestions { get; set; } // Không null
        public int TotalMarks { get; set; } // Không null
        public DateTime UpdateDate { get; set; } = DateTime.Now; // Mặc định là thời gian hiện tại
        public DateTime CreateDate { get; set; } = DateTime.Now; // Mặc định là thời gian hiện tại
        public bool IsDelete { get; set; } = false; // Mặc định là false
        public int TestID { get; set; } // Không null (khóa ngoại đến bảng Test)
    }
}
