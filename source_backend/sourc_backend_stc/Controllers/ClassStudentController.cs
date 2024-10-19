using Microsoft.AspNetCore.Mvc;
using sourc_backend_stc.Data;
using sourc_backend_stc.Models;
using System.Linq;

namespace sourc_backend_stc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Class_StudentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Class_StudentController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Class_Student
        [HttpGet]
        public IActionResult GetAll()
        {
            var classStudents = _context.Class_Students.Where(cs => !cs.IsDelete).ToList();
            return Ok(classStudents);
        }

        // POST: api/Class_Student
        [HttpPost]
        public IActionResult Create(Class_Student classStudent)
        {
            _context.Class_Students.Add(classStudent);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetAll), new { id = classStudent.Class_StudentID }, classStudent);
        }

        // PUT: api/Class_Student/5
        [HttpPut("{id}")]
        public IActionResult UpdateClassStudent(int id, [FromBody] Class_Student classStudent)
        {
            var existingClassStudent = _context.Class_Students.Find(id);
            if (existingClassStudent == null || existingClassStudent.IsDelete)
            {
                return NotFound();
            }

            existingClassStudent.ClassID = classStudent.ClassID;
            existingClassStudent.StudentID = classStudent.StudentID;
            existingClassStudent.UpdateDate = DateTime.Now;

            _context.SaveChanges();
            return NoContent();
        }


        // DELETE: api/Class_Student/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var classStudent = _context.Class_Students.Find(id);
            if (classStudent == null || classStudent.IsDelete)
            {
                return NotFound();
            }

            classStudent.IsDelete = true;
            _context.SaveChanges();
            return NoContent();
        }
    }
}
