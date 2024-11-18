namespace sourc_backend_stc.Models
{
    public class TestQuestion_ReadAllRes 
    {
        public int Test_QuestionID { get; set;}
        // public DateTime CreateDate {get; set;}
        public int QuestionID { get; set; }
        public string? QuestionCode { get; set; }
        public string? QuestionName { get; set; }
        public int TestID { get; set; }
        public string? TestCode { get; set; } // Không null
        public string? TestName { get; set; } // Không null
    }
}