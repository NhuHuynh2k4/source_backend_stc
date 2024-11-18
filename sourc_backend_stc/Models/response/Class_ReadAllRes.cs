namespace sourc_backend_stc.Models
{
    public class Class_ReadAllRes
    {
        public int ClassID { get; set; } // Primary Key
        public string? ClassCode { get; set; } // Không null
        public string? ClassName { get; set; } // Không null
        public string? Session { get; set; }
        public DateTime UpdateDate { get; set; } // Mặc định là thời gian hiện tại
        public DateTime CreateDate { get; set; } // Mặc định là thời gian hiện tại
        public int? SubjectsID { get; set; } // Có thể null
        public string? SubjectsName { get; set; }
    }
}
