using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sourc_backend_stc.Models;
using sourc_backend_stc.Services;

namespace sourc_backend_stc.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
            try
            {
                var questions = await _questionService.GetAllQuestions();
                return Ok(questions); // Trả về danh sách câu hỏi với mã 200 OK
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        // Lấy câu hỏi theo ID
        [HttpGet("get-by-id/{questionId}")]
        public async Task<IActionResult> GetQuestionById(int questionId)
        {
            if (questionId <= 0)
                return BadRequest("ID câu hỏi không hợp lệ.");

            try
            {
                var question = await _questionService.GetQuestionById(questionId);
                if (question != null)
                    return Ok(question);

                return NotFound("Không tìm thấy câu hỏi với ID đã cho.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        // Tạo mới câu hỏi
        [HttpPost("create")]
        public async Task<IActionResult> CreateQuestion([FromBody] Question_CreateReq request)
        {
            if (request == null)
                return BadRequest("Dữ liệu câu hỏi không hợp lệ.");

            // Kiểm tra các trường bắt buộc
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

            // Optional: nếu không có ảnh thì để null
            request.QuestionImgContent = string.IsNullOrWhiteSpace(request.QuestionImgContent) ? null : request.QuestionImgContent;

            try
            {
                await _questionService.CreateQuestion(request);
                return Ok(new { message = "Câu hỏi đã tạo thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi tạo câu hỏi: {ex.Message}" });
            }
        }


        // Cập nhật câu hỏi
        [HttpPut("update")]
        public async Task<IActionResult> UpdateQuestion([FromBody] Question_UpdateReq updateReq)
        {
            // Kiểm tra dữ liệu đầu vào
            if (updateReq == null)
                return BadRequest(new { message = "Dữ liệu cập nhật câu hỏi không hợp lệ." });

            // Validate các trường bắt buộc
            var validationErrors = ValidateUpdateRequest(updateReq);
            if (validationErrors.Count > 0)
                return BadRequest(new { message = "Dữ liệu không hợp lệ.", errors = validationErrors });

            try
            {
                // Gọi service để cập nhật câu hỏi
                var isUpdated = await _questionService.UpdateQuestion(updateReq);

                if (isUpdated)
                    return Ok(new { message = "Cập nhật câu hỏi thành công." });

                return NotFound(new { message = "Không tìm thấy câu hỏi hoặc cập nhật thất bại." });
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về lỗi server
                return StatusCode(500, new { message = $"Lỗi khi cập nhật câu hỏi: {ex.Message}" });
            }
        }

        // Phương thức validate dữ liệu cập nhật
        private List<string> ValidateUpdateRequest(Question_UpdateReq updateReq)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(updateReq.QuestionCode))
                errors.Add("Mã câu hỏi không được để trống.");
            if (string.IsNullOrWhiteSpace(updateReq.QuestionName))
                errors.Add("Tên câu hỏi không được để trống.");
            if (string.IsNullOrWhiteSpace(updateReq.QuestionTextContent))
                errors.Add("Nội dung văn bản câu hỏi không được để trống.");
            if (updateReq.SubjectsID <= 0)
                errors.Add("ID môn học không hợp lệ.");
            if (updateReq.QuestionTypeID <= 0)
                errors.Add("ID loại câu hỏi không hợp lệ.");

            return errors;
        }

        // Xóa mềm câu hỏi
        [HttpDelete("delete/{questionId}")]
        public async Task<IActionResult> SoftDeleteQuestion(int questionId)
        {
            if (questionId <= 0)
                return BadRequest("ID câu hỏi không hợp lệ.");

            try
            {
                var isDeleted = await _questionService.DeleteQuestion(questionId);
                if (isDeleted)
                    return Ok("Đã xóa mềm câu hỏi thành công.");

                return NotFound("Không tìm thấy câu hỏi với ID đã cho.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi xóa câu hỏi: {ex.Message}" });
            }
        }
        [HttpGet("export")]
        public async Task<IActionResult> ExportQuestionsToExcel()
        {
            try
            {
                // Lấy danh sách các lớp học từ dịch vụ của bạn
                var questions = await _questionService.GetAllQuestions();

                // Chuyển đổi từ IEnumerable sang List
                var questionsList = questions.ToList(); // Sử dụng ToList() để chuyển đổi

                // Sử dụng service ExcelExportService để xuất dữ liệu ra file Excel
                var excelFile = _questionService.ExportQuestionToExcel(questionsList);

                // Trả về file Excel cho client
                return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Questions.xlsx");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra khi xuất Excel.");
            }
        }
    }
}
