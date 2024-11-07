namespace sourc_backend_stc.Models
{
    public class ClassStudent_CreateReq
    {
        public int Class_StudentID { get; set; } // Khóa chính
        public DateTime CreateDate { get; set; } = DateTime.Now; // Mặc định là ngày hiện tại
        public DateTime UpdateDate { get; set; } = DateTime.Now; // Mặc định là ngày hiện tại
        public bool IsDelete { get; set; } = false; // Đánh dấu bản ghi bị xóa
        public int ClassID { get; set; }
        public int StudentID { get; set; }
    }
}
