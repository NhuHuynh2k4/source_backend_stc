namespace sourc_backend_stc.Models
{
    public class Subject
    {
        public int SubjectsID { get; set; }
        public string SubjectsCode { get; set; }
        public string SubjectsName { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsDelete { get; set; }
    }
}
