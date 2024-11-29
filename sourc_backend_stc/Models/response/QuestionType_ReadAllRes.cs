namespace sourc_backend_stc.Models
{
    public class QuestionTypeResponse
    {
        public int QuestionTypeID { get; set; }
        public string QuestionTypeCode { get; set; }
        public string QuestionTypeName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
