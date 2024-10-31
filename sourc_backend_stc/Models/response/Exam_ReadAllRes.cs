namespace sourc_backend_stc.Models
{
    public class Exam_ReadAllRes
    {
        public int ExamID { get; set; }
        public string ExamCode { get; set; } 
        public string ExamName { get; set; }
        public DateTime ExamDate { get; set; }
        public int Duration { get; set; }
        public int NumberOfQuestions { get; set; }
        public int TotalMarks { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime CreateDate { get; set; } 
        public int TestID { get; set; } 
        public bool IsDelete { get; set; }
    }
}