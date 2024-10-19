namespace sourc_backend_stc.Models
{
public class Answer
{
    public int AnswerID { get; set; }
    public string AnswerName { get; set; } // Không null
    public string AnswerTextContent { get; set; } // Không null
    public string AnswerImgContent { get; set; }
    public bool IsTrue { get; set; }
    public DateTime UpdateDate { get; set; } // Mặc định là thời gian hiện tại
    public DateTime CreateDate { get; set; } // Mặc định là thời gian hiện tại
    public bool IsDelete { get; set; }
    public int QuestionID { get; set; }
}}