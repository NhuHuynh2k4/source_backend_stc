using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using sourc_backend_stc.Models;
using System;
using System.Linq;

namespace sourc_backend_stc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TestController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Lấy tất cả dữ liệu Test
        [HttpGet("get-all")]
        public IActionResult GetAllTests()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC GetAllTest;";
                    var tests = connection.Query<Test>(query).ToList();
                    return Ok(tests);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Lấy Test theo ID
        [HttpGet("get-by-id/{id}")]
        public IActionResult GetTestByID(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC GetTestByID @TestID";
                    var testData = connection.QuerySingleOrDefault<Test>(query, new { TestID = id });
                    if (testData == null) return NotFound("Test not found.");
                    return Ok(testData);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Tạo mới Test
        [HttpPost("create")]
        public IActionResult CreateTest([FromBody] Test newTest)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC CreateTest @TestCode, @TestName, @NumberOfQuestions, @SubjectsID";
                    var parameters = new
                    {
                        TestCode = newTest.TestCode,
                        TestName = newTest.TestName,
                        NumberOfQuestions = newTest.NumberOfQuestions,
                        SubjectsID = newTest.SubjectsID
                    };
                    connection.Execute(query, parameters);
                    return Ok("Test created successfully.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Cập nhật Test
        [HttpPut("update/{id}")]
public IActionResult UpdateTest(int id, [FromBody] Test updatedTest)
{
    if (updatedTest == null)
    {
        return BadRequest("The updatedTest field is required.");
    }

    try
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var query = "UPDATE Test SET UpdateDate = GETDATE()";
            var parameters = new DynamicParameters();
            parameters.Add("@TestID", id);

            if (!string.IsNullOrEmpty(updatedTest.TestCode))
            {
                query += ", TestCode = @TestCode";
                parameters.Add("@TestCode", updatedTest.TestCode);
            }

            if (!string.IsNullOrEmpty(updatedTest.TestName))
            {
                query += ", TestName = @TestName";
                parameters.Add("@TestName", updatedTest.TestName);
            }

            if (updatedTest.NumberOfQuestions.HasValue)
            {
                query += ", NumberOfQuestions = @NumberOfQuestions";
                parameters.Add("@NumberOfQuestions", updatedTest.NumberOfQuestions.Value);
            }

            if (updatedTest.SubjectsID.HasValue)
            {
                query += ", SubjectsID = @SubjectsID";
                parameters.Add("@SubjectsID", updatedTest.SubjectsID.Value);
            }

            query += " WHERE TestID = @TestID AND IsDelete = 0";
            var rowsAffected = connection.Execute(query, parameters);

            if (rowsAffected == 0)
            {
                return NotFound("Test not found or already deleted.");
            }

            return Ok("Test updated successfully.");
        }
    }
    catch (Exception ex)
    {
        return StatusCode(500, ex.Message);
    }
}


        // Xóa mềm Test
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteTest(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC DeleteTest @TestID";
                    connection.Execute(query, new { TestID = id });
                    return Ok("Test deleted successfully (soft delete).");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
