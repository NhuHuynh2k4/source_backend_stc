namespace sourc_backend_stc.Models
{
    public class Exam_StudentRes
    {
        public int Exam_StudentID { get; set; }
        public int ExamID { get; set; }
        public string? ExamCode { get; set; } 
        public string? ExamName { get; set; }
        public int StudentID{ get; set; }
        public string? StudentCode {get; set;}
        public string? StudentName { get; set; }
        public DateTime CreateDate {get; set;}
        public DateTime UpdateDate {get; set;}
    }
}
