namespace sourc_backend_stc.Models
{
    public class Mark_CreateReq
    {
        public float Result { get; set; } 
        public float PassingScore { get; set; } 
        public int StudentID { get; set; }
        public int ExamID { get; set; }
        public int TestID { get; set; }
    }
}