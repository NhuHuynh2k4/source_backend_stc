namespace sourc_backend_stc.Models
{
    public class Answer
    {
        public int AnswerID { get; set; }
        public string AnswerName { get; set; }
        public string AnswerTextContent { get; set; }
        public string AnswerImgContent { get; set; }
        public bool IsTrue{ get; set;}
        public DateTime CreateDate {get; set;}
        public DateTime UpdateDate {get; set;}
        public bool IsDelete{ get; set;}
        public int QuestionID { get; set; }
    }
}
