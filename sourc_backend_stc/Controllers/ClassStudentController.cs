using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sourc_backend_stc.Models;
using sourc_backend_stc.Services;

namespace sourc_backend_stc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClassStudentController : ControllerBase
    {
        private readonly IClassStudentService _classStudentService;

        public ClassStudentController(IClassStudentService classStudentService)
        {
            _classStudentService = classStudentService;
        }

        // Lấy thông tin ClassStudent theo ID
        [HttpGet("get-by-id/{id}")]
        
        public async Task<IActionResult> GetClassStudentByID(int id)
        {
            try
            {
                var result = await _classStudentService.GetClassStudentById(id);
                if (result == null) return NotFound(new { message = "ClassStudent không tồn tại." });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Lấy tất cả ClassStudents
        [HttpGet("get-all")]
        
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _classStudentService.GetAllClassStudent();
                if (result == null || !result.Any())
                {
                    return NotFound(new { message = "Không có bản ghi nào." });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách ClassStudent", error = ex.Message });
            }
        }


        // Tạo mới ClassStudent
        [HttpPost("create")]
        
        public async Task<IActionResult> Create([FromBody] ClassStudent_CreateReq request)
        {
            try
            {
                // Kiểm tra thông tin đầu vào
                if (request.ClassID <= 0)
                {
                    return BadRequest(new { message = "ID lớp học không hợp lệ." });
                }

                if (request.StudentID <= 0)
                {
                    return BadRequest(new { message = "ID sinh viên không hợp lệ." });
                }

                var success = await _classStudentService.CreateClassStudent(request);
                if (!success) return BadRequest(new { message = "Tạo mới thất bại." });
                return Ok(new { message = "Tạo mới thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Cập nhật ClassStudent
        [HttpPut("update")]
        
        public async Task<IActionResult> Update([FromBody] ClassStudent_UpdateReq updateReq)
        {
            try
            {
                // Kiểm tra thông tin đầu vào
                if (updateReq.Class_StudentID <= 0)
                {
                    return BadRequest(new { message = "ID không hợp lệ." });
                }

                if (updateReq.ClassID <= 0)
                {
                    return BadRequest(new { message = "ID lớp học không hợp lệ." });
                }

                if (updateReq.StudentID <= 0)
                {
                    return BadRequest(new { message = "ID sinh viên không hợp lệ." });
                }


                var success = await _classStudentService.UpdateClassStudent(updateReq.Class_StudentID, updateReq);
                if (!success) return BadRequest(new { message = "Cập nhật thất bại." });
                return Ok(new { message = "Cập nhật thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Xóa ClassStudent
        [HttpDelete("delete/{id}")]
        
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _classStudentService.DeleteClassStudent(id);
                if (!success) return BadRequest(new { message = "Xóa thất bại." });
                return Ok(new { message = "Xóa thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Endpoint để xuất lớp học ra file Excel
        [HttpGet("export")]
        
        public async Task<IActionResult> ExportClassStudentsToExcel()
        {
            try
            {
                // Lấy danh sách các lớp học từ dịch vụ của bạn
                var classStudents = await _classStudentService.GetAllClassStudent();

                // Chuyển đổi từ IEnumerable sang List
                var classStudentList = classStudents.ToList(); // Sử dụng ToList() để chuyển đổi

                // Sử dụng service ExcelExportService để xuất dữ liệu ra file Excel
                var excelFile = _classStudentService.ExportClassStudentsToExcel(classStudentList);

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
