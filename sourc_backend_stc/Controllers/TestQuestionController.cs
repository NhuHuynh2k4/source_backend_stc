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
    public class TestQuestionController : ControllerBase
    {
        private readonly ITestQuestionService _testQuestionService;

        public TestQuestionController(ITestQuestionService testQuestionService)
        {
            _testQuestionService = testQuestionService;
        }

        // Lấy tất cả các TestQuestion
        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<TestQuestion_ReadAllRes>>> GetAllTestQuestions()
        {
            var testQuestions = await _testQuestionService.GetAllTestQuestions();
            return Ok(testQuestions); // Trả về danh sách TestQuestion với mã 200 OK
        }

        // Tạo mới TestQuestion
        [HttpPost("create")]
        public async Task<IActionResult> CreateTestQuestion([FromBody] TestQuestion_CreateReq testQuestionDto)
        {
            if (testQuestionDto == null)
            {
                // Trả về mã 400 Bad Request nếu đầu vào không hợp lệ
                return BadRequest(new {status = 400, message = "Yêu cầu không hợp lệ."} );
            }

            var isCreated = await _testQuestionService.CreateTestQuestion(testQuestionDto);

            if (isCreated)
            {
                // Trả về mã 201 Created nếu thành công
                return CreatedAtAction(nameof(CreateTestQuestion), new { id = testQuestionDto.QuestionID }, new {status = 201, message = "TestQuestion đã được tạo thành công.", data = testQuestionDto});
            }
            else
            {
                // Trả về mã 500 Internal Server Error nếu có lỗi xảy ra
                return StatusCode(StatusCodes.Status500InternalServerError, new {status = 500, message = "Không thể tạo TestQuestion."});
            }
        }


        [HttpGet("getByID/{testQuestionId}")]
        public async Task<IActionResult> GetTestQuestionById(int testQuestionId)
        {
            if (testQuestionId <= 0)
            {
                // Trả về mã lỗi 400 nếu testQuestionId không hợp lệ
                return BadRequest(new {status = 400, message = "ID TestQuestion không hợp lệ."});
            }

            var TestQuestionInfo = await _testQuestionService.GetTestQuestionById(testQuestionId);

            if (TestQuestionInfo != null)
            {
                // Trả về mã 200 OK và thông tin TestQuestion nếu tìm thấy
                return Ok(TestQuestionInfo);
            }
            else
            {
                // Trả về mã lỗi 404 nếu không tìm thấy TestQuestion
                return NotFound( new {status = 404, message = "Không tìm thấy TestQuestion với ID đã cho."});
            }
        }

        [HttpPut("update/{testQuestionId}")]
        public async Task<IActionResult> UpdateTestQuestion(int testQuestionId, [FromBody] TestQuestion_UpdateReq updateReq)
        {
            if (updateReq == null || updateReq.TestID <=0 || updateReq.QuestionID<=0)
            {
                return BadRequest(new {status = 400, message = "Dữ liệu cập nhật không hợp lệ."});
            }

            var isUpdated = await _testQuestionService.UpdateTestQuestion(testQuestionId, updateReq);

            if (isUpdated)
            {
                return Ok(new {status = 200 ,message = "Cập nhật TestQuestion thành công.", data = updateReq});
            }
            else
            {
                return NotFound( new {status = 404, message = "Không tìm thấy TestQuestion hoặc cập nhật thất bại."});
            }
        }


        [HttpDelete("delete/{testQuestionId}")]
        public async Task<IActionResult> SoftDeleteTestQuestion(int testQuestionId)
        {
            if (testQuestionId <= 0)
            {
                return BadRequest( new {status = 400, message = "ID TestQuestion không hợp lệ."});
            }

            var isDeleted = await _testQuestionService.DeleteTestQuestion(testQuestionId);

            if (isDeleted)
            {
                return Ok( new {status = 200, message = "Đã xóa mềm TestQuestion thành công."});
            }
            else
            {
                return NotFound( new {status = 404 ,message = "Không tìm thấy TestQuestion với ID đã cho."});
            }
        }

    }

}
