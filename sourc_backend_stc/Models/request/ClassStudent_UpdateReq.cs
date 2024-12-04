namespace sourc_backend_stc.Models
{
    public class ClassStudent_UpdateReq
    {
        public int Class_StudentID { get; set; } // Khóa chính
        public int ClassID { get; set; }
        public int StudentID { get; set; }
    }
}
