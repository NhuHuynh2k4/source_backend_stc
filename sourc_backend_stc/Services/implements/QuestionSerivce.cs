using Dapper;
using Microsoft.Data.SqlClient;
using sourc_backend_stc.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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

        public async Task<int> CreateQuestion(Question_CreateReq request)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                const string query = @"
                    EXEC CreateQuestion 
                        @QuestionCode, @QuestionName, 
                        @QuestionTextContent, @QuestionImgContent, 
                        @SubjectsID, @QuestionTypeID";

                var parameters = new
                {
                    request.QuestionCode,
                    request.QuestionName,
                    request.QuestionTextContent,
                    request.QuestionImgContent,
                    request.SubjectsID,
                    request.QuestionTypeID
                };

                return await connection.ExecuteScalarAsync<int>(query, parameters);
            }
        }

        public async Task<bool> UpdateQuestion(int questionId, Question_UpdateReq request)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                const string query = @"
                    EXEC UpdateQuestion 
                        @QuestionID, @QuestionCode, @QuestionName, 
                        @QuestionTextContent, @QuestionImgContent, 
                        @SubjectsID, @QuestionTypeID";

                var parameters = new
                {
                    QuestionID = questionId,
                    request.QuestionCode,
                    request.QuestionName,
                    request.QuestionTextContent,
                    request.QuestionImgContent,
                    request.SubjectsID,
                    request.QuestionTypeID
                };

                var rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
        }

        public async Task<bool> DeleteQuestion(int questionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                const string query = "EXEC DeleteQuestion @QuestionID";
                var rowsAffected = await connection.ExecuteAsync(query, new { QuestionID = questionId });
                return rowsAffected > 0;
            }
        }
    }
}
