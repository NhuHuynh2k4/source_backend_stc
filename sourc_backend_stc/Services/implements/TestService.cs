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
    }
}
