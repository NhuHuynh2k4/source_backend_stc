using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class QuestionType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int QuestionTypeID { get; set; }

    [Required]
    [StringLength(50)]
    public string QuestionTypeCode { get; set; }

    [Required]
    [StringLength(100)]
    public string QuestionTypeName { get; set; }

    public DateTime CreateDate { get; set; } = DateTime.Now;

    public DateTime UpdateDate { get; set; } = DateTime.Now;

    public bool IsDelete { get; set; } = false;
}