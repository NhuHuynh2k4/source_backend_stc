namespace sourc_backend_stc.Models
{
  public class Student
{
    public int StudentID { get; set; }
    public string StudentCode { get; set; }
    public string StudentName { get; set; }
     public bool? Gender { get; set; }
    public string NumberPhone { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
   public DateTime? BirthdayDate { get; set; }


    // Constructor để khởi tạo các giá trị không null
    public Student()
    {
        StudentCode = string.Empty;  // Khởi tạo chuỗi rỗng 
        StudentName = string.Empty;  // Khởi tạo chuỗi rỗng
        NumberPhone = string.Empty;  // Khởi tạo chuỗi rỗng
        Address = string.Empty;      // Khởi tạo chuỗi rỗng
        Email = string.Empty;        // Khởi tạo chuỗi rỗng

    }
}

}
