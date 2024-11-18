namespace sourc_backend_stc.Models
{
    public class Student_UpdateReq
    {
        public int StudentID { get; set; }
        public string? StudentCode { get; set; }
        public string? StudentName { get; set; }
        public bool Gender{ get; set;}
        public string? NumberPhone { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public DateTime BirthdayDate { get; set; }
    }
}
