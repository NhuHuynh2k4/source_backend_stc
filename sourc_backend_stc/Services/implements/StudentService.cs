using Microsoft.Data.SqlClient;
using sourc_backend_stc.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using sourc_backend_stc.Utils;

namespace sourc_backend_stc.Services
{
    public class StudentService : IStudentService
    {
        private readonly IConfiguration _configuration;

        public StudentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Các phương thức khác...

        public async Task<IEnumerable<Student>> GetAllStudent()
        {
            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Gọi stored procedure để lấy tất cả các lớp học không bị xoá
                    var result = await connection.QueryAsync<Student>(
                        "GetAllStudent", // Tên stored procedure
                        commandType: CommandType.StoredProcedure // Xác định là stored procedure
                    );


                    return result.Select(StudentInfo => new Student
                    {
                        StudentID = StudentInfo.StudentID,
                        StudentCode = StudentInfo.StudentCode,
                        StudentName = StudentInfo.StudentName,
                        Gender = StudentInfo.Gender,
                        NumberPhone = StudentInfo.NumberPhone,
                        Address = StudentInfo.Address,
                        Email = StudentInfo.Email,
                        BirthdayDate = StudentInfo.BirthdayDate,
                        UpdateDate = StudentInfo.UpdateDate,
                        CreateDate = StudentInfo.CreateDate,

                    });
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu cần
                    return Enumerable.Empty<Student>(); // Trả về danh sách trống nếu có lỗi
                }
            }
        }


        public async Task<Student> GetStudentById(int studentId)
        {

            var (isValid, errorMessage) = ErrorHandling.ValidateId(studentId);
            if (!isValid)
            {
                return null;
            }

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Sử dụng Dapper để gọi stored procedure
                    var result = await connection.QueryFirstOrDefaultAsync<Student>(
                        "GetStudentByID",  // Tên stored procedure
                        new {StudentID = studentId },  // Tham số đầu vào
                        commandType: CommandType.StoredProcedure  // Xác định là stored procedure
                    );

                    return result; // Trả về thông tin sinh viên hoặc null nếu không tìm thấy
                }
                catch (Exception ex)
                {
                    // Log exception nếu cần thiết
                    return null; // Trả về null nếu có lỗi xảy ra
                }
            }
        }
        public async Task<bool> CreateStudent(Student_CreateReq createReq)
        {
            // Kiểm tra đầu vào
            var (isValidCode, messageCode) = ErrorHandling.HandleIfEmpty(createReq.StudentCode);
            var (isValidName, messageName) = ErrorHandling.HandleIfEmpty(createReq.StudentName);
            var (isValidNumberphone, messageNumberphone) = ErrorHandling.HandleIfEmpty(createReq.NumberPhone);
            var (isValidAddress, messageAddress) = ErrorHandling.HandleIfEmpty(createReq.Address);
            var (isValidEmail, messageEmail) = ErrorHandling.HandleIfEmpty(createReq.Email);

            // Kiểm tra tất cả các trường đầu vào
            if (!isValidCode || !isValidName || !isValidNumberphone || !isValidAddress || !isValidEmail)
            {
                return ErrorHandling.HandleError(StatusCodes.Status400BadRequest); // Trả về lỗi nếu dữ liệu không hợp lệ
            }

            var newStudent = new Student_CreateReq
            {
                StudentCode = createReq.StudentCode,
                StudentName = createReq.StudentName,
                Gender = createReq.Gender,
                NumberPhone = createReq.NumberPhone,
                Address = createReq.Address,
                Email = createReq.Email,
                BirthdayDate = createReq.BirthdayDate
            };

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Sử dụng Dapper để gọi stored procedure
                    var result = await connection.ExecuteAsync("CreateStudent", newStudent, commandType: CommandType.StoredProcedure);

                    if (result > 0)
                    {
                        return true; // Trả về true nếu thành công
                    }
                    else
                    {
                        return ErrorHandling.HandleError(StatusCodes.Status500InternalServerError); // Trả về lỗi nếu thất bại
                    }
                }
                catch (Exception ex) // Bắt exception và ghi log
                {
                    // Ghi log lỗi ở đây
                    Console.WriteLine($"Error occurred: {ex.Message}"); // Ghi log thông tin lỗi
                    return ErrorHandling.HandleError(StatusCodes.Status500InternalServerError); // Trả về lỗi cho exception
                }
            }
        }


        public async Task<bool> DeleteStudent(int studentId)
        {
            // Kiểm tra ID hợp lệ
            var (isValid, errorMessage) = ErrorHandling.ValidateId(studentId);
            if (!isValid)
            {
                return ErrorHandling.HandleError(StatusCodes.Status400BadRequest); // Trả về lỗi nếu ID không hợp lệ
            }

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Đánh dấu IsDeleted là true cho bản ghi có ID tương ứng
                    var result = await connection.ExecuteAsync(
                        "DeleteStudent",
                        new { StudentID = studentId },
                        commandType: CommandType.StoredProcedure
                    );

                    return result > 0; // Trả về true nếu cập nhật thành công
                }
                catch (Exception ex)
                {
                    // Xử lý ngoại lệ và ghi log nếu cần thiết
                    return ErrorHandling.HandleError(StatusCodes.Status500InternalServerError); // Trả về lỗi nếu có exception
                }
            }
        }

        public async Task<bool> UpdateStudent(int studentId, Student_UpdateReq updateReq)
        {
            // var (isValid, errorMessage) = ErrorHandling.ValidateId(studentId);
            // if (!isValid)
            // {
            //     return ErrorHandling.HandleError(StatusCodes.Status400BadRequest); // Trả về lỗi nếu ID không hợp lệ
            // }
            // Kiểm tra đầu vào
            var (isValidCode, messageCode) = ErrorHandling.HandleIfEmpty(updateReq.StudentCode);
            var (isValidName, messageName) = ErrorHandling.HandleIfEmpty(updateReq.StudentName);
            var (isValidNumberphone, messageNumberphone) = ErrorHandling.HandleIfEmpty(updateReq.NumberPhone);
            var (isValidAddress, messageAddress) = ErrorHandling.HandleIfEmpty(updateReq.Address);
            var (isValidEmail, messageEmail) = ErrorHandling.HandleIfEmpty(updateReq.Email);

           if (!isValidCode || !isValidName || !isValidNumberphone || !isValidAddress || !isValidEmail)

            {
                return ErrorHandling.HandleError(StatusCodes.Status400BadRequest); // Trả về lỗi nếu dữ liệu không hợp lệ
            }

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Gọi stored procedure để cập nhật lớp học
                    var result = await connection.ExecuteAsync(
                        "UpdateStudent",
                        new
                        {
                            StudentID = studentId,                   // ID lấy từ tham số hàm
                            StudentCode = updateReq.StudentCode,     // Lấy dữ liệu từ updateReq
                            StudentName = updateReq.StudentName,
                            Gender = updateReq.Gender,
                            NumberPhone = updateReq.NumberPhone,
                            Address = updateReq.Address,
                            Email = updateReq.Email,
                            BirthdayDate = updateReq.BirthdayDate


                        },
                        commandType: CommandType.StoredProcedure
                    );

                    return result > 0; // Trả về true nếu cập nhật thành công
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu cần
                    return false; // Trả về false nếu có lỗi xảy ra
                }
            }
        }
    }


}
