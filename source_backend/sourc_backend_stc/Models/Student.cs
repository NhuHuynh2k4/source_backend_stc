namespace sourc_backend_stc.Models
{
  public class Student
{
    public int StudentID { get; set; }
    public string StudentCode { get; set; }
     public bool? Gender { get; set; }
    public string NumberPhone { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public DateTime BirthdayDate { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public bool IsDelete { get; set; }

    // Constructor để khởi tạo các giá trị không null
    public Student()
    {
        StudentCode = string.Empty;  // Khởi tạo chuỗi rỗng
        NumberPhone = string.Empty;  // Khởi tạo chuỗi rỗng
        Address = string.Empty;      // Khởi tạo chuỗi rỗng
        Email = string.Empty;        // Khởi tạo chuỗi rỗng
        BirthdayDate = DateTime.Now; // Khởi tạo với thời gian hiện tại
        CreateDate = DateTime.Now;   // Khởi tạo với thời gian hiện tại
        UpdateDate = DateTime.Now;   // Khởi tạo với thời gian hiện tại
        IsDelete = false;            // Mặc định không bị xóa
    }
}

}
