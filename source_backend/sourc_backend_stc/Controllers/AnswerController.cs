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
    public class AnswerController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AnswerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Lấy tất cả dữ liệu Answer
        [HttpGet("getAll")]
        public IActionResult GetAllAnswer()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC GetAllAnswers";
                    var Answeres = connection.Query<Answer>(query).ToList();
                    return Ok(Answeres);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Tạo mới Answer
        [HttpPost("create")]
        public IActionResult CreateAnswer([FromBody] Answer createAnswer)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC CreateAnswer @AnswerName, @AnswerTextContent, @AnswerImgContent, @IsTrue, @QuestionID";
                    var parameters = new
                    {
                        AnswerName = createAnswer.AnswerName,
                        AnswerTextContent = createAnswer.AnswerTextContent,
                        AnswerImgContent = createAnswer.AnswerImgContent,
                        IsTrue = createAnswer.IsTrue,
                        QuestionID = createAnswer.QuestionID
                    };
                    connection.Execute(query, parameters);

                    var response = new
                    {
                        success = true,
                        message = "Answer created successfully.",
                        data = createAnswer
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Lấy Answer theo ID
        [HttpGet("getById/{id}")]
        public IActionResult GetAnswerByID(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC GetAnswerById @AnswerID";
                    var answerData = connection.QuerySingleOrDefault<Answer>(query, new { AnswerId = id });
                    if (answerData == null) return NotFound("Answer not found.");
                    return Ok(answerData);
                }
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    success = false,
                    message = "An error occurred while creating the answer.",
                    error = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        // Cập nhật Answer
        [HttpPut("update/{id}")]
        public IActionResult UpdateAnswer(int id, [FromBody] Answer updatedAnswer)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC UpdateAnswer @AnswerID, @AnswerName, @AnswerTextContent, @AnswerImgContent, @IsTrue, @QuestionID";
                    var parameters = new
                    {
                        AnswerID = id,
                        AnswerName = updatedAnswer.AnswerName,
                        AnswerTextContent = updatedAnswer.AnswerTextContent,
                        AnswerImgContent = updatedAnswer.AnswerImgContent,
                        IsTrue = updatedAnswer.IsTrue,
                        QuestionID = updatedAnswer.QuestionID
                    };
                    connection.Execute(query, parameters);

                    var response = new
                    {
                        success = true,
                        message = "Answer updated successfully.",
                        data = updatedAnswer
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    success = false,
                    message = "An error occurred while updating the answer.",
                    error = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        // Xóa mềm Answer
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteAnswer(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC DeleteAnswer @AnswerID";
                    connection.Execute(query, new { AnswerID = id });

                    var response = new
                    {
                        success = true,
                        message = "Answer deleted successfully (soft delete).",
                        deletedAnswerId = id
                    };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    success = false,
                    message = "An error occurred while deleting the answer.",
                    error = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }

}
