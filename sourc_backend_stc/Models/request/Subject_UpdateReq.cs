namespace sourc_backend_stc.Models
{
    public class Subject_UpdateReq
    {
        public int SubjectsID { get; set; }       // Thêm ID của Subject
        public string? SubjectsCode { get; set; }
        public string? SubjectsName { get; set; }
    }
}
