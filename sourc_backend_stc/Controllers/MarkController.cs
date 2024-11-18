using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sourc_backend_stc.Models;
using sourc_backend_stc.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sourc_backend_stc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class MarkController : ControllerBase
    {
        private readonly IMarkService _markService;

        public MarkController(IMarkService markService)
        {
            _markService = markService;
        }

        // Lấy tất cả các điểm
        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<Mark_ReadAllRes>>> GetAllMarks()
        {
            var marks = await _markService.GetAllMarks();
            return Ok(marks);
        }

        // Tạo mới điểm
        [HttpPost("create")]
        public async Task<IActionResult> CreateMark([FromBody] Mark_CreateReq markDto)
        {
            if (markDto == null)
            {
                return BadRequest("Yêu cầu không hợp lệ.");
            }

            var isCreated = await _markService.CreateMark(markDto);

            if (isCreated)
            {
                return CreatedAtAction(nameof(CreateMark), new { id = markDto.Result }, "Điểm đã được tạo thành công.");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Không thể tạo điểm.");
            }
        }

        // Lấy điểm theo ID
        [HttpGet("get-by-id/{markId}")]
        public async Task<IActionResult> GetMarkById(int markId)
        {
            if (markId <= 0)
            {
                return BadRequest("ID điểm không hợp lệ.");
            }

            var markInfo = await _markService.GetMarkById(markId);

            if (markInfo != null)
            {
                return Ok(markInfo);
            }
            else
            {
                return NotFound("Không tìm thấy điểm với ID đã cho.");
            }
        }

        // Cập nhật điểm theo ID
        [HttpPut("update")]
        public async Task<IActionResult> UpdateMark([FromBody] Mark_UpdateReq updateReq)
        {
            if (updateReq == null)
            {
                return BadRequest("Dữ liệu cập nhật không hợp lệ.");
            }

            var isUpdated = await _markService.UpdateMark(updateReq);

            if (isUpdated)
            {
                return Ok("Cập nhật điểm thành công.");
            }
            else
            {
                return NotFound("Không tìm thấy điểm hoặc cập nhật thất bại.");
            }
        }

        // Xóa mềm điểm
        [HttpDelete("delete/{markId}")]
        public async Task<IActionResult> SoftDeleteMark(int markId)
        {
            if (markId <= 0)
            {
                return BadRequest("ID điểm không hợp lệ.");
            }

            var isDeleted = await _markService.DeleteMark(markId);

            if (isDeleted)
            {
                return Ok("Đã xóa mềm điểm thành công.");
            }
            else
            {
                return NotFound("Không tìm thấy điểm với ID đã cho.");
            }
        }
    }
}