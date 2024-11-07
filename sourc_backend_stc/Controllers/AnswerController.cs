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
    public class AnswerController : ControllerBase
    {
        private readonly IAnswerService _answerService;

        public AnswerController(IAnswerService answerService)
        {
            _answerService = answerService;
        }

        // Lấy tất cả các answer
        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<Answer_ReadAllRes>>> GetAllAnswers()
        {
            var answers = await _answerService.GetAllAnswers();
            return Ok(answers); // Trả về danh sách answer với mã 200 OK
        }

        // Tạo mới Answer
        [HttpPost("create")]
        public async Task<IActionResult> CreateAnswer([FromBody] Answer_CreateReq answerDto)
        {
            if (answerDto == null)
            {
                // Trả về mã 400 Bad Request nếu đầu vào không hợp lệ
                return BadRequest(new {status = 400, message = "Yêu cầu không hợp lệ."} );
            }

            var isCreated = await _answerService.CreateAnswer(answerDto);
            Console.WriteLine(isCreated);
            if (isCreated)
            {
                // Trả về mã 201 Created nếu thành công
                return CreatedAtAction(nameof(CreateAnswer), new { id = answerDto.AnswerName }, new {status = 201, message = "Answer đã được tạo thành công.", data = answerDto});
            }
            else
            {
                // Trả về mã 500 Internal Server Error nếu có lỗi xảy ra
                return StatusCode(StatusCodes.Status500InternalServerError, new {status = 500, message = "Không thể tạo answer."});
            }
        }


        [HttpGet("getByID/{answerId}")]
        public async Task<IActionResult> GetAnswerById(int answerId)
        {
            if (answerId <= 0)
            {
                // Trả về mã lỗi 400 nếu AnswerId không hợp lệ
                return BadRequest(new {status = 400, message = "ID answer không hợp lệ."});
            }

            var answerInfo = await _answerService.GetAnswerById(answerId);

            if (answerInfo != null)
            {
                // Trả về mã 200 OK và thông tin answer nếu tìm thấy
                return Ok(answerInfo);
            }
            else
            {
                // Trả về mã lỗi 404 nếu không tìm thấy answer
                return NotFound( new {status = 404, message = "Không tìm thấy answer với ID đã cho."});
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateAnswer([FromBody] Answer_UpdateReq updateReq)
        {
            if (updateReq == null || string.IsNullOrWhiteSpace(updateReq.AnswerName) || string.IsNullOrWhiteSpace(updateReq.AnswerTextContent))
            {
                return BadRequest(new {status = 400, message = "Dữ liệu cập nhật không hợp lệ."});
            }

            var isUpdated = await _answerService.UpdateAnswer(updateReq);

            if (isUpdated)
            {
                return Ok(new {status = 200 ,message = "Cập nhật answer thành công.", data = updateReq});
            }
            else
            {
                return NotFound( new {status = 404, message = "Không tìm thấy answer hoặc cập nhật thất bại."});
            }
        }


        [HttpDelete("delete/{answerId}")]
        public async Task<IActionResult> SoftDeleteAnswer(int answerId)
        {
            if (answerId <= 0)
            {
                return BadRequest( new {status = 400, message = "ID answer không hợp lệ."});
            }

            var isDeleted = await _answerService.DeleteAnswer(answerId);

            if (isDeleted)
            {
                return Ok( new {status = 200, message = "Đã xóa mềm answer thành công."});
            }
            else
            {
                return NotFound( new {status = 404 ,message = "Không tìm thấy answer với ID đã cho."});
            }
        }

    }

}
