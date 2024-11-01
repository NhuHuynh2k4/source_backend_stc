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

        [HttpGet("get-by-id/{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var result = _classStudentService.GetClassStudentById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                var result = _classStudentService.GetAllClassStudents();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("create")]
        public IActionResult Create([FromBody] ClassStudent_CreateReq request)
        {
            try
            {
                _classStudentService.CreateClassStudent(request);
                return Ok(new { message = "Tạo mới thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] ClassStudent_CreateReq request)
        {
            try
            {
                _classStudentService.UpdateClassStudent(id, request);
                return Ok(new { message = "Cập nhật thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _classStudentService.DeleteClassStudent(id);
                return Ok(new { message = "Xóa thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
