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
    [Authorize]
    public class TestController : ControllerBase
    {
        private readonly ITestService _testService;

        public TestController(ITestService testService)
        {
            _testService = testService;
        }

        // Lấy tất cả các lớp học
        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<Test_ReadAllRes>>> GetAllTestes()
        {
            var classes = await _testService.GetAllTests();
            return Ok(classes); // Trả về danh sách lớp học với mã 200 OK
        }

        // Tạo mới Test
        [HttpPost("create")]
        public async Task<IActionResult> CreateTest([FromBody] Test_CreateReq classDto)
        {
            if (classDto == null)
            {
                // Trả về mã 400 Bad Request nếu đầu vào không hợp lệ
                return BadRequest("Yêu cầu không hợp lệ.");
            }

            var isCreated = await _testService.CreateTest(classDto);

            if (isCreated)
            {
                // Trả về mã 201 Created nếu thành công
                return CreatedAtAction(nameof(CreateTest), new { id = classDto.TestCode }, "Lớp học đã được tạo thành công.");
            }
            else
            {
                // Trả về mã 500 Internal Server Error nếu có lỗi xảy ra
                return StatusCode(StatusCodes.Status500InternalServerError, "Không thể tạo lớp học.");
            }
        }


        // // Lấy Test theo ID
        [HttpGet("get-by-id/{classId}")]
        public async Task<IActionResult> GetTestById(int classId)
        {
            if (classId <= 0)
            {
                // Trả về mã lỗi 400 nếu classId không hợp lệ
                return BadRequest("ID lớp học không hợp lệ.");
            }

            var classInfo = await _testService.GetTestById(classId);

            if (classInfo != null)
            {
                // Trả về mã 200 OK và thông tin lớp học nếu tìm thấy
                return Ok(classInfo);
            }
            else
            {
                // Trả về mã lỗi 404 nếu không tìm thấy lớp học
                return NotFound("Không tìm thấy lớp học với ID đã cho.");
            }
        }

        [HttpPut("update/{classId}")]
        public async Task<IActionResult> UpdateTest(int classId, [FromBody] Test_UpdateReq updateReq)
        {
            if (updateReq == null || string.IsNullOrWhiteSpace(updateReq.TestCode) || string.IsNullOrWhiteSpace(updateReq.TestName))
            {
                return BadRequest("Dữ liệu cập nhật không hợp lệ.");
            }

            var isUpdated = await _testService.UpdateTest(classId, updateReq);

            if (isUpdated)
            {
                return Ok("Cập nhật lớp học thành công.");
            }
            else
            {
                return NotFound("Không tìm thấy lớp học hoặc cập nhật thất bại.");
            }
        }


        [HttpDelete("delete/{classId}")]
        public async Task<IActionResult> SoftDeleteTest(int classId)
        {
            if (classId <= 0)
            {
                return BadRequest("ID lớp học không hợp lệ.");
            }

            var isDeleted = await _testService.DeleteTest(classId);

            if (isDeleted)
            {
                return Ok("Đã xóa mềm lớp học thành công.");
            }
            else
            {
                return NotFound("Không tìm thấy lớp học với ID đã cho.");
            }
        }

    }

}
