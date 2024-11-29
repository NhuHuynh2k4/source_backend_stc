namespace sourc_backend_stc.Models
{
    public class Mark_ReadAllRes
    {
        public int MarkID { get; set; }
        public float Result { get; set; }
        public float PassingScore { get; set; }
        public int StudentID { get; set; }
        public int ExamID { get; set; }
        public int TestID { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsDelete { get; set; }
    }
}