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
    public class TestService : ITestService
    {
        private readonly IConfiguration _configuration;

        public TestService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Các phương thức khác...

        public async Task<IEnumerable<Test_ReadAllRes>> GetAllTests()
        {
            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Gọi stored procedure để lấy tất cả các lớp học không bị xoá
                    var result = await connection.QueryAsync<Test_ReadAllRes>(
                        "GetAllTest", // Tên stored procedure
                        commandType: CommandType.StoredProcedure // Xác định là stored procedure
                    );

                    // Chỉ chọn các cột cần thiết mà không bao gồm IsDelete
                    return result;
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu cần
                    return Enumerable.Empty<Test_ReadAllRes>(); // Trả về danh sách trống nếu có lỗi
                }
            }
        }


        public async Task<Test_ReadAllRes> GetTestById(int testId)
        {
            // Kiểm tra ID hợp lệ
            var (isValid, errorMessage) = ErrorHandling.ValidateId(testId);
            if (!isValid)
            {
                return null; // Trả về null nếu ID không hợp lệ
            }

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Sử dụng Dapper để gọi stored procedure
                    var result = await connection.QueryFirstOrDefaultAsync<Test_ReadAllRes>(
                        "GetTestByID",  // Tên stored procedure
                        new { TestID = testId },  // Tham số đầu vào
                        commandType: CommandType.StoredProcedure  // Xác định là stored procedure
                    );

                    return result; // Trả về thông tin lớp học hoặc null nếu không tìm thấy
                }
                catch (Exception ex)
                {
                    // Log exception nếu cần thiết
                    return null; // Trả về null nếu có lỗi xảy ra
                }
            }
        }


        public async Task<bool> CreateTest(Test_CreateReq testDto)
        {
            // Kiểm tra đầu vào
            var (isValidCode, messageCode) = ErrorHandling.HandleIfEmpty(testDto.TestCode);
            var (isValidName, messageName) = ErrorHandling.HandleIfEmpty(testDto.TestName);

            if (!isValidCode || !isValidName)
            {
                return ErrorHandling.HandleError(StatusCodes.Status400BadRequest); // Trả về lỗi nếu dữ liệu không hợp lệ
            }


            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Sử dụng Dapper để gọi stored procedure
                    var result = await connection.ExecuteAsync("CreateTest", testDto, commandType: CommandType.StoredProcedure);

                    if (result > 0)
                    {
                        return true; // Trả về true nếu thành công
                    }
                    else
                    {
                        return ErrorHandling.HandleError(StatusCodes.Status500InternalServerError); // Trả về lỗi nếu thất bại
                    }
                }
                catch (Exception)
                {
                    // Xử lý và ghi log lỗi nếu có
                    return ErrorHandling.HandleError(StatusCodes.Status500InternalServerError); // Trả về lỗi cho exception
                }
            }
        }

        public async Task<bool> DeleteTest(int testId)
        {
            // Kiểm tra ID hợp lệ
            var (isValid, errorMessage) = ErrorHandling.ValidateId(testId);
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
                        "DeleteTest",
                        new { TestID = testId },
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

        public async Task<bool> UpdateTest(Test_UpdateReq updateReq)
        {
            // Kiểm tra đầu vào hợp lệ
            if (string.IsNullOrWhiteSpace(updateReq.TestCode) || string.IsNullOrWhiteSpace(updateReq.TestName))
            {
                return false; // Trả về false nếu dữ liệu không hợp lệ
            }

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Gọi stored procedure để cập nhật lớp học
                    var result = await connection.ExecuteAsync(
                        "UpdateTest", updateReq,
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

        public async Task<IEnumerable<Test_ReadAllRes>> SearchTests(string testCode = null, string testName = null, string numberOfQuestions = null, string subjectName = null)
        {
            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Tạo câu truy vấn SQL động tùy thuộc vào các tham số tìm kiếm
                    var query = "SELECT * FROM Tests WHERE 1=1";  // Lọc tất cả các lớp, 1=1 là điều kiện luôn đúng

                    // Các tham số tìm kiếm
                    if (!string.IsNullOrEmpty(testCode))
                    {
                        query += " AND TestCode LIKE @TestCode";
                    }
                    if (!string.IsNullOrEmpty(testName))
                    {
                        query += " AND TestName LIKE @TestName";
                    }
                    if (!string.IsNullOrEmpty(numberOfQuestions))
                    {
                        query += " AND NumberOfQuestions LIKE @NumberOfQuestions";
                    }
                    if (!string.IsNullOrEmpty(subjectName))
                    {
                        query += " AND SubjectsName LIKE @SubjectsName";
                    }

                    // Thực thi câu truy vấn
                    var result = await connection.QueryAsync<Test_ReadAllRes>(
                        query,
                        new { TestCode = $"%{testCode}%", TestName = $"%{testName}%", NumberOfQuestions = $"%{numberOfQuestions}%", SubjectsName = $"%{subjectName}%" }
                    );

                    return result; // Trả về kết quả tìm kiếm
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu có
                    return Enumerable.Empty<Test_ReadAllRes>(); // Trả về danh sách trống nếu có lỗi
                }
            }
        }

        public byte[] ExportTestsToExcel(List<Test_ReadAllRes> tests)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Tests");

                // Đặt tiêu đề cho các cột
                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Mã bài kiểm tra";
                worksheet.Cells[1, 3].Value = "Tên bài kiểm tra";
                worksheet.Cells[1, 4].Value = "Số lượng câu hỏi";
                worksheet.Cells[1, 5].Value = "Chủ đề";

                // Dữ liệu lớp học
                for (int i = 0; i < tests.Count; i++)
                {
                    var currentTest = tests[i];
                    worksheet.Cells[i + 2, 1].Value = i + 1;
                    worksheet.Cells[i + 2, 2].Value = currentTest.TestCode;
                    worksheet.Cells[i + 2, 3].Value = currentTest.TestName;
                    worksheet.Cells[i + 2, 4].Value = currentTest.NumberOfQuestions;
                    worksheet.Cells[i + 2, 5].Value = currentTest.SubjectsName;
                }

                // Trả về byte array của file Excel
                return package.GetAsByteArray();
            }
        }

        // Phương thức để lấy TestName từ TestID
        public async Task<string> GetTestNameById(int testId)
        {
            // Kiểm tra ID hợp lệ
            var (isValid, errorMessage) = ErrorHandling.ValidateId(testId);
            if (!isValid)
            {
                return null; // Trả về null nếu ID không hợp lệ
            }

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Dùng Dapper để lấy tên bài kiểm tra từ cơ sở dữ liệu
                    var result = await connection.QueryFirstOrDefaultAsync<string>(
                        "SELECT TestName FROM Test WHERE TestID = @TestID",
                        new { TestID = testId }
                    );

                    return result; // Trả về tên bài kiểm tra
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu cần thiết
                    return null; // Trả về null nếu có lỗi xảy ra
                }
            }
        }
    }
}
