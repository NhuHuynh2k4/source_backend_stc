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

        public async Task<bool> CreateSubjectAsync(Subject_CreateReq request)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var parameters = new DynamicParameters();
            parameters.Add("@SubjectsCode", request.SubjectsCode);
            parameters.Add("@SubjectsName", request.SubjectsName);
            return await connection.ExecuteScalarAsync<bool>("CreateSubject", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> UpdateSubjectAsync(int subjectId, Subject_UpdateReq request)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var result = await connection.ExecuteAsync(
                "UpdateSubject",
                new { SubjectsID = subjectId, request.SubjectsCode, request.SubjectsName },
                commandType: CommandType.StoredProcedure);
            return result > 0;
        }

        public async Task<bool> DeleteSubjectAsync(int subjectId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var result = await connection.ExecuteAsync(
                "DeleteSubject",
                new { SubjectsID = subjectId },
                commandType: CommandType.StoredProcedure);
            return result > 0;
        }
    }
}