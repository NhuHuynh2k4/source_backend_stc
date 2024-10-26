namespace sourc_backend_stc.Models
{
    public class Test_ReadAllRes
    {
        public int TestID { get; set; } // Primary Key
        public string? TestCode { get; set; } // Không null
        public string? TestName { get; set; } // Không null
        public int? NumberOfQuestions { get; set; } // Có thể null
        public DateTime? UpdateDate { get; set; } // Mặc định là thời gian hiện tại
        public DateTime? CreateDate { get; set; } // Mặc định là thời gian hiện tại
        public int? SubjectsID { get; set; } // Có thể null
    }
}
