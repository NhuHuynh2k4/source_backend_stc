using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using sourc_backend_stc.Models;
using Dapper;
using sourc_backend_stc.Services;
using sourc_backend_stc.Utils;
using Microsoft.AspNetCore.Authorization;
using static sourc_backend_stc.Services.StudentService;

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
        // [Authorize]
        public async Task<ActionResult<IEnumerable<Student_ReadAllRes>>> GetAllStudent()
        {
            var classes = await _studentService.GetAllStudent();
            return Ok(classes); // Trả về danh sách sinh viên với mã 200 OK
        }

        // Tạo mới Sinh viên
        [HttpPost("create")]
        public async Task<IActionResult> CreateStudent([FromBody] Student_CreateReq createReq)
        {
            // Kiểm tra xem yêu cầu có hợp lệ hay không
            if (createReq == null)
            {
                return BadRequest("Yêu cầu không hợp lệ.");
            }

            // Gọi service để tạo sinh viên
            var createResult = await _studentService.CreateStudent(createReq);

            // Dựa trên kết quả trả về từ service, trả về phản hồi phù hợp
            switch (createResult)
            {
                case CreateStudentResult.Success:
                    return CreatedAtAction(nameof(CreateStudent), new { id = createReq.StudentCode }, "Sinh viên đã được tạo thành công.");

                case CreateStudentResult.DuplicateStudentCode:
                    return Conflict("Mã sinh viên đã tồn tại trong hệ thống.");

                case CreateStudentResult.DuplicateEmail:
                    return Conflict("Email đã tồn tại trong hệ thống.");
                case CreateStudentResult.DuplicateBirthdayDate:
                    return BadRequest("Ngày sinh không hợp lệ. ");

                case CreateStudentResult.InvalidInput:
                    return BadRequest("Dữ liệu đầu vào không hợp lệ. ");

                case CreateStudentResult.Error:
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, "Không thể tạo sinh viên. Đã xảy ra lỗi hệ thống.");
            }
        }





        // // Lấy Student theo ID
        [HttpGet("get-by-id/{studentId}")]
        // [Authorize]
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

        [HttpPut("update")]
        // [Authorize]
        public async Task<IActionResult> UpdateClassUpdateStudent([FromBody] Student_UpdateReq updateReq)
        {
            // Kiểm tra xem yêu cầu có hợp lệ hay không
            if (updateReq == null)
            {
                return BadRequest("Yêu cầu không hợp lệ.");
            }

            // Gọi service để cập nhật sinh viên
            var updateResult = await _studentService.UpdateStudent(updateReq);

            // Dựa trên kết quả trả về từ service, trả về phản hồi phù hợp
            switch (updateResult)
            {
                case UpdateStudentResult.Success:
                    return Ok("Cập nhật sinh viên thành công.");

                case UpdateStudentResult.StudentNotFound:
                    return NotFound("Không tìm thấy sinh viên.");

                case UpdateStudentResult.DuplicateStudentCode:
                    return Conflict("Mã sinh viên đã tồn tại trong hệ thống.");

                case UpdateStudentResult.DuplicateEmail:
                    return Conflict("Email đã tồn tại trong hệ thống.");

                case UpdateStudentResult.InvalidInput:
                    return BadRequest("Dữ liệu đầu vào không hợp lệ.");

                case UpdateStudentResult.Error:
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, "Không thể cập nhật sinh viên. Đã xảy ra lỗi hệ thống.");
            }
        }



        [HttpDelete("delete/{studentId}")]
        [Authorize]
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
