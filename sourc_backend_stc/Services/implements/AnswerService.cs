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
                    return result;
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

            if (!isValidAnswerName || !isValidAnswerTextContent || !isValidQuestionID)
            {
                return ErrorHandling.HandleError(StatusCodes.Status400BadRequest); // Trả về lỗi nếu dữ liệu không hợp lệ
            }

            // var newAnswer = new Answer_CreateReq
            // {
            //     AnswerName = answerDto.AnswerName,
            //     AnswerTextContent = answerDto.AnswerTextContent,
            //     AnswerImgContent = answerDto.AnswerImgContent,
            //     IsTrue = answerDto.IsTrue,
            //     QuestionID = answerDto.QuestionID
            // };

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Sử dụng Dapper để gọi stored procedure
                    var result = await connection.ExecuteAsync("CreateAnswer", answerDto, commandType: CommandType.StoredProcedure);

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

                    return result == 1; // Trả về true nếu cập nhật thành công
                }
                catch (Exception ex)
                {
                    // Xử lý ngoại lệ và ghi log nếu cần thiết
                    return ErrorHandling.HandleError(StatusCodes.Status500InternalServerError); // Trả về lỗi nếu có exception
                }
            }
        }

        public async Task<bool> UpdateAnswer(Answer_UpdateReq updateReq)
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
                        "UpdateAnswer", updateReq,
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

        // public async Task<bool> UploadAnswerImage(int answerId, IFormFile imageFile)
        // {
        //     if (imageFile == null || imageFile.Length == 0)
        //     {
        //         return false; // Return false if no file is provided
        //     }

        //     // Define the path to save the file (adjust the path as needed)
        //     var filePath = Path.Combine("wwwroot/images", $"{answerId}_{imageFile.FileName}");

        //     try
        //     {
        //         // Save the file to the specified path
        //         using (var stream = new FileStream(filePath, FileMode.Create))
        //         {
        //             await imageFile.CopyToAsync(stream);
        //         }

        //         // Optionally, update the database with the file path if needed (not shown here)
        //         return true; 
        //     }
        //     catch (Exception ex)
        //     {
        //         return false; 
        //     }
        // }

    }
}