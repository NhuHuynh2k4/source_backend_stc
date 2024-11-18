using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using sourc_backend_stc.Models;
using Dapper;
using sourc_backend_stc.Services;
using sourc_backend_stc.Utils;
using Microsoft.AspNetCore.Authorization;

namespace sourc_backend_stc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;

        public ClassController(IClassService classService)
        {
            _classService = classService;
        }

        // Lấy tất cả các lớp học
        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<Class_ReadAllRes>>> GetAllClasses()
        {
            var classes = await _classService.GetAllClasses();
            return Ok(classes); // Trả về danh sách lớp học với mã 200 OK
        }

        // Tạo mới Class
        [HttpPost("create")]
        public async Task<IActionResult> CreateClass([FromBody] Class_CreateReq classDto)
        {
            if (classDto == null)
            {
                // Trả về mã 400 Bad Request nếu đầu vào không hợp lệ
                return BadRequest(new {messgase="Yêu cầu không hợp lệ."});
            }

            var isCreated = await _classService.CreateClass(classDto);

            if (isCreated)
            {
                // Trả về mã 201 Created nếu thành công
                return CreatedAtAction(nameof(CreateClass), new { id = classDto.ClassCode }, new {messgase="Lớp học đã được tạo thành công."});
            }
            else
            {
                // Trả về mã 500 Internal Server Error nếu có lỗi xảy ra
                return StatusCode(StatusCodes.Status500InternalServerError, "Không thể tạo lớp học.");
            }
        }


        [HttpGet("get-by-id/{classId}")]
        public async Task<IActionResult> GetClassById(int classId)
        {
            if (classId <= 0)
            {
                // Trả về mã lỗi 400 nếu classId không hợp lệ
                return BadRequest(new {messgase="ID lớp học không hợp lệ."});
            }

            var classInfo = await _classService.GetClassById(classId);

            if (classInfo != null)
            {
                // Trả về mã 200 OK và thông tin lớp học nếu tìm thấy
                return Ok(classInfo);
            }
            else
            {
                // Trả về mã lỗi 404 nếu không tìm thấy lớp học
                return NotFound(new {messgase="Không tìm thấy lớp học với ID đã cho."});
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateClass([FromBody] Class_UpdateReq updateReq)
        {
            if (updateReq == null || string.IsNullOrWhiteSpace(updateReq.ClassCode) || string.IsNullOrWhiteSpace(updateReq.ClassName))
            {
                return BadRequest(new {messgase="Dữ liệu cập nhật không hợp lệ."});
            }

            var isUpdated = await _classService.UpdateClass(updateReq);

            if (isUpdated)
            {
                return Ok(new {messgase="Cập nhật lớp học thành công."});
            }
            else
            {
                return NotFound(new {messgase="Không tìm thấy lớp học hoặc cập nhật thất bại."});
            }
        }


        [HttpDelete("delete/{classId}")]
        public async Task<IActionResult> SoftDeleteClass(int classId)
        {
            if (classId <= 0)
            {
                return BadRequest(new {messgase= "ID lớp học không hợp lệ."});
            }

            var isDeleted = await _classService.DeleteClass(classId);

            if (isDeleted)
            {
                return Ok(new {messgase="Đã xóa mềm lớp học thành công."});
            }
            else
            {
                return NotFound(new {messgase="Không tìm thấy lớp học với ID đã cho."});
            }
        }

    }

}
