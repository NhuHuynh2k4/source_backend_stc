using Microsoft.AspNetCore.Mvc;
using sourc_backend_stc.Data;
using sourc_backend_stc.Models;
using System.Linq;

namespace sourc_backend_stc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionTypeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuestionTypeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/QuestionType
        [HttpGet]
        public IActionResult GetAll()
        {
            var questionTypes = _context.QuestionTypes.Where(qt => !qt.IsDelete).ToList();
            return Ok(questionTypes);
        }

        // POST: api/QuestionType
        [HttpPost]
        public IActionResult Create(QuestionType questionType)
        {
            _context.QuestionTypes.Add(questionType);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetAll), new { id = questionType.QuestionTypeID }, questionType);
        }

        // PUT: api/QuestionType/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, QuestionType questionType)
        {
            var existingQuestionType = _context.QuestionTypes.Find(id);
            if (existingQuestionType == null || existingQuestionType.IsDelete)
            {
                return NotFound();
            }

            existingQuestionType.QuestionTypeCode = questionType.QuestionTypeCode;
            existingQuestionType.QuestionTypeName = questionType.QuestionTypeName;
            existingQuestionType.UpdateDate = DateTime.Now;

            _context.SaveChanges();
            return NoContent();
        }

        // DELETE: api/QuestionType/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var questionType = _context.QuestionTypes.Find(id);
            if (questionType == null || questionType.IsDelete)
            {
                return NotFound();
            }

            questionType.IsDelete = true;
            _context.SaveChanges();
            return NoContent();
        }
    }
}
