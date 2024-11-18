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

        public QuestionService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<QuestionReadAllRes>> GetAllQuestions()
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                return await connection.QueryAsync<QuestionReadAllRes>("GetAllQuestion", commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<QuestionReadAllRes> GetQuestionById(int questionId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                return await connection.QueryFirstOrDefaultAsync<QuestionReadAllRes>(
                "GetQuestionById", 
                new { QuestionID = questionId },
                commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> CreateQuestion(Question_CreateReq request)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                return await connection.ExecuteAsync("CreateQuestion", request, commandType: CommandType.StoredProcedure);
            }
        }


        public async Task<bool> UpdateQuestion(Question_UpdateReq request)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var rowsAffected = await connection.ExecuteAsync("UpdateQuestion", request, commandType: CommandType.StoredProcedure);
                return rowsAffected > 0;
            }
        }


        public async Task<bool> DeleteQuestion(int questionId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var parameters = new { QuestionID = questionId };

                var rowsAffected = await connection.ExecuteAsync("DeleteQuestion", parameters, commandType: CommandType.StoredProcedure);
                return rowsAffected > 0;
            }
        }

    }
}
