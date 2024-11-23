namespace sourc_backend_stc.Models
{
    public class Answer_CreateReq
    {
        public string AnswerName { get; set; }
        public string AnswerTextContent { get; set; }
        public string AnswerImgContent { get; set; }
        public bool IsTrue{ get; set;}
        public int QuestionID { get; set; }
    }
}
