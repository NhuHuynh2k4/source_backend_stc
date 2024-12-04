using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sourc_backend_stc.Models;
using sourc_backend_stc.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sourc_backend_stc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            try
            {
                var subjects = await _subjectService.GetAllSubjectsAsync();
                return Ok(subjects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        // Tạo mới môn học
        [HttpPost("create")]
        public async Task<IActionResult> CreateSubject([FromBody] Subject_CreateReq request)
        {
            // Kiểm tra dữ liệu đầu vào
            if (request == null ||
                string.IsNullOrWhiteSpace(request.SubjectsCode) || request.SubjectsCode == "string" ||
                string.IsNullOrWhiteSpace(request.SubjectsName) || request.SubjectsName == "string")
            {
                return BadRequest("Dữ liệu yêu cầu không hợp lệ.");
            }

            try
            {
                await _subjectService.CreateSubjectAsync(request);
                return Ok(new { message = "Môn học đã tạo thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi tạo môn học: {ex.Message}" });
            }
        }

        // Lấy môn học theo ID
        [HttpGet("get-by-id/{subjectId}")]
        public async Task<IActionResult> GetSubjectById(int subjectId)
        {
            if (subjectId <= 0)
                return BadRequest("ID môn học không hợp lệ.");

            try
            {
                var subject = await _subjectService.GetSubjectByIdAsync(subjectId);
                if (subject != null)
                    return Ok(subject);

                return NotFound("Không tìm thấy môn học với ID đã cho.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        // Cập nhật môn học
        [HttpPut("update")]
        public async Task<IActionResult> UpdateSubject([FromBody] Subject_UpdateReq request)
        {
            // Kiểm tra dữ liệu đầu vào
            if (request == null ||
                string.IsNullOrWhiteSpace(request.SubjectsCode) || request.SubjectsCode == "string" ||
                string.IsNullOrWhiteSpace(request.SubjectsName) || request.SubjectsName == "string")
            {
                return BadRequest(new { message = "Dữ liệu cập nhật không hợp lệ." });
            }

            try
            {
                // Cập nhật môn học dựa trên SubjectsCode
                var isUpdated = await _subjectService.UpdateSubjectAsync(request);
                if (isUpdated)
                {
                    // Trả về đối tượng JSON khi cập nhật thành công
                    return Ok(new { message = "Cập nhật môn học thành công." });
                }
                return NotFound(new { message = "Không tìm thấy môn học hoặc cập nhật thất bại." });
            }
            catch (Exception ex)
            {
                // Trả về thông báo lỗi dưới dạng JSON khi gặp lỗi
                return StatusCode(500, new { message = $"Lỗi khi cập nhật môn học: {ex.Message}" });
            }
        }



        // Xóa mềm môn học
        [HttpDelete("delete/{subjectId}")]
        public async Task<IActionResult> SoftDeleteSubject(int subjectId)
        {
            if (subjectId <= 0)
                return BadRequest("ID môn học không hợp lệ.");

            try
            {
                var isDeleted = await _subjectService.DeleteSubjectAsync(subjectId);
                if (isDeleted)
                    return Ok("Đã xóa mềm môn học thành công.");

                return NotFound("Không tìm thấy môn học với ID đã cho.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi xóa môn học: {ex.Message}" });
            }
        }

        // Endpoint để xuất lớp học ra file Excel
        [HttpGet("export")]
        public async Task<IActionResult> ExportSubjectsToExcel()
        {
            try
            {
                // Lấy danh sách các lớp học từ dịch vụ của bạn
                var subjects = await _subjectService.GetAllSubjectsAsync();

                // Chuyển đổi từ IEnumerable sang List
                var subjectsList = subjects.ToList(); // Sử dụng ToList() để chuyển đổi

                // Sử dụng service ExcelExportService để xuất dữ liệu ra file Excel
                var excelFile = _subjectService.ExportSubjectsToExcel(subjectsList);

                // Trả về file Excel cho client
                return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Subjects.xlsx");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra khi xuất Excel.");
            }
        }
    }
}