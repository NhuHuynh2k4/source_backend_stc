using Dapper;
using Microsoft.Data.SqlClient;
using sourc_backend_stc.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using sourc_backend_stc.Utils;
namespace sourc_backend_stc.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public QuestionService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<QuestionReadAllRes>> GetAllQuestions()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                const string query = "EXEC GetAllQuestion";
                return (await connection.QueryAsync<QuestionReadAllRes>(query)).ToList();
            }
        }

        public async Task<QuestionReadAllRes> GetQuestionById(int questionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                const string query = "EXEC GetQuestionById @QuestionID";
                return await connection.QueryFirstOrDefaultAsync<QuestionReadAllRes>(query, new { QuestionID = questionId });
            }
        }

        public async Task<int> CreateQuestion(Question_CreateReq createReq)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                const string query = @"
            EXEC CreateQuestion 
                @QuestionCode = @QuestionCode, 
                @QuestionName = @QuestionName, 
                @QuestionTextContent = @QuestionTextContent, 
                @QuestionImgContent = @QuestionImgContent, 
                @SubjectsID = @SubjectsID, 
                @QuestionTypeID = @QuestionTypeID";

                var parameters = new
                {
                    createReq.QuestionCode,
                    createReq.QuestionName,
                    createReq.QuestionTextContent,
                    createReq.QuestionImgContent,
                    createReq.SubjectsID,
                    createReq.QuestionTypeID
                };

                return await connection.ExecuteScalarAsync<int>(query, parameters);
            }
        }


        public async Task<bool> UpdateQuestion(Question_UpdateReq updateReq)
        {
            // Kiểm tra dữ liệu đầu vào
            (bool isValidCode, string messageCode) = ErrorHandling.HandleIfEmpty(updateReq.QuestionCode);
            (bool isValidName, string messageName) = ErrorHandling.HandleIfEmpty(updateReq.QuestionName);
            (bool isValidContent, string messageContent) = ErrorHandling.HandleIfEmpty(updateReq.QuestionTextContent);

            if (!isValidCode || !isValidName || !isValidContent || updateReq.SubjectsID <= 0 || updateReq.QuestionTypeID <= 0)
            {
                Console.WriteLine("Dữ liệu đầu vào không hợp lệ");
                return false;
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    const string query = @"
                    EXEC UpdateQuestion 
                        @QuestionID = @QuestionID, 
                        @QuestionCode = @QuestionCode, 
                        @QuestionName = @QuestionName, 
                        @QuestionTextContent = @QuestionTextContent, 
                        @QuestionImgContent = @QuestionImgContent, 
                        @SubjectsID = @SubjectsID, 
                        @QuestionTypeID = @QuestionTypeID";

                    var parameters = new
                    {
                        updateReq.QuestionID,
                        updateReq.QuestionCode,
                        updateReq.QuestionName,
                        updateReq.QuestionTextContent,
                        updateReq.QuestionImgContent,
                        updateReq.SubjectsID,
                        updateReq.QuestionTypeID
                    };

                    var rowsAffected = await connection.ExecuteAsync(query, parameters, commandType: CommandType.Text);
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật câu hỏi: {ex.Message}");
                return false;
            }
        }




        public async Task<bool> DeleteQuestion(int questionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                const string query = "EXEC DeleteQuestion @QuestionID = @QuestionID";
                var parameters = new { QuestionID = questionId };

                var rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
        }

    }
}
