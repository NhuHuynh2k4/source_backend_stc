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
            if (request == null)
                return BadRequest("Dữ liệu câu hỏi không hợp lệ.");

            // Check required fields
            if (string.IsNullOrWhiteSpace(request.QuestionCode))
                return BadRequest("Mã câu hỏi không được để trống.");
            if (string.IsNullOrWhiteSpace(request.QuestionName))
                return BadRequest("Tên câu hỏi không được để trống.");
            if (string.IsNullOrWhiteSpace(request.QuestionTextContent))
                return BadRequest("Nội dung văn bản câu hỏi không được để trống.");
            if (request.SubjectsID <= 0)
                return BadRequest("ID môn học không hợp lệ.");
            if (request.QuestionTypeID <= 0)
                return BadRequest("ID loại câu hỏi không hợp lệ.");

            // Optional field checks
            if (string.IsNullOrWhiteSpace(request.QuestionImgContent))
                request.QuestionImgContent = null; // Set to null if not provided

            try
            {
                await _questionService.CreateQuestion(request);
                return Ok(new { message = "Câu hỏi đã tạo thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        // Cập nhật câu hỏi
        [HttpPut("update/{questionId}")]
        public async Task<IActionResult> UpdateQuestion(int questionId, [FromBody] Question_UpdateReq request)
        {
            if (request == null)
                return BadRequest("Dữ liệu cập nhật câu hỏi không hợp lệ.");

            // Validate required fields
            if (string.IsNullOrWhiteSpace(request.QuestionCode))
                return BadRequest("Mã câu hỏi không được để trống.");
            if (string.IsNullOrWhiteSpace(request.QuestionName))
                return BadRequest("Tên câu hỏi không được để trống.");
            if (string.IsNullOrWhiteSpace(request.QuestionTextContent))
                return BadRequest("Nội dung văn bản câu hỏi không được để trống.");
            if (request.SubjectsID <= 0)
                return BadRequest("ID môn học không hợp lệ.");
            if (request.QuestionTypeID <= 0)
                return BadRequest("ID loại câu hỏi không hợp lệ.");

            try
            {
                var isUpdated = await _questionService.UpdateQuestion(questionId, request);
                if (isUpdated)
                    return Ok("Cập nhật câu hỏi thành công.");

                return NotFound("Không tìm thấy câu hỏi hoặc cập nhật thất bại.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
