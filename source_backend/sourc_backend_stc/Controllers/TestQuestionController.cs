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
    public class TestQuestionController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TestQuestionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Lấy tất cả dữ liệu TestQuestion
        [HttpGet("getAll")]
        public IActionResult GetAllTestQuestion()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC GetAllTestQuestions";
                    var testQuestions = connection.Query<TestQuestion>(query).ToList();
                    return Ok(testQuestions);
                }
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    success = false,
                    message = "An error occurred while getting all the TestQuestion.",
                    error = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        // Tạo mới TestQuestion
        [HttpPost("create")]
        public IActionResult CreateTestQuestion([FromBody] TestQuestion createTestQuestion)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC CreateTestQuestion @QuestionID, @TestID";
                    var parameters = new
                    {
                        QuestionID = createTestQuestion.QuestionID,
                        TestID = createTestQuestion.TestID
                    };
                    connection.Execute(query, parameters);

                    var response = new
                    {
                        success = true,
                        message = "TestQuestion created successfully.",
                        data = createTestQuestion
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    success = false,
                    message = "An error occurred while creating the TestQuestion.",
                    error = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        // Lấy TestQuestion theo ID
        [HttpGet("getById/{id}")]
        public IActionResult GetTestQuestionByID(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC GetTestQuestionById @Test_QuestionID";
                    var testQuestionData = connection.QuerySingleOrDefault<TestQuestion>(query, new { Test_QuestionId = id });
                    if (testQuestionData == null) return NotFound("TestQuestion not found.");
                    return Ok(testQuestionData);
                }
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    success = false,
                    message = "An error occurred while getting by id the TestQuestion.",
                    error = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        // Cập nhật TestQuestion
        [HttpPut("update/{id}")]
        public IActionResult UpdateTestQuestion(int id, [FromBody] TestQuestion updatedTestQuestion)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC UpdateTestQuestion @Test_QuestionID, @QuestionID, @TestID";
                    var parameters = new
                    {
                        Test_QuestionID = id,
                        QuestionID = updatedTestQuestion.QuestionID,
                        TestID = updatedTestQuestion.TestID
                    };
                    connection.Execute(query, parameters);

                    var response = new
                    {
                        success = true,
                        message = "TestQuestion updated successfully.",
                        data = updatedTestQuestion
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    success = false,
                    message = "An error occurred while updating the TestQuestion.",
                    error = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        // Xóa mềm TestQuestion
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteTestQuestion(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC DeleteTestQuestion @Test_QuestionID";
                    connection.Execute(query, new { Test_QuestionID = id });

                    var response = new
                    {
                        success = true,
                        message = "TestQuestion deleted successfully (soft delete).",
                        deletedTestQuestionId = id
                    };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    success = false,
                    message = "An error occurred while deleting the TestQuestion.",
                    error = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }

}
