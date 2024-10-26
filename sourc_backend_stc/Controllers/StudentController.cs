// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Data.SqlClient;
// using System.Data;
// using sourc_backend_stc.Data;
// using sourc_backend_stc.Models;

// namespace sourc_backend_stc.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class StudentController : ControllerBase
//     {
//         private readonly IConfiguration _config;

//         public StudentController(IConfiguration config)
//         {
//             _config = config;
//         }

//         [HttpGet]
//         public IActionResult GetAllStudents()
//         {
//             var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
//             var command = new SqlCommand("GetAllStudent", connection)
//             {
//                 CommandType = CommandType.StoredProcedure
//             };

//             connection.Open();
//             var reader = command.ExecuteReader();

//             var Students = new List<Student>();
//             while (reader.Read())
//             {
//                 Students.Add(new Student
//                 {
//                     StudentID = reader.GetInt32(0),
//                     StudentCode = reader.GetString(1),
//                     Gender = reader.GetBoolean(2),
//                     NumberPhone = reader.GetString(3),
//                     Address = reader.GetString(4),
//                     BirthdayDate = reader.GetDateTime(5),
//                     CreateDate = reader.GetDateTime(6),
//                     UpdateDate = reader.GetDateTime(7),
//                     IsDelete= reader.GetBoolean(8),
//                     Email = reader.GetString(9)
//                 });
//             }
//             connection.Close();

//             return Ok(Students);
//         }

//         [HttpPost]
//         public IActionResult CreateStudent(Student Student)
//         {
//             var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
//             var command = new SqlCommand("CreateStudent", connection)
//             {
//                 CommandType = CommandType.StoredProcedure
//             };

//             command.Parameters.AddWithValue("@StudentCode", Student.StudentCode);
//             command.Parameters.AddWithValue("@Gender", Student.Gender);
//             command.Parameters.AddWithValue("@NumberPhone", Student.NumberPhone);
//             command.Parameters.AddWithValue("@Address", Student.Address);
//             command.Parameters.AddWithValue("@BirthdayDate", Student.BirthdayDate);
//             command.Parameters.AddWithValue("@Email", Student.Email);

//             connection.Open();
//             var newStudentId = Convert.ToInt32(command.ExecuteScalar());
//             connection.Close();

//             Student.StudentID = newStudentId;
//             return CreatedAtAction(nameof(GetAllStudents), new { id = newStudentId }, Student);
//         }
//     }
// }
