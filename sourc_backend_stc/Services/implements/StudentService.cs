using Microsoft.Data.SqlClient;
using sourc_backend_stc.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using sourc_backend_stc.Utils;
using OfficeOpenXml;

namespace sourc_backend_stc.Services
{
    public class StudentService : IStudentService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<StudentService> _logger;

        public StudentService(IConfiguration configuration, ILogger<StudentService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // Các phương thức khác...

        public async Task<IEnumerable<Student_ReadAllRes>> GetAllStudent()
        {
            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Gọi stored procedure để lấy tất cả các lớp học không bị xoá
                    var result = await connection.QueryAsync<Student_ReadAllRes>(
                        "GetAllStudent", // Tên stored procedure
                        commandType: CommandType.StoredProcedure // Xác định là stored procedure
                    );


                    return result;
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu cần
                    return Enumerable.Empty<Student_ReadAllRes>(); // Trả về danh sách trống nếu có lỗi
                }
            }
        }


        public async Task<Student_ReadAllRes> GetStudentById(int studentId)
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
                    var result = await connection.QueryFirstOrDefaultAsync<Student_ReadAllRes>(
                        "GetStudentByID",  // Tên stored procedure
                        new { StudentID = studentId },  // Tham số đầu vào
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

        public enum CreateStudentResult
        {
            Success,
            DuplicateStudentCode,
            DuplicateEmail,
            DuplicateBirthdayDate,
            InvalidInput,
            Error
        }

        public async Task<CreateStudentResult> CreateStudent(Student_CreateReq createReq)
        {
            // Kiểm tra các trường dữ liệu không được để trống
            var (isValidCode, _) = ErrorHandling.HandleIfEmpty(createReq.StudentCode);
            var (isValidName, _) = ErrorHandling.HandleIfEmpty(createReq.StudentName);
            var (isValidPass, _) = ErrorHandling.HandleIfEmpty(createReq.Password);
            var (isValidNumberphone, _) = ErrorHandling.HandleIfEmpty(createReq.NumberPhone);
            var (isValidAddress, _) = ErrorHandling.HandleIfEmpty(createReq.Address);
            var (isValidEmail, _) = ErrorHandling.HandleIfEmpty(createReq.Email);

            // Nếu có bất kỳ trường nào không hợp lệ, trả về InvalidInput
            if (!isValidCode || !isValidName || !isValidPass || !isValidNumberphone || !isValidAddress || !isValidEmail)
            {
                return CreateStudentResult.InvalidInput;  // Trả về lỗi nếu có trường trống
            }
            
            if(createReq.BirthdayDate > DateTime.Now){
                return CreateStudentResult.DuplicateBirthdayDate;
            }
            
            // Băm mật khẩu trước khi lưu vào cơ sở dữ liệu
            createReq.Password = PasswordHasher.HashPassword(createReq.Password);

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                // Kiểm tra mã sinh viên đã tồn tại hay chưa
                var existingStudentCode = await connection.QueryFirstOrDefaultAsync<int>(
                    "SELECT COUNT(1) FROM Student WHERE StudentCode = @StudentCode",
                    new { createReq.StudentCode });

                if (existingStudentCode > 0)
                    return CreateStudentResult.DuplicateStudentCode; // Trả về lỗi trùng mã sinh viên

                // Kiểm tra email đã tồn tại hay chưa
                var existingEmail = await connection.QueryFirstOrDefaultAsync<int>(
                    "SELECT COUNT(1) FROM Student WHERE Email = @Email",
                    new { createReq.Email });

                if (existingEmail > 0)
                    return CreateStudentResult.DuplicateEmail; // Trả về lỗi trùng email

                // Nếu không có lỗi, thực hiện gọi stored procedure để tạo sinh viên
                try
                {
                    var result = await connection.ExecuteAsync("CreateStudent", createReq, commandType: CommandType.StoredProcedure);
                    return result > 0 ? CreateStudentResult.Success : CreateStudentResult.Error;
                }
                catch (Exception ex)
                {
                    // Ghi lại lỗi nếu có và trả về CreateStudentResult.Error
                    _logger.LogError(ex, "Lỗi khi tạo sinh viên.");
                    return CreateStudentResult.Error; // Trả về lỗi hệ thống
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
public enum UpdateStudentResult
{
    Success,
    StudentNotFound,
    DuplicateStudentCode,
    DuplicateEmail,
    InvalidInput,
    Error
}

       public async Task<UpdateStudentResult> UpdateStudent(Student_UpdateReq updateReq)
{
    // Kiểm tra đầu vào
    if (string.IsNullOrWhiteSpace(updateReq.StudentCode) || 
        string.IsNullOrWhiteSpace(updateReq.StudentName) ||
        string.IsNullOrWhiteSpace(updateReq.NumberPhone) ||
        string.IsNullOrWhiteSpace(updateReq.Address) ||
        string.IsNullOrWhiteSpace(updateReq.Email))
    {
        return UpdateStudentResult.InvalidInput;
    }

    using (var connection = DatabaseConnection.GetConnection(_configuration))
    {
        await connection.OpenAsync();

        try
        {
            // Kiểm tra mã sinh viên và email trùng lặp trước khi cập nhật
            var existingStudentCode = await connection.QueryFirstOrDefaultAsync<int?>(
                "SELECT StudentID FROM Student WHERE StudentCode = @StudentCode AND StudentID != @StudentID",
                new { updateReq.StudentCode, updateReq.StudentID });

            if (existingStudentCode.HasValue)
            {
                return UpdateStudentResult.DuplicateStudentCode;
            }

            var existingEmail = await connection.QueryFirstOrDefaultAsync<int?>(
                "SELECT StudentID FROM Student WHERE Email = @Email AND StudentID != @StudentID",
                new { updateReq.Email, StudentID = updateReq.StudentID });

            if (existingEmail.HasValue)
            {
                return UpdateStudentResult.DuplicateEmail;
            }

            // Gọi stored procedure để cập nhật sinh viên
            var result = await connection.ExecuteAsync(
                "UpdateStudent", updateReq,
                commandType: CommandType.StoredProcedure);

            return result > 0 ? UpdateStudentResult.Success : UpdateStudentResult.StudentNotFound;
        }
        catch (Exception ex)
        {
            // Log lỗi nếu cần thiết
            Console.WriteLine($"Error during student update: {ex.Message}");
            return UpdateStudentResult.Error;
        }
    }
}
public byte[] ExportStudentToExcel(List<Student_ReadAllRes> student)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Student");

                // Đặt tiêu đề cho các cột
                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Mã sinh viên";
                worksheet.Cells[1, 3].Value = "Họ và tên";
                worksheet.Cells[1, 4].Value = "Giới tính";
                worksheet.Cells[1, 5].Value = "Số điện thoại";
                worksheet.Cells[1, 6].Value = "Địa chỉ";
                worksheet.Cells[1, 7].Value = "Email";
                worksheet.Cells[1, 8].Value = "Ngày sinh";
               

                // Dữ liệu lớp học
                for (int i = 0; i < student.Count; i++)
                {
                    var currentStudent = student[i];
                    worksheet.Cells[i + 2, 1].Value = i + 1;
                    worksheet.Cells[i + 2, 2].Value = currentStudent.StudentCode;
                    worksheet.Cells[i + 2, 3].Value = currentStudent.StudentName;
                    worksheet.Cells[i + 2, 4].Value = currentStudent.Gender;
                    worksheet.Cells[i + 2, 5].Value = currentStudent.NumberPhone;
                    worksheet.Cells[i + 2, 6].Value = currentStudent.Address;
                    worksheet.Cells[i + 2, 7].Value = currentStudent.Email;
                    worksheet.Cells[i + 2, 8].Value = currentStudent.BirthdayDate;
                }

                // Trả về byte array của file Excel
                return package.GetAsByteArray();
            }
        }


}
}
