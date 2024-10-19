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
    public class ClassController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ClassController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Lấy tất cả dữ liệu Class
        [HttpGet("get-all")]
        public IActionResult GetAllClass()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC GetAllClass;";
                    var classes = connection.Query<Class>(query).ToList();
                    return Ok(classes);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Tạo mới Class
        [HttpPost("create")]
        public IActionResult CreateClass([FromBody] Class newClass)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC CreateClass @ClassCode, @ClassName, @Session, @SubjectsID";
                    var parameters = new
                    {
                        ClassCode = newClass.ClassCode,
                        ClassName = newClass.ClassName,
                        Session = newClass.Session,
                        SubjectsID = newClass.SubjectsID
                    };
                    connection.Execute(query, parameters);
                    return Ok("Class created successfully.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Lấy Class theo ID
        [HttpGet("get-by-id/{id}")]
        public IActionResult GetClassByID(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC GetClassByID @ClassID";
                    var classData = connection.QuerySingleOrDefault<Class>(query, new { ClassID = id });
                    if (classData == null) return NotFound("Class not found.");
                    return Ok(classData);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Cập nhật Class
        [HttpPut("update/{id}")]
        public IActionResult UpdateClass(int id, [FromBody] Class updatedClass)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC UpdateClass @ClassID, @ClassCode, @ClassName, @Session, @SubjectsID";
                    var parameters = new
                    {
                        ClassID = id,
                        ClassCode = updatedClass.ClassCode,
                        ClassName = updatedClass.ClassName,
                        Session = updatedClass.Session,
                        SubjectsID = updatedClass.SubjectsID
                    };
                    connection.Execute(query, parameters);
                    return Ok("Class updated successfully.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Xóa mềm Class
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteClass(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC DeleteClass @ClassID";
                    connection.Execute(query, new { ClassID = id });
                    return Ok("Class deleted successfully (soft delete).");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

}
