using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using sourc_backend_stc.Data;
using sourc_backend_stc.Models;

namespace sourc_backend_stc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IConfiguration _config;

        public StudentController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult GetAllStudents()
        {
            var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var command = new SqlCommand("GetAllStudent", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            connection.Open();
            var reader = command.ExecuteReader();

            var Students = new List<Student>();
            while (reader.Read())
            {
                Students.Add(new Student
                {
                    StudentID = reader.GetInt32(0),
                    StudentCode = reader.GetString(1),
                    Gender = reader.GetBoolean(2),
                    NumberPhone = reader.GetString(3),
                    Address = reader.GetString(4),
                    BirthdayDate = reader.GetDateTime(5),
                    CreateDate = reader.GetDateTime(6),
                    UpdateDate = reader.GetDateTime(7),
                    IsDelete = reader.GetBoolean(8),
                    Email = reader.GetString(9)
                });
            }
            connection.Close();

            return Ok(Students);
        }

        [HttpPost]
        public IActionResult CreateStudent(Student Student)
        {
            // Kiểm tra StudentID
            if (Student.StudentID <= 0)
            {
                return BadRequest("StudentID must be a positive integer.");
            }

            // Kiểm tra StudentCode
            if (string.IsNullOrWhiteSpace(Student.StudentCode))
            {
                return BadRequest("StudentCode cannot be empty.");
            }

            // Kiểm tra Gender
            if (!Student.Gender.HasValue) // Gender là nullable
            {
                return BadRequest("Gender is required and cannot be empty.");
            }

            // Kiểm tra NumberPhone
            if (string.IsNullOrWhiteSpace(Student.NumberPhone) || Student.NumberPhone.Length != 10)
            {
                return BadRequest("NumberPhone must be exactly 10 characters long and cannot be empty.");
            }

            // Kiểm tra Email (nếu cần thiết)
            if (string.IsNullOrWhiteSpace(Student.Email))
            {
                return BadRequest("Email cannot be empty.");
            }

            var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var command = new SqlCommand("CreateStudent", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@StudentCode", Student.StudentCode);
            command.Parameters.AddWithValue("@Gender", Student.Gender);
            command.Parameters.AddWithValue("@NumberPhone", Student.NumberPhone);
            command.Parameters.AddWithValue("@Address", Student.Address);
            command.Parameters.AddWithValue("@BirthdayDate", Student.BirthdayDate);
            command.Parameters.AddWithValue("@Email", Student.Email);

            connection.Open();
            var newStudentId = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();

            Student.StudentID = newStudentId;
            return CreatedAtAction(nameof(GetAllStudents), new { id = newStudentId }, Student);
        }

       [HttpPut("{id}")]
public IActionResult UpdateStudent(int id, Student student)
{
    using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
    connection.Open();

    // Kiểm tra sinh viên có tồn tại
    var command = new SqlCommand("GetStudentByID", connection)
    {
        CommandType = CommandType.StoredProcedure
    };
    command.Parameters.AddWithValue("@StudentID", id);

    using var reader = command.ExecuteReader();

    // Kiểm tra nếu không có sinh viên
    if (!reader.HasRows)
    {
        return NotFound(new { message = $"Sinh viên với ID {id} không tồn tại." });
    }

    reader.Close(); // Đóng reader trước khi sử dụng lại connection

    // Cập nhật sinh viên
    command = new SqlCommand("UpdateStudent", connection)
    {
        CommandType = CommandType.StoredProcedure
    };
    command.Parameters.AddWithValue("@StudentID", id);
    command.Parameters.AddWithValue("@StudentCode", student.StudentCode);
    command.Parameters.AddWithValue("@Gender", student.Gender);
    command.Parameters.AddWithValue("@NumberPhone", student.NumberPhone);
    command.Parameters.AddWithValue("@Address", student.Address);
    command.Parameters.AddWithValue("@BirthdayDate", student.BirthdayDate);
    command.Parameters.AddWithValue("@Email", student.Email);

    var rowsAffected = command.ExecuteNonQuery(); // Thực hiện cập nhật

    // Kiểm tra nếu không có bản ghi nào được cập nhật
    if (rowsAffected == 0)
    {
        return BadRequest(new { message = "Cập nhật không thành công, vui lòng kiểm tra lại thông tin." });
    }

    // Trả về thông tin sinh viên đã cập nhật
    return Ok(new { message = "Cập nhật thành công!", student });
}





    }
}
