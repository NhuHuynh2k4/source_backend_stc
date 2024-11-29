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
    public class Exam_StudentService : IExam_StudentService
    {
        private readonly IConfiguration _configuration;

        public Exam_StudentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Các phương thức khác...

        public async Task<IEnumerable<Exam_StudentRes>> GetAllExam_Student()
        {
            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Gọi stored procedure để lấy tất cả các lớp học không bị xoá
                    var result = await connection.QueryAsync<Exam_StudentRes>(
                        "GetAllExam_StudentJOINStudent", // Tên stored procedure
                        commandType: CommandType.StoredProcedure // Xác định là stored procedure
                    );


                    return result.Select(Exam_StudentInfo => new Exam_StudentRes
                    {

                        Exam_StudentID = Exam_StudentInfo.Exam_StudentID,
                        ExamID = Exam_StudentInfo.ExamID,
                        ExamCode = Exam_StudentInfo.ExamCode,
                        ExamName = Exam_StudentInfo.ExamName,
                        Duration = Exam_StudentInfo.Duration,
                        NumberOfQuestions = Exam_StudentInfo.NumberOfQuestions,
                        TotalMarks = Exam_StudentInfo.TotalMarks,
                        StudentID = Exam_StudentInfo.StudentID,
                        StudentCode = Exam_StudentInfo.StudentCode,
                        StudentName = Exam_StudentInfo.StudentName,
                        Gender = Exam_StudentInfo.Gender,
                        NumberPhone = Exam_StudentInfo.NumberPhone,
                        Address = Exam_StudentInfo.Address,
                        Email = Exam_StudentInfo.Email,
                        BirthdayDate = Exam_StudentInfo.BirthdayDate,
                        UpdateDate = Exam_StudentInfo.UpdateDate,
                        CreateDate = Exam_StudentInfo.CreateDate,

                    });
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu cần
                    return Enumerable.Empty<Exam_StudentRes>(); // Trả về danh sách trống nếu có lỗi
                }
            }
        }


        public async Task<Exam_StudentRes> GetExam_StudentById(int Exam_StudentId)
        {

            var (isValid, errorMessage) = ErrorHandling.ValidateId(Exam_StudentId);
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
                    var result = await connection.QueryFirstOrDefaultAsync<Exam_StudentRes>(
                        "GetExam_StudentByID",  // Tên stored procedure
                        new { Exam_StudentID = Exam_StudentId },  // Tham số đầu vào
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
        public async Task<bool> CreateExam_Student(Exam_Student_CreateReq createReq)
        {
            // Kiểm tra đầu vào
            var (isValidStudentID, messageStudentID) = ErrorHandling.ValidateId(createReq.StudentID);
            var (isValidExamID, messageExamID) = ErrorHandling.ValidateId(createReq.ExamID);


            // Kiểm tra tất cả các trường đầu vào
            if (!isValidStudentID || !isValidExamID)
            {
                return ErrorHandling.HandleError(StatusCodes.Status400BadRequest); // Trả về lỗi nếu dữ liệu không hợp lệ
            }

            var newExam_Student = new Exam_Student_CreateReq
            {
                ExamID = createReq.ExamID,
                StudentID = createReq.StudentID

            };

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Sử dụng Dapper để gọi stored procedure
                    var result = await connection.ExecuteAsync("CreateExam_Student", newExam_Student, commandType: CommandType.StoredProcedure);

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


        public async Task<bool> DeleteExam_Student(int Exam_StudentId)
        {
            // Kiểm tra ID hợp lệ
            var (isValid, errorMessage) = ErrorHandling.ValidateId(Exam_StudentId);
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
                        "DeleteExam_Student",
                        new { Exam_StudentID = Exam_StudentId },
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

        public async Task<bool> UpdateExam_Student(int Exam_StudentId, Exam_Student_UpdateReq updateReq)
        {
            var (isValidStudentID, messageStudentID) = ErrorHandling.ValidateId(updateReq.StudentID);
            var (isValidExamID, messageExamID) = ErrorHandling.ValidateId(updateReq.ExamID);


            // Kiểm tra tất cả các trường đầu vào
            if (!isValidStudentID || !isValidExamID)
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
                        "UpdateExam_Student",
                        new
                        {
                            Exam_StudentID = Exam_StudentId,             // ID lấy từ tham số hàm
                            ExamID = updateReq.ExamID,                  // Lấy dữ liệu từ updateReq
                            StudentID = updateReq.StudentID
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
