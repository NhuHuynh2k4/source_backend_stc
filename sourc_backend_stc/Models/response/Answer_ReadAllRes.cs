namespace sourc_backend_stc.Models
{
    public class Answer_ReadAllRes
    {
        public int AnswerID { get; set; }
        public string AnswerName { get; set; }
        public string AnswerTextContent { get; set; }
        public string AnswerImgContent { get; set; }
        public bool IsTrue{ get; set;}
        public int QuestionID { get; set; }
        public string QuestionName{get; set;}
    }
}