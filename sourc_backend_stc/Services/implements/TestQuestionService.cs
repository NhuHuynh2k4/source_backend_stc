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
    public class TestQuestionService : ITestQuestionService
{
        private readonly IConfiguration _configuration;

        public TestQuestionService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Các phương thức khác...

        public async Task<IEnumerable<TestQuestion_ReadAllRes>> GetAllTestQuestions()
        {
            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Gọi stored procedure để lấy tất cả các lớp học không bị xoá
                    var result = await connection.QueryAsync<TestQuestion_ReadAllRes>(
                        "GetAllTestQuestions", // Tên stored procedure
                        commandType: CommandType.StoredProcedure // Xác định là stored procedure
                    );

                    // Chỉ chọn các cột cần thiết mà không bao gồm IsDelete
                    return result.Select(testQuestionInfo => new TestQuestion_ReadAllRes
                    {
                        Test_QuestionID = testQuestionInfo.Test_QuestionID,
                        QuestionID = testQuestionInfo.QuestionID,
                        QuestionName = testQuestionInfo.QuestionName,
                        TestID = testQuestionInfo.TestID,
                        TestName = testQuestionInfo.TestName
                    });
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu cần
                    return Enumerable.Empty<TestQuestion_ReadAllRes>(); // Trả về danh sách trống nếu có lỗi
                }
            }
        }


        public async Task<TestQuestion_ReadAllRes> GetTestQuestionById(int testQuestionId)
        {
            // Kiểm tra ID hợp lệ
            var (isValid, errorMessage) = ErrorHandling.ValidateId(testQuestionId);
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
                    var result = await connection.QueryFirstOrDefaultAsync<TestQuestion_ReadAllRes>(
                        "GetTestQuestionById",  // Tên stored procedure
                        new { Test_QuestionID = testQuestionId },  // Tham số đầu vào
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


        public async Task<bool> CreateTestQuestion(TestQuestion_CreateReq testQuestionDto)
        {
            // Kiểm tra đầu vào
            var (isValidQuestionID, messageQuestionID) = ErrorHandling.ValidateId(testQuestionDto.QuestionID);
            var (isValidTestID, messageTestID) = ErrorHandling.ValidateId(testQuestionDto.TestID);

            if (!isValidQuestionID || !isValidTestID)
            {
                return ErrorHandling.HandleError(StatusCodes.Status400BadRequest); // Trả về lỗi nếu dữ liệu không hợp lệ
            }

            var newTestQuestion = new TestQuestion_CreateReq
            {
                
                QuestionID = testQuestionDto.QuestionID,
                TestID = testQuestionDto.TestID
            };

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Sử dụng Dapper để gọi stored procedure
                    var result = await connection.ExecuteAsync("CreateTestQuestion", newTestQuestion, commandType: CommandType.StoredProcedure);

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

        public async Task<bool> DeleteTestQuestion(int testQuestionId)
        {
            // Kiểm tra ID hợp lệ
            var (isValid, errorMessage) = ErrorHandling.ValidateId(testQuestionId);
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
                        "DeleteTestQuestion",
                        new { Test_QuestionID = testQuestionId },
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

        public async Task<bool> UpdateTestQuestion(int testQuestionId, TestQuestion_UpdateReq updateReq)
        {
            // Kiểm tra đầu vào hợp lệ
            if (updateReq.QuestionID <= 0 || updateReq.TestID <=0)
            {
                return false; // Trả về false nếu dữ liệu không hợp lệ
            }

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Gọi stored procedure để cập nhật TestQuestion
                    var result = await connection.ExecuteAsync(
                        "UpdateTestQuestion",
                        new
                        {
                            Test_QuestionID = testQuestionId,
                            QuestionID = updateReq.QuestionID,
                            TestID = updateReq.TestID
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