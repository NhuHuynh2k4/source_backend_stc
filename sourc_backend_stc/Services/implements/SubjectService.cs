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
                const string query = @"
            EXEC CreateSubject 
                @SubjectsCode, @SubjectsName";

                var parameters = new
                {
                    request.SubjectsCode,
                    request.SubjectsName
                };

                return await connection.ExecuteScalarAsync<int>(query, parameters);
            }
        }

        public async Task<bool> UpdateSubjectAsync(Subject_UpdateReq updateReq)
        {
            // Kiểm tra đầu vào
            var (isValidCode, messageCode) = ErrorHandling.HandleIfEmpty(updateReq.SubjectsCode);
            var (isValidName, messageName) = ErrorHandling.HandleIfEmpty(updateReq.SubjectsName);

            if (!isValidCode || !isValidName)
            {
                return ErrorHandling.HandleError(StatusCodes.Status400BadRequest); // Trả về lỗi nếu dữ liệu không hợp lệ
            }

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Gọi stored procedure để cập nhật môn học
                    var result = await connection.ExecuteAsync(
                        "UpdateSubject", // Tên của stored procedure
                        updateReq, // Truyền đối tượng updateReq chứa các tham số cần thiết
                        commandType: CommandType.StoredProcedure
                    );

                    // Trả về true nếu cập nhật thành công
                    return result > 0;
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu cần thiết
                    Console.WriteLine($"Lỗi khi cập nhật môn học: {ex.Message}");
                    return false; // Trả về false nếu có lỗi xảy ra
                }
            }
        }




        public async Task<bool> DeleteSubjectAsync(int subjectId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            const string query = @"
        EXEC DeleteSubject 
            @SubjectsID = @SubjectsID";

            var parameters = new
            {
                SubjectsID = subjectId
            };

            var result = await connection.ExecuteAsync(query, parameters);
            return result > 0;
        }

    }
}