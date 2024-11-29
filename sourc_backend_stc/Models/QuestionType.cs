namespace sourc_backend_stc.Models
{
    public class QuestionType
    {
        public int QuestionTypeID { get; set; }
        public string QuestionTypeCode { get; set; }
        public string QuestionTypeName { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime UpdateDate { get; set; } = DateTime.Now;
        public bool IsDelete { get; set; } = false;
    }
}