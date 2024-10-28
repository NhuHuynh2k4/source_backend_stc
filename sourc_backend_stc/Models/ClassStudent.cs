namespace sourc_backend_stc.Models
{
    public class ClassStudent
    {
        public int ClassStudentID { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime UpdateDate { get; set; } = DateTime.Now;
        public bool IsDelete { get; set; } = false;
        public int ClassID { get; set; }
        public int StudentID { get; set; }
    }
}
