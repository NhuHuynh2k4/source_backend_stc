using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using sourc_backend_stc.Models;
using Dapper;

namespace sourc_backend_stc.Controllers  
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ExamController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Lấy tất cả dữ liệu Exam
        [HttpGet("get-all")]
        public IActionResult GetAllExams()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC GetAllExam;";
                    var exams = connection.Query<Exam>(query).ToList();
                    return Ok(exams);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Tạo mới Exam
        [HttpPost("create")]
        public IActionResult CreateExam([FromBody] Exam newExam)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC CreateExam @ExamCode, @ExamName, @ExamDate, @Duration, @NumberOfQuestions, @TotalMarks, @TestID";
                    var parameters = new
                    {
                        newExam.ExamCode,
                        newExam.ExamName,
                        newExam.ExamDate,
                        newExam.Duration,
                        newExam.NumberOfQuestions,
                        newExam.TotalMarks,
                        newExam.TestID
                    };
                    connection.Execute(query, parameters);
                    return Ok("Exam created successfully.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Lấy Exam theo ID
        [HttpGet("get-by-id/{id}")]
        public IActionResult GetExamByID(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC GetExamByID @ExamID";
                    var exam = connection.QuerySingleOrDefault<Exam>(query, new { ExamID = id });
                    if (exam == null) return NotFound("Exam not found.");
                    return Ok(exam);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Cập nhật Exam
        [HttpPut("update/{id}")]
        public IActionResult UpdateExam(int id, [FromBody] Exam updatedExam)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC UpdateExam @ExamID, @ExamCode, @ExamName, @ExamDate, @Duration, @NumberOfQuestions, @TotalMarks, @TestID";
                    var parameters = new
                    {
                        ExamID = id,
                        updatedExam.ExamCode,
                        updatedExam.ExamName,
                        updatedExam.ExamDate,
                        updatedExam.Duration,
                        updatedExam.NumberOfQuestions,
                        updatedExam.TotalMarks,
                        updatedExam.TestID
                    };
                    connection.Execute(query, parameters);
                    return Ok("Exam updated successfully.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Xóa mềm Exam
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteExam(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = "EXEC DeleteExam @ExamID";
                    connection.Execute(query, new { ExamID = id });
                    return Ok("Exam deleted successfully (soft delete).");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
