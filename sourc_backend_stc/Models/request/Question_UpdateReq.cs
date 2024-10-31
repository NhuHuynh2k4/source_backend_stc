namespace sourc_backend_stc.Models
{
    public class Question_UpdateReq
    {
        public string QuestionCode { get; set; }
        public string QuestionName { get; set; }
        public string QuestionTextContent { get; set; }
        public string QuestionImgContent { get; set; }
        public int SubjectsID { get; set; }
        public int QuestionTypeID { get; set; }
    }
}
