namespace sourc_backend_stc.Models
{
    public class Mark_ReadAllRes
    {
        public int MarkID { get; set; }
        public float Result { get; set; }
        public float PassingScore { get; set; }
        public int StudentID { get; set; }
        public string? StudentCode { get; set; }
        public string? StudentName { get; set; }
        public int ExamID { get; set; }
        public string? ExamCode { get; set; } 
        public string? ExamName { get; set; }
        public int TestID { get; set; }
        public string? TestCode { get; set; } // Không null
        public string? TestName { get; set; } // Không null
        public DateTime UpdateDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}