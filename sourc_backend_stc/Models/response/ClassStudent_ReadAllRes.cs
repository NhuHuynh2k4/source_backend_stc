namespace sourc_backend_stc.Models
{
    public class ClassStudent_ReadAllRes
    {
        public int Class_StudentID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int ClassID { get; set;}
        public string? ClassName { get; set; } // Không null
        public string? ClassCode { get; set; } // Không null
        public int StudentID { get; set; }
        public string? StudentCode { get; set; }
        public string? StudentName { get; set; }
    }
}
