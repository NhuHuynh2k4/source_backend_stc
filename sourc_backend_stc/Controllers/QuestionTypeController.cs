using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using sourc_backend_stc.Models;
using sourc_backend_stc.Services;
using System.Threading.Tasks;

namespace sourc_backend_stc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuestionTypeController : ControllerBase
    {
        private readonly IQuestionTypeService _questionTypeService;
        private readonly ILogger<QuestionTypeController> _logger;

        public QuestionTypeController(IQuestionTypeService questionTypeService, ILogger<QuestionTypeController> logger)
        {
            _questionTypeService = questionTypeService;
            _logger = logger;
        }

        // Lấy tất cả QuestionTypes
        [HttpGet("get-all")]
        
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _questionTypeService.GetAllQuestionType();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi lấy tất cả QuestionTypes: {ex.Message}");
                return BadRequest(new { message = "Đã có lỗi xảy ra khi lấy danh sách các loại câu hỏi." });
            }
        }

        // Lấy QuestionType theo ID
        [HttpGet("get-by-id/{id}")]
        
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _questionTypeService.GetQuestionTypeById(id);
                if (result == null)
                {
                    return NotFound(new { message = $"Không tìm thấy loại câu hỏi với ID: {id}" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi lấy QuestionType theo ID {id}: {ex.Message}");
                return BadRequest(new { message = "Đã có lỗi xảy ra khi lấy loại câu hỏi." });
            }
        }

        // Tạo mới QuestionType
        [HttpPost("create")]
        
        public async Task<IActionResult> Create([FromBody] QuestionType_CreateReq req)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var isCreated = await _questionTypeService.CreateQuestionType(req);
                if (isCreated)
                {
                    return Ok(new { message = "Tạo mới thành công." });
                }
                return BadRequest(new { message = "Không thể tạo mới loại câu hỏi." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tạo mới QuestionType: {ex.Message}");
                return BadRequest(new { message = "Đã có lỗi xảy ra khi tạo mới loại câu hỏi." });
            }
        }

        // Cập nhật QuestionType
        [HttpPut("update")]
        
        public async Task<IActionResult> Update([FromBody] QuestionType_UpdateReq req)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var isUpdated = await _questionTypeService.UpdateQuestionType(req);
                if (isUpdated)
                {
                    return Ok(new { message = "Cập nhật thành công." });
                }
                return NotFound(new { message = $"Không tìm thấy loại câu hỏi với ID: {req.QuestionTypeID}" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi cập nhật QuestionType với ID {req.QuestionTypeID}: {ex.Message}");
                return StatusCode(500, new { message = "Đã có lỗi xảy ra khi cập nhật loại câu hỏi." });
            }
        }


        // Xóa QuestionType
        [HttpDelete("delete/{id}")]
        
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var isDeleted = await _questionTypeService.DeleteQuestionType(id);
                if (isDeleted)
                {
                    return Ok(new { message = "Xóa thành công." });
                }
                return NotFound(new { message = $"Không tìm thấy loại câu hỏi với ID: {id}" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi xóa QuestionType với ID {id}: {ex.Message}");
                return BadRequest(new { message = "Đã có lỗi xảy ra khi xóa loại câu hỏi." });
            }
        }

        [HttpGet("export")]
        
        public async Task<IActionResult> ExportQuestionTypeToExcel()
        {
            try
            {
                // Lấy danh sách các lớp học từ dịch vụ của bạn
                var questionTypes = await _questionTypeService.GetAllQuestionType();

                // Chuyển đổi từ IEnumerable sang List
                var questionTypeList = questionTypes.ToList(); // Sử dụng ToList() để chuyển đổi

                // Sử dụng service ExcelExportService để xuất dữ liệu ra file Excel
                var excelFile = _questionTypeService.ExportQuestionTypeToExcel(questionTypeList);

                // Trả về file Excel cho client
                return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ClassStudents.xlsx");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra khi xuất Excel.");
            }
        }
    }
}
