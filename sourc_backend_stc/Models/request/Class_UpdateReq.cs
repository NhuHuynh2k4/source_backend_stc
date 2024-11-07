namespace sourc_backend_stc.Models
{
    public class Class_UpdateReq
    {
        public int ClassID { get; set;}
        public string ClassCode { get; set; } // Không null
        public string ClassName { get; set; } // Không null
        public string Session { get; set; }
        public int? SubjectsID { get; set; } // Có thể null
    }
}
