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
    [Authorize]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        // Lấy tất cả các môn học
        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<SubjectReadAllRes>>> GetAllSubjects()
        {
            var subjects = await _subjectService.GetAllSubjectsAsync();
            return Ok(subjects);
        }

        // Tạo mới môn học
        [HttpPost("create")]
        public async Task<IActionResult> CreateSubject([FromBody] Subject_CreateReq request)
        {
            if (request == null)
                return BadRequest("Yêu cầu không hợp lệ.");

            try
            {
                // Gọi phương thức tạo môn học
                await _subjectService.CreateSubjectAsync(request);
                return Ok(new { message = "Môn học đã tạo thành công." });
            }
            catch (Exception ex)
            {
                // Trả về thông báo lỗi nếu có ngoại lệ xảy ra
                return BadRequest(new { message = ex.Message });
            }
        }


        // Lấy môn học theo ID
        [HttpGet("get-by-id/{subjectId}")]
        public async Task<IActionResult> GetSubjectById(int subjectId)
        {
            if (subjectId <= 0)
                return BadRequest("ID môn học không hợp lệ.");

            var subject = await _subjectService.GetSubjectByIdAsync(subjectId);
            return subject != null ? Ok(subject) : NotFound("Không tìm thấy môn học với ID đã cho.");
        }

        // Cập nhật môn học
        [HttpPut("update/{subjectId}")]
        public async Task<IActionResult> UpdateSubject(int subjectId, [FromBody] Subject_UpdateReq request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.SubjectsCode) || string.IsNullOrWhiteSpace(request.SubjectsName))
                return BadRequest("Dữ liệu cập nhật không hợp lệ.");

            var isUpdated = await _subjectService.UpdateSubjectAsync(subjectId, request);

            return isUpdated ? Ok("Cập nhật môn học thành công.") : NotFound("Không tìm thấy môn học hoặc cập nhật thất bại.");
        }

        // Xóa mềm môn học
        [HttpDelete("delete/{subjectId}")]
        public async Task<IActionResult> SoftDeleteSubject(int subjectId)
        {
            if (subjectId <= 0)
                return BadRequest("ID môn học không hợp lệ.");

            var isDeleted = await _subjectService.DeleteSubjectAsync(subjectId);
            return isDeleted ? Ok("Đã xóa mềm môn học thành công.") : NotFound("Không tìm thấy môn học với ID đã cho.");
        }
    }
}
