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
    public class MarkService : IMarkService
    {
        private readonly IConfiguration _configuration;

        public MarkService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<Mark_ReadAllRes>> GetAllMarks()
        {
            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    var result = await connection.QueryAsync<Mark_ReadAllRes>(
                        "GetAllMark",
                        commandType: CommandType.StoredProcedure
                    );

                    return result; // Trả về danh sách điểm
                }
                catch
                {
                    return Enumerable.Empty<Mark_ReadAllRes>(); // Trả về danh sách trống nếu có lỗi
                }
            }
        }

        public async Task<Mark_ReadAllRes> GetMarkById(int markId)
        {
            var (isValid, errorMessage) = ErrorHandling.ValidateId(markId);
            if (!isValid)
            {
                return null; // Trả về null nếu ID không hợp lệ
            }

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    var result = await connection.QueryFirstOrDefaultAsync<Mark_ReadAllRes>(
                        "GetMarkByID",
                        new { MarkID = markId },
                        commandType: CommandType.StoredProcedure
                    );

                    return result; // Trả về thông tin điểm hoặc null nếu không tìm thấy
                }
                catch
                {
                    return null; // Trả về null nếu có lỗi
                }
            }
        }

        public async Task<bool> CreateMark(Mark_CreateReq markDto)
        {
            var (isValidResult, messageResult) = ErrorHandling.HandleIfEmpty(markDto.Result.ToString());
            var (isValidPassingScore, messagePassingScore) = ErrorHandling.HandleIfEmpty(markDto.PassingScore.ToString());

            if (!isValidResult || !isValidPassingScore)
            {
                return false; // Trả về false nếu dữ liệu không hợp lệ
            }

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    var result = await connection.ExecuteAsync("CreateMark", markDto, commandType: CommandType.StoredProcedure);

                    return result > 0; // Trả về true nếu thành công
                }
                catch
                {
                    return false; // Trả về false nếu có lỗi
                }
            }
        }

        public async Task<bool> UpdateMark(Mark_UpdateReq updateReq)
        {
            if (updateReq == null)
            {
                return false; // Trả về false nếu dữ liệu không hợp lệ
            }

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    var result = await connection.ExecuteAsync(
                        "UpdateMark", updateReq,
                        commandType: CommandType.StoredProcedure
                    );

                    return result > 0; // Trả về true nếu cập nhật thành công
                }
                catch
                {
                    return false; // Trả về false nếu có lỗi
                }
            }
        }

        public async Task<bool> DeleteMark(int markId)
        {
            var (isValid, errorMessage) = ErrorHandling.ValidateId(markId);
            if (!isValid)
            {
                return false; // Trả về false nếu ID không hợp lệ
            }

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    var result = await connection.ExecuteAsync(
                        "DeleteMark",
                        new { MarkID = markId },
                        commandType: CommandType.StoredProcedure
                    );

                    return result==1; // Trả về true nếu cập nhật thành công
                }
                catch
                {
                    return false; // Trả về false nếu có lỗi
                }
            }
        }
    }
}