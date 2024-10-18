using Microsoft.AspNetCore.Mvc;
using sourc_backend_stc.Data;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ClassStudentController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClassStudentController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var classStudents = _context.ClassStudents.Where(cs => !cs.IsDelete).ToList();
        return Ok(classStudents);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var classStudent = _context.ClassStudents.FirstOrDefault(cs => cs.ClassStudentID == id && !cs.IsDelete);
        if (classStudent == null) return NotFound();
        return Ok(classStudent);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ClassStudent classStudent)
    {
        _context.ClassStudents.Add(classStudent);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = classStudent.ClassStudentID }, classStudent);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ClassStudent updatedClassStudent)
    {
        var existingClassStudent = _context.ClassStudents.FirstOrDefault(cs => cs.ClassStudentID == id && !cs.IsDelete);
        if (existingClassStudent == null) return NotFound();

        existingClassStudent.ClassID = updatedClassStudent.ClassID;
        existingClassStudent.StudentID = updatedClassStudent.StudentID;
        existingClassStudent.UpdateDate = DateTime.Now;

        _context.ClassStudents.Update(existingClassStudent);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var classStudent = _context.ClassStudents.FirstOrDefault(cs => cs.ClassStudentID == id && !cs.IsDelete);
        if (classStudent == null) return NotFound();

        classStudent.IsDelete = true;
        _context.ClassStudents.Update(classStudent);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
