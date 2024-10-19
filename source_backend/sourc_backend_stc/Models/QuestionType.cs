using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sourc_backend_stc.Models
{
    [Table("QuestionType")] // Chỉ định tên bảng chính xác
    public class QuestionType
    {
        [Key]
        public int QuestionTypeID { get; set; }
        public string QuestionTypeCode { get; set; }
        public string QuestionTypeName { get; set; }
        public DateTime UpdateDate { get; set; } = DateTime.Now;
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public bool IsDelete { get; set; } = false;
    }
}
