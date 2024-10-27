using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using sourc_backend_stc.Models;
using Dapper;
using sourc_backend_stc.Services;
using sourc_backend_stc.Utils;

namespace sourc_backend_stc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Exam_StudentController : ControllerBase
    {
        private readonly IExam_StudentService _exam_studentService;

        public Exam_StudentController(IExam_StudentService exam_studentService)
        {
            _exam_studentService = exam_studentService;
        }

        // Lấy tất cả các exam_student
        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<Exam_StudentRes>>> GetAllExam_Student()
        {
            var classes = await _exam_studentService.GetAllExam_Student();
            return Ok(classes); // Trả về danh sách exam_student với mã 200 OK
        }

        // Tạo mới exam_student
        [HttpPost("create")]
        public async Task<IActionResult> CreateExam_Student([FromBody] Exam_Student_CreateReq createReq)
        {
            if (createReq == null || createReq.ExamID <= 0 || createReq.StudentID <= 0)
            {
                return BadRequest("Dữ liệu cập nhật không hợp lệ. Vui lòng kiểm tra ExamID và StudentID.");
            }

            var isCreated = await _exam_studentService.CreateExam_Student(createReq);

            if (isCreated)
            {
                // Trả về mã 201 Created nếu thành công
                return CreatedAtAction(nameof(CreateExam_Student), new { id = createReq.ExamID }, "ExamID đã được tạo thành công.");
            }
            else
            {
                // Trả về mã 500 Internal Server Error nếu có lỗi xảy ra
                return StatusCode(StatusCodes.Status500InternalServerError, "Không thể tạo ExamID.");
            }
        }


        // // Lấy Student theo ID
        [HttpGet("get-by-id/{exam_studentID}")]
        public async Task<IActionResult> GetStudentById(int Exam_StudentId)
        {
            if (Exam_StudentId <= 0)
            {
                // Trả về mã lỗi 400 nếu studentId không hợp lệ
                return BadRequest("ID Exam_Student không hợp lệ.");
            }

            var exam_studentInfo = await _exam_studentService.GetExam_StudentById(Exam_StudentId);

            if (exam_studentInfo != null)
            {
                // Trả về mã 200 OK và thông tin lớp học nếu tìm thấy
                return Ok(exam_studentInfo);
            }
            else
            {
                // Trả về mã lỗi 404 nếu không tìm thấy lớp học
                return NotFound("Không tìm thấy Exam_Student với ID đã cho.");
            }
        }

        [HttpPut("update/{exam_studentId}")]
        public async Task<IActionResult> UpdateExam_Student(int Exam_StudentId, [FromBody] Exam_Student_UpdateReq updateReq)
        {
            if (updateReq == null || updateReq.ExamID <= 0 || updateReq.StudentID <= 0)
            {
                return BadRequest("Dữ liệu cập nhật không hợp lệ. Vui lòng kiểm tra ExamID và StudentID.");
            }

            var isUpdated = await _exam_studentService.UpdateExam_Student(Exam_StudentId, updateReq);

            if (isUpdated)
            {
                return Ok("Cập nhật Exam_Student thành công.");
            }
            else
            {
                Console.WriteLine("Không tìm thấy Exam_Student hoặc cập nhật thất bại cho ID: " + Exam_StudentId);
                return NotFound("Không tìm thấy Exam_Student hoặc cập nhật thất bại.");
            }
        }



        [HttpDelete("delete/{exam_studentId}")]
        public async Task<IActionResult> DeleteStudent(int Exam_StudentId)
        {
            if (Exam_StudentId <= 0)
            {
                return BadRequest("ID Exam_Student không hợp lệ.");
            }

            var isDeleted = await _exam_studentService.DeleteExam_Student(Exam_StudentId);

            if (isDeleted)
            {
                return Ok("Đã xóa mềm Exam_Student thành công.");
            }
            else
            {
                return NotFound("Không tìm thấy Exam_Student với ID đã cho.");
            }
        }

    }

}
