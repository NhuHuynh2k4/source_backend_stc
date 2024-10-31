using Microsoft.AspNetCore.Mvc;
using sourc_backend_stc.Models;
using sourc_backend_stc.Services;

namespace sourc_backend_stc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        // Lấy tất cả câu hỏi
        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<QuestionReadAllRes>>> GetAllQuestions()
        {
            var questions = await _questionService.GetAllQuestions();
            return Ok(questions); // Trả về danh sách câu hỏi với mã 200 OK
        }

        // Lấy câu hỏi theo ID
        [HttpGet("get-by-id/{questionId}")]
        public async Task<IActionResult> GetQuestionById(int questionId)
        {
            if (questionId <= 0)
                return BadRequest("ID câu hỏi không hợp lệ.");

            var question = await _questionService.GetQuestionById(questionId);
            if (question != null)
                return Ok(question);

            return NotFound("Không tìm thấy câu hỏi với ID đã cho.");
        }

        // Tạo mới câu hỏi
        [HttpPost("create")]
        public async Task<IActionResult> CreateQuestion([FromBody] Question_CreateReq request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.QuestionCode) || string.IsNullOrWhiteSpace(request.QuestionName))
                return BadRequest("Dữ liệu câu hỏi không hợp lệ.");

            var newQuestionId = await _questionService.CreateQuestion(request);
            if (newQuestionId > 0)
                return CreatedAtAction(nameof(GetQuestionById), new { questionId = newQuestionId }, "Câu hỏi đã được tạo thành công.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Không thể tạo câu hỏi.");
        }

        // Cập nhật câu hỏi
        [HttpPut("update/{questionId}")]
        public async Task<IActionResult> UpdateQuestion(int questionId, [FromBody] Question_UpdateReq request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.QuestionCode) || string.IsNullOrWhiteSpace(request.QuestionName))
                return BadRequest("Dữ liệu cập nhật câu hỏi không hợp lệ.");

            var isUpdated = await _questionService.UpdateQuestion(questionId, request);
            if (isUpdated)
                return Ok("Cập nhật câu hỏi thành công.");

            return NotFound("Không tìm thấy câu hỏi hoặc cập nhật thất bại.");
        }

        // Xóa mềm câu hỏi
        [HttpDelete("delete/{questionId}")]
        public async Task<IActionResult> SoftDeleteQuestion(int questionId)
        {
            if (questionId <= 0)
                return BadRequest("ID câu hỏi không hợp lệ.");

            var isDeleted = await _questionService.DeleteQuestion(questionId);
            if (isDeleted)
                return Ok("Đã xóa mềm câu hỏi thành công.");

            return NotFound("Không tìm thấy câu hỏi với ID đã cho.");
        }
    }
}
