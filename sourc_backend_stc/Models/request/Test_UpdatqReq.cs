namespace sourc_backend_stc.Models
{
    public class Test_UpdateReq
    {
        public string? TestCode { get; set; } // Không null
        public string? TestName { get; set; } // Không null
        public int? NumberOfQuestions { get; set; } // Có thể null
        public int? SubjectsID { get; set; } // Có thể null
    }
}
