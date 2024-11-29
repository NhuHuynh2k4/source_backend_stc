namespace sourc_backend_stc.Models
{
    public class ClassStudent_CreateReq
    {
        public DateTime CreateDate { get; set; } = DateTime.Now; // Mặc định là ngày hiện tại
        public DateTime UpdateDate { get; set; } = DateTime.Now; // Mặc định là ngày hiện tại
        public int ClassID { get; set; }
        public int StudentID { get; set; }

        
    }
}
