namespace sourc_backend_stc.Models
{
    public class Question
    {
        public int QuestionID { get; set; }
        public string QuestionCode { get; set; }
        public string QuestionName { get; set; }
        public string QuestionTextContent { get; set; }
        public string QuestionImgContent { get; set; }
        public int SubjectsID { get; set; }
        public Subject Subject { get; set; }
        public int QuestionTypeID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsDelete { get; set; }
    }
}