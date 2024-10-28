using Microsoft.AspNetCore.Mvc;
using sourc_backend_stc.Models;
using sourc_backend_stc.Services;

namespace sourc_backend_stc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionTypeController : ControllerBase
    {
        private readonly IQuestionTypeService _questionTypeService;

        public QuestionTypeController(IQuestionTypeService questionTypeService)
        {
            _questionTypeService = questionTypeService;
        }

        // Lấy QuestionType theo ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var result = _questionTypeService.GetQuestionTypeById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Tạo mới QuestionType
        [HttpPost]
        public IActionResult Create([FromBody] QuestionType_CreateReq req)
        {
            try
            {
                _questionTypeService.CreateQuestionType(req);
                return Ok(new { message = "Tạo mới thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Cập nhật QuestionType
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] QuestionType_CreateReq req)
        {
            try
            {
                _questionTypeService.UpdateQuestionType(id, req);
                return Ok(new { message = "Cập nhật thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Xóa QuestionType
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _questionTypeService.DeleteQuestionType(id);
                return Ok(new { message = "Xóa thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
