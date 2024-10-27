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
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // Lấy tất cả các sinh viên
        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllStudent()
        {
            var classes = await _studentService.GetAllStudent();
            return Ok(classes); // Trả về danh sách sinh viên với mã 200 OK
        }

        // Tạo mới Sinh viên
        [HttpPost("create")]
        public async Task<IActionResult> CreateStudent([FromBody] Student_CreateReq createReq)
        {
            if (createReq == null)
            {
                // Trả về mã 400 Bad Request nếu đầu vào không hợp lệ
                return BadRequest("Yêu cầu không hợp lệ.");
            }

            var isCreated = await _studentService.CreateStudent(createReq);

            if (isCreated)
            {
                // Trả về mã 201 Created nếu thành công
                return CreatedAtAction(nameof(CreateStudent), new { id = createReq.StudentCode }, "Sinh viên đã được tạo thành công.");
            }
            else
            {
                // Trả về mã 500 Internal Server Error nếu có lỗi xảy ra
                return StatusCode(StatusCodes.Status500InternalServerError, "Không thể tạo sinh viên.");
            }
        }


            // // Lấy Student theo ID
            [HttpGet("get-by-id/{studentId}")]
            public async Task<IActionResult> GetStudentById(int studentId)
            {
                if (studentId <= 0)
                {
                    // Trả về mã lỗi 400 nếu studentId không hợp lệ
                    return BadRequest("ID sinh viên không hợp lệ.");
                }

                var studentInfo = await _studentService.GetStudentById(studentId);

                if (studentInfo != null)
                {
                    // Trả về mã 200 OK và thông tin lớp học nếu tìm thấy
                    return Ok(studentInfo);
                }
                else
                {
                    // Trả về mã lỗi 404 nếu không tìm thấy lớp học
                    return NotFound("Không tìm thấy sinh viên với ID đã cho.");
                }
            }

            [HttpPut("update/{studentId}")]
            public async Task<IActionResult> UpdateClassUpdateStudent(int studentId, [FromBody] Student_UpdateReq updateReq)
            {
                if (updateReq == null || string.IsNullOrWhiteSpace(updateReq.StudentCode) 
                || string.IsNullOrWhiteSpace(updateReq.StudentName) 
                || string.IsNullOrWhiteSpace(updateReq.NumberPhone) 
                || string.IsNullOrWhiteSpace(updateReq.Email))
                {
                    return BadRequest("Dữ liệu cập nhật không hợp lệ.");
                }

                var isUpdated = await _studentService.UpdateStudent(studentId, updateReq);

                if (isUpdated)
                {
                    return Ok("Cập nhật sinh viên thành công.");
                }
                else
                {
                    return NotFound("Không tìm thấy sinh viên hoặc cập nhật thất bại.");
                }
            }


            [HttpDelete("delete/{studentId}")]
            public async Task<IActionResult> DeleteStudent(int studentId)
            {
                if (studentId <= 0)
                {
                    return BadRequest("ID sinh viên không hợp lệ.");
                }

                var isDeleted = await _studentService.DeleteStudent(studentId);

                if (isDeleted)
                {
                    return Ok("Đã xóa mềm sinh viên thành công.");
                }
                else
                {
                    return NotFound("Không tìm thấy sinh viên với ID đã cho.");
                }
            }

    }

}
