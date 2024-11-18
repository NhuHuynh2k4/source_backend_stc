namespace sourc_backend_stc.Models
{
    public class QuestionReadAllRes
    {
        public int QuestionID { get; set; }
        public string? QuestionCode { get; set; }
        public string? QuestionName { get; set; }
        public string? QuestionTextContent { get; set; }
        public string? QuestionImgContent { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime CreateDate { get; set; }
        public int SubjectsID { get; set; }
        public string? SubjectsCode { get; set; }
        public string? SubjectsName { get; set; }
        public int QuestionTypeID { get; set; }
        public string? QuestionTypeCode { get; set; }
        public string? QuestionTypeName { get; set; }
    }
}
