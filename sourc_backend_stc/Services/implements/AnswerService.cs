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
    public class AnswerService : IAnswerService
{
        private readonly IConfiguration _configuration;

        public AnswerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Các phương thức khác...

        public async Task<IEnumerable<Answer_ReadAllRes>> GetAllAnswers()
        {
            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Gọi stored procedure để lấy tất cả các lớp học không bị xoá
                    var result = await connection.QueryAsync<Answer_ReadAllRes>(
                        "GetAllAnswers", // Tên stored procedure
                        commandType: CommandType.StoredProcedure // Xác định là stored procedure
                    );

                    // Chỉ chọn các cột cần thiết
                    return result.Select(answerInfo => new Answer_ReadAllRes
                    {
                        AnswerID = answerInfo.AnswerID,
                        AnswerName = answerInfo.AnswerName,
                        AnswerTextContent = answerInfo.AnswerTextContent,
                        AnswerImgContent = answerInfo.AnswerImgContent,
                        IsTrue = answerInfo.IsTrue,
                        QuestionID = answerInfo.QuestionID,
                        QuestionName = answerInfo.QuestionName
                    });
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu cần
                    return Enumerable.Empty<Answer_ReadAllRes>(); // Trả về danh sách trống nếu có lỗi
                }
            }
        }


        public async Task<Answer_ReadAllRes> GetAnswerById(int answerId)
        {
            // Kiểm tra ID hợp lệ
            var (isValid, errorMessage) = ErrorHandling.ValidateId(answerId);
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
                    var result = await connection.QueryFirstOrDefaultAsync<Answer_ReadAllRes>(
                        "GetAnswerById",  // Tên stored procedure
                        new { AnswerID = answerId },  // Tham số đầu vào
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


        public async Task<bool> CreateAnswer(Answer_CreateReq answerDto)
        {
            // Kiểm tra đầu vào
            var (isValidAnswerTextContent, messageAnswerTextContent) = ErrorHandling.HandleIfEmpty(answerDto.AnswerTextContent);
            var (isValidAnswerName, messageAnswerName) = ErrorHandling.HandleIfEmpty(answerDto.AnswerName);
            var (isValidQuestionID, messageQuestionID) = ErrorHandling.ValidateId(answerDto.QuestionID);

            if (!isValidAnswerName || !isValidAnswerTextContent || answerDto.IsTrue!=true || answerDto.IsTrue!=false )
            {
                return ErrorHandling.HandleError(StatusCodes.Status400BadRequest); // Trả về lỗi nếu dữ liệu không hợp lệ
            }

            var newAnswer = new Answer_CreateReq
            {
                AnswerName = answerDto.AnswerName,
                AnswerTextContent = answerDto.AnswerTextContent,
                AnswerImgContent = answerDto.AnswerImgContent,
                IsTrue = answerDto.IsTrue,
                QuestionID = answerDto.QuestionID
            };

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Sử dụng Dapper để gọi stored procedure
                    var result = await connection.ExecuteAsync("CreateAnswer", newAnswer, commandType: CommandType.StoredProcedure);

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

        public async Task<bool> DeleteAnswer(int answerId)
        {
            // Kiểm tra ID hợp lệ
            var (isValid, errorMessage) = ErrorHandling.ValidateId(answerId);
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
                        "DeleteAnswer",
                        new { AnswerID = answerId },
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

        public async Task<bool> UpdateAnswer(int answerId, Answer_UpdateReq updateReq)
        {
            // Kiểm tra đầu vào hợp lệ
            if (string.IsNullOrWhiteSpace(updateReq.AnswerTextContent) || string.IsNullOrWhiteSpace(updateReq.AnswerName))
            {
                return false; // Trả về false nếu dữ liệu không hợp lệ
            }

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Gọi stored procedure để cập nhật Answer
                    var result = await connection.ExecuteAsync(
                        "UpdateAnswer",
                        new
                        {
                            AnswerID = answerId,                   // ID lấy từ tham số hàm
                            AnswerName = updateReq.AnswerName,
                            AnswerTextContent = updateReq.AnswerTextContent,     // Lấy dữ liệu từ updateReq
                            AnswerImgContent = updateReq.AnswerImgContent,
                            IsTrue = updateReq.IsTrue,
                            QuestionID = updateReq.QuestionID,
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