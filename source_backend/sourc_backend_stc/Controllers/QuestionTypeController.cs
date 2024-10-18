using Microsoft.AspNetCore.Mvc;
using sourc_backend_stc.Data;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class QuestionTypeController : ControllerBase
{
    private readonly AppDbContext _context;

    public QuestionTypeController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var questionTypes = _context.QuestionTypes.Where(q => !q.IsDelete).ToList();
        return Ok(questionTypes);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var questionType = _context.QuestionTypes.FirstOrDefault(q => q.QuestionTypeID == id && !q.IsDelete);
        if (questionType == null) return NotFound();
        return Ok(questionType);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] QuestionType questionType)
    {
        _context.QuestionTypes.Add(questionType);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = questionType.QuestionTypeID }, questionType);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] QuestionType updatedQuestionType)
    {
        var existingQuestionType = _context.QuestionTypes.FirstOrDefault(q => q.QuestionTypeID == id && !q.IsDelete);
        if (existingQuestionType == null) return NotFound();

        existingQuestionType.QuestionTypeCode = updatedQuestionType.QuestionTypeCode;
        existingQuestionType.QuestionTypeName = updatedQuestionType.QuestionTypeName;
        existingQuestionType.UpdateDate = DateTime.Now;

        _context.QuestionTypes.Update(existingQuestionType);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var questionType = _context.QuestionTypes.FirstOrDefault(q => q.QuestionTypeID == id && !q.IsDelete);
        if (questionType == null) return NotFound();

        questionType.IsDelete = true;
        _context.QuestionTypes.Update(questionType);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
