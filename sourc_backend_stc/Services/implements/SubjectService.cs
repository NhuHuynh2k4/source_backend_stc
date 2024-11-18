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
    public class SubjectService : ISubjectService
    {
        private readonly IConfiguration _configuration;
        public SubjectService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<SubjectReadAllRes>> GetAllSubjectsAsync()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return await connection.QueryAsync<SubjectReadAllRes>("GetAllSubjects", commandType: CommandType.StoredProcedure);
        }

        public async Task<SubjectReadAllRes> GetSubjectByIdAsync(int subjectId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return await connection.QueryFirstOrDefaultAsync<SubjectReadAllRes>(
                "GetSubjectById",
                new { SubjectsID = subjectId },
                commandType: CommandType.StoredProcedure);
        }
        public async Task<int> CreateSubjectAsync(Subject_CreateReq request)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {

                return await connection.ExecuteAsync("CreateSubject", request, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<bool> UpdateSubjectAsync(Subject_UpdateReq request)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        
            var result = await connection.ExecuteAsync("UpdateSubject", request, commandType: CommandType.StoredProcedure);
            return result > 0;
        }


        public async Task<bool> DeleteSubjectAsync(int subjectId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var parameters = new{ SubjectsID = subjectId };

            var result = await connection.ExecuteAsync("DeleteSubject", parameters, commandType: CommandType.StoredProcedure);
            return result > 0;
        }

    }
}