using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using sourc_backend_stc.Data;
using sourc_backend_stc.Models;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Text.RegularExpressions;
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
            try
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
                        StudentName = reader.GetString(2),
                        Gender = reader.GetBoolean(3),
                        NumberPhone = reader.GetString(4),
                        Address = reader.GetString(5),
                        BirthdayDate = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                        // CreateDate = reader.GetDateTime(7),
                        // UpdateDate = reader.GetDateTime(8),
                        // IsDelete = reader.GetBoolean(9),
                        Email = reader.GetString(7)
                    });
                }
                connection.Close();

                return Ok(Students);
            }
            catch (SqlException sqlEx)
            {
                return StatusCode(500, $"SQL Query error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An Query occurred: {ex.Message}");
            }
        }
        [HttpGet("{id}")]
        public IActionResult GetStudentByID(int id)
        {
            try
            {


                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var command = new SqlCommand("GetStudentByID", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@StudentID", id);
                connection.Open();

                using var reader = command.ExecuteReader();
                // Kiểm tra nếu không có sinh viên
                if (!reader.HasRows)
                {
                    return NotFound(new { message = $"Sinh viên với ID {id} không tồn tại." });
                }


                Student student = null;
                while (reader.Read())
                {
                    student = new Student
                    {
                        StudentID = reader.GetInt32(0),
                        StudentCode = reader.GetString(1),
                        StudentName = reader.GetString(2),
                        Gender = reader.GetBoolean(3),
                        NumberPhone = reader.GetString(4),
                        Address = reader.GetString(5),
                        BirthdayDate = reader.GetDateTime(6),
                        Email = reader.GetString(7)
                    };
                }




                return Ok(student);
            }
            catch (SqlException sqlEx)
            {
                return StatusCode(500, $"Lỗi truy vấn SQL: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult CreateStudent(Student Student)
        {


            // Kiểm tra StudentCode
            if (string.IsNullOrWhiteSpace(Student.StudentCode))
            {
                return BadRequest("Mã sinh viên không được bỏ trống.");
            }
            if (Student.StudentCode.Length > 15)
            {
                return BadRequest("Mã sinh viên không được quá 15 ký tự");
            }


            // Kiểm tra Gender
            if (!Student.Gender.HasValue) // Gender là nullable
            {
                return BadRequest("Giới tính không được bỏ trống.");
            }

            // Kiểm tra NumberPhone
            if (string.IsNullOrWhiteSpace(Student.NumberPhone) )
            {
                return BadRequest("Số điện thoại không được bỏ trống.");
            }
            if ( Student.NumberPhone.Length != 10 ||!Regex.IsMatch(Student.NumberPhone, @"^\d{10}$"))
            {
                return BadRequest("Số điện thoại phải đúng 10 số và chỉ chứa các chữ số.");
            }

            // Kiểm tra Email 
            if (string.IsNullOrWhiteSpace(Student.Email))
            {
                return BadRequest("Email không được trống.");
            }
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(Student.Email, emailPattern))
            {
                return BadRequest("Email phải đúng định dạng.");
            }
            //Kiểm tra Birthday
            if (Student.BirthdayDate == default)
            {
                return BadRequest("Ngày sinh không được bỏ trống.");
            }
            if (Student.BirthdayDate > DateTime.Now)
            {
                return BadRequest("Ngày sinh không được lớn hơn ngày hiện tại.");
            }


            var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var command = new SqlCommand("CreateStudent", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@StudentCode", Student.StudentCode);
            command.Parameters.AddWithValue("@StudentName", Student.StudentName);
            command.Parameters.AddWithValue("@Gender", Student.Gender);
            command.Parameters.AddWithValue("@NumberPhone", Student.NumberPhone);
            command.Parameters.AddWithValue("@Address", Student.Address);
            command.Parameters.AddWithValue("@BirthdayDate", Student.BirthdayDate);
            command.Parameters.AddWithValue("@Email", Student.Email);

            connection.Open();
            var newStudentId = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();

            Student.StudentID = newStudentId;
            return CreatedAtAction(nameof(GetAllStudents), new { id = newStudentId }, new { message = "Thêm thành công!", student = Student });


        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id, Student student)
        {
            // Kiểm tra ID
            if (id <= 0)
            {
                return BadRequest("ID không được bé hơn hoặc bằng 0");
            }
            // Kiểm tra StudentCode
            if (string.IsNullOrWhiteSpace(student.StudentCode))
            {
                return BadRequest("Mã sinh viên không được bỏ trống.");
            }
            if (student.StudentCode.Length > 15)
            {
                return BadRequest("Mã sinh viên không được quá 15 ký tự");
            }


            // Kiểm tra Gender
            if (!student.Gender.HasValue) // Gender là nullable
            {
                return BadRequest("Giới tính không được bỏ trống.");
            }

            // Kiểm tra NumberPhone
            if (string.IsNullOrWhiteSpace(student.NumberPhone) )
            {
                return BadRequest("Số điện thoại không được bỏ trống.");
            }
            if ( student.NumberPhone.Length != 10 ||!Regex.IsMatch(student.NumberPhone, @"^\d{10}$"))
            {
                return BadRequest("Số điện thoại phải đúng 10 số và chỉ chứa các chữ số.");
            }

            // Kiểm tra Email 
            if (string.IsNullOrWhiteSpace(student.Email))
            {
                return BadRequest("Email không được trống.");
            }
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(student.Email, emailPattern))
            {
                return BadRequest("Email phải đúng định dạng.");
            }
            //Kiểm tra Birthday
            if (student.BirthdayDate == default)
            {
                return BadRequest("Ngày sinh không được bỏ trống.");
            }
            if (student.BirthdayDate > DateTime.Now)
            {
                return BadRequest("Ngày sinh không được lớn hơn ngày hiện tại.");
            }
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
            command.Parameters.AddWithValue("@StudentName", student.StudentName);
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


        [HttpDelete("{id}")]
        public IActionResult DeletedStudent(int id)
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

            // Cập nhật sinh viên để đánh dấu đã bị xóa
            command = new SqlCommand("DeleteStudent", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@StudentID", id);

            var rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 0)
            {
                return BadRequest(new { message = "Xóa không thành công, vui lòng kiểm tra lại thông tin." });
            }
            

            return Ok(new { message = "Xóa thành công!" });
        }


    }
}
