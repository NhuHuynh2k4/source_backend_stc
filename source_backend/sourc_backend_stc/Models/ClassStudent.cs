using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ClassStudent
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ClassStudentID { get; set; }

    public DateTime CreateDate { get; set; } = DateTime.Now;

    public DateTime UpdateDate { get; set; } = DateTime.Now;

    public bool IsDelete { get; set; } = false;

    [Required]
    public int ClassID { get; set; }

    [Required]
    public int StudentID { get; set; }
}