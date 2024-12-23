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
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        // Lấy tất cả các kỳ thi
        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<Exam_ReadAllRes>>> GetAllExams()
        {
            var exams = await _examService.GetAllExams();
            return Ok(exams);
        }

        // Tạo mới kỳ thi
        [HttpPost("create")]
        public async Task<IActionResult> CreateExam([FromBody] Exam_CreateReq examDto)
        {
            if (examDto == null)
            {
                return BadRequest("Yêu cầu không hợp lệ.");
            }

            var isCreated = await _examService.CreateExam(examDto);

            if (isCreated)
            {
                return CreatedAtAction(nameof(CreateExam), new { id = examDto.ExamCode }, "Kỳ thi đã được tạo thành công.");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Không thể tạo kỳ thi.");
            }
        }

        // Lấy kỳ thi theo ID
        [HttpGet("get-by-id/{examId}")]
        public async Task<IActionResult> GetExamById(int examId)
        {
            if (examId <= 0)
            {
                return BadRequest("ID kỳ thi không hợp lệ.");
            }

            var examInfo = await _examService.GetExamById(examId);

            if (examInfo != null)
            {
                return Ok(examInfo);
            }
            else
            {
                return NotFound("Không tìm thấy kỳ thi với ID đã cho.");
            }
        }

        // Cập nhật kỳ thi theo ID
        [HttpPut("update")]
        public async Task<IActionResult> UpdateExam([FromBody] Exam_UpdateReq updateReq)
        {
            if (updateReq == null)
            {
                return BadRequest("Dữ liệu cập nhật không hợp lệ.");
            }

            var isUpdated = await _examService.UpdateExam(updateReq);

            if (isUpdated)
            {
                return Ok("Cập nhật kỳ thi thành công.");
            }
            else
            {
                return NotFound("Không tìm thấy kỳ thi hoặc cập nhật thất bại.");
            }
        }

        // Xóa mềm kỳ thi
        [HttpDelete("delete/{examId}")]
        public async Task<IActionResult> SoftDeleteExam(int examId)
        {
            if (examId <= 0)
            {
                return BadRequest("ID kỳ thi không hợp lệ.");
            }

            var isDeleted = await _examService.DeleteExam(examId);

            if (isDeleted)
            {
                return Ok("Đã xóa mềm kỳ thi thành công.");
            }
            else
            {
                return NotFound("Không tìm thấy kỳ thi với ID đã cho.");
            }
        }

        // Endpoint để xuất lớp học ra file Excel
        [HttpGet("export")]
        public async Task<IActionResult> ExportExamsToExcel()
        {
            try
            {
                // Lấy danh sách các lớp học từ dịch vụ của bạn
                var exams = await _examService.GetAllExams();

                // Chuyển đổi từ IEnumerable sang List
                var examsList = exams.ToList(); // Sử dụng ToList() để chuyển đổi

                // Sử dụng service ExcelExportService để xuất dữ liệu ra file Excel
                var excelFile = _examService.ExportExamsToExcel(examsList);

                // Trả về file Excel cho client
                return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Exams.xlsx");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra khi xuất Excel.");
            }
        }
    }
}