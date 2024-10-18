using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sourc_backend_stc.Data;
using sourc_backend_stc.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace sourc_backend_stc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuestionsController(AppDbContext context) // Thay đổi ApplicationDbContext thành AppDbContext
        {
            _context = context;
        }

        // GET: api/questions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Question>>> GetQuestions()
        {
            return await _context.Questions
                .Where(q => q.IsDelete == false)
                .ToListAsync();
        }

        // GET: api/questions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Question>> GetQuestion(int id)
        {
            var question = await _context.Questions.FindAsync(id);

            if (question == null || question.IsDelete)
            {
                return NotFound();
            }

            return question;
        }

        // POST: api/questions
        [HttpPost]
        public async Task<ActionResult<Question>> CreateQuestion(Question question)
        {
            question.CreateDate = DateTime.Now;
            question.UpdateDate = DateTime.Now;
            question.IsDelete = false;

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuestion), new { id = question.QuestionID }, question);
        }

        // PUT: api/questions/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, Question question)
        {
            if (id != question.QuestionID)
            {
                return BadRequest();
            }

            var existingQuestion = await _context.Questions.FindAsync(id);

            if (existingQuestion == null || existingQuestion.IsDelete)
            {
                return NotFound();
            }

            existingQuestion.QuestionCode = question.QuestionCode;
            existingQuestion.QuestionName = question.QuestionName;
            existingQuestion.QuestionTextContent = question.QuestionTextContent;
            existingQuestion.QuestionImgContent = question.QuestionImgContent;
            existingQuestion.SubjectsID = question.SubjectsID;
            existingQuestion.QuestionTypeID = question.QuestionTypeID;
            existingQuestion.UpdateDate = DateTime.Now;

            _context.Entry(existingQuestion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/questions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _context.Questions.FindAsync(id);

            if (question == null || question.IsDelete)
            {
                return NotFound();
            }

            // Soft delete
            question.IsDelete = true;
            question.UpdateDate = DateTime.Now;

            _context.Entry(question).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.QuestionID == id && e.IsDelete == false);
        }
    }
}
