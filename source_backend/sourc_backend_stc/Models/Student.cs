namespace sourc_backend_stc.Models
{
    public class Student
    {
        public int StudentID { get; set; }
        public string StudentCode { get; set; }
        public bool Gender{ get; set;}
        public string NumberPhone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public DateTime BirthdayDate {get; set;}
        public DateTime CreateDate {get; set;}
        public DateTime UpdateDate {get; set;}
        public bool IsDelete{ get; set;}
        //public decimal Price { get; set; }
    }
}
