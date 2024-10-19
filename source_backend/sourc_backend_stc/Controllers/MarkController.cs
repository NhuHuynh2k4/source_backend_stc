using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using sourc_backend_stc.Data;
using sourc_backend_stc.Models;
using Dapper;

namespace sourc_backend_stc.Controllers  
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarkController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public MarkController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Lấy tất cả dữ liệu Mark
        [HttpGet("get-all")]
        public IActionResult GetAllMarks()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC GetAllMark;";
                    var marks = connection.Query<Mark>(query).ToList();
                    return Ok(marks);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Tạo mới Mark
        [HttpPost("create")]
        public IActionResult CreateMark([FromBody] Mark newMark)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC CreateMark @Result, @PassingScore, @StudentID, @ExamID, @TestID";
                    var parameters = new
                    {
                        newMark.Result,
                        newMark.PassingScore,
                        newMark.StudentID,
                        newMark.ExamID,
                        newMark.TestID
                    };
                    connection.Execute(query, parameters);
                    return Ok("Mark created successfully.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Lấy Mark theo ID
        [HttpGet("get-by-id/{id}")]
        public IActionResult GetMarkByID(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC GetMarkByID @MarkID";
                    var mark = connection.QuerySingleOrDefault<Mark>(query, new { MarkID = id });
                    if (mark == null) return NotFound("Mark not found.");
                    return Ok(mark);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Cập nhật Mark
        [HttpPut("update/{id}")]
        public IActionResult UpdateMark(int id, [FromBody] Mark updatedMark)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC UpdateMark @MarkID, @Result, @PassingScore, @StudentID, @ExamID, @TestID";
                    var parameters = new
                    {
                        MarkID = id,
                        updatedMark.Result,
                        updatedMark.PassingScore,
                        updatedMark.StudentID,
                        updatedMark.ExamID,
                        updatedMark.TestID
                    };
                    connection.Execute(query, parameters);
                    return Ok("Mark updated successfully.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Xóa mềm Mark
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteMark(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC DeleteMark @MarkID";
                    connection.Execute(query, new { MarkID = id });
                    return Ok("Mark deleted successfully (soft delete).");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
