using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sourc_backend_stc.Data;
using sourc_backend_stc.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace sourc_backend_stc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly AppDbContext _context;  // Đổi từ ApplicationDbContext thành AppDbContext

        public SubjectController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/subject
        [HttpGet]
        public async Task<IActionResult> GetAllSubjects()
        {
            var subjects = await _context.Subjects
                .Where(s => !s.IsDelete)
                .ToListAsync();
            return Ok(subjects);
        }

        // GET: api/subject/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubjectById(int id)
        {
            var subject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.SubjectsID == id && !s.IsDelete);

            if (subject == null)
            {
                return NotFound();
            }

            return Ok(subject);
        }

        // POST: api/subject
        [HttpPost]
        public async Task<IActionResult> CreateSubject(Subject subject)
        {
            subject.CreateDate = DateTime.Now;
            subject.UpdateDate = DateTime.Now;
            subject.IsDelete = false;

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSubjectById), new { id = subject.SubjectsID }, subject);
        }

        // PUT: api/subject/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(int id, Subject updatedSubject)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null || subject.IsDelete)
            {
                return NotFound();
            }

            subject.SubjectsCode = updatedSubject.SubjectsCode;
            subject.SubjectsName = updatedSubject.SubjectsName;
            subject.UpdateDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/subject/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null || subject.IsDelete)
            {
                return NotFound();
            }

            subject.IsDelete = true;
            subject.UpdateDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
