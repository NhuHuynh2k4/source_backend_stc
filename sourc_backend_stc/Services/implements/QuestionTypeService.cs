using Microsoft.Data.SqlClient;
using sourc_backend_stc.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace sourc_backend_stc.Services
{
    public class QuestionTypeService : IQuestionTypeService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<QuestionTypeService> _logger;

        public QuestionTypeService(IConfiguration configuration, ILogger<QuestionTypeService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // Lấy tất cả QuestionTypes
        public async Task<IEnumerable<QuestionTypeResponse>> GetAllQuestionType()
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                try
                {
                    var result = await connection.QueryAsync<QuestionTypeResponse>(
                        "GetAllQuestionType",
                        commandType: CommandType.StoredProcedure
                    );
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Lỗi khi lấy tất cả QuestionTypes: {ex.Message}");
                    throw new Exception("Lỗi khi lấy danh sách QuestionTypes từ database.");
                }
            }
        }

        // Lấy QuestionType theo ID
        public async Task<QuestionTypeResponse> GetQuestionTypeById(int id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                try
                {
                    var result = await connection.QueryFirstOrDefaultAsync<QuestionTypeResponse>(
                        "GetQuestionTypeById",
                        new { QuestionTypeID = id },
                        commandType: CommandType.StoredProcedure
                    );
                    if (result == null)
                    {
                        _logger.LogWarning($"Không tìm thấy QuestionType với ID: {id}");
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Lỗi khi lấy QuestionType theo ID {id}: {ex.Message}");
                    throw new Exception($"Lỗi khi lấy QuestionType theo ID {id}", ex);
                }
            }
        }

        // Tạo mới QuestionType
        public async Task<bool> CreateQuestionType(QuestionType_CreateReq request)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                try
                {
                    var result = await connection.ExecuteAsync(
                        "CreateQuestionType",
                        new
                        {
                            QuestionTypeCode = request.QuestionTypeCode,
                            QuestionTypeName = request.QuestionTypeName
                        },
                        commandType: CommandType.StoredProcedure
                    );
                    return result > 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Lỗi khi tạo mới QuestionType: {ex.Message}");
                    throw new Exception("Lỗi khi tạo mới QuestionType", ex);
                }
            }
        }

        // Cập nhật QuestionType
        public async Task<bool> UpdateQuestionType(QuestionType_UpdateReq request)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                try
                {
                    var result = await connection.ExecuteAsync(
                        "UpdateQuestionType",
                        new
                        {
                            QuestionTypeID = request.QuestionTypeID,
                            QuestionTypeCode = request.QuestionTypeCode,
                            QuestionTypeName = request.QuestionTypeName
                        },
                        commandType: CommandType.StoredProcedure
                    );
                    return result > 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Lỗi khi cập nhật QuestionType với ID {request.QuestionTypeID}: {ex.Message}");
                    throw new Exception("Lỗi khi cập nhật QuestionType", ex);
                }
            }
        }


        // Xóa QuestionType
        public async Task<bool> DeleteQuestionType(int id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                try
                {
                    var existingQuestionType = await connection.QueryFirstOrDefaultAsync<QuestionType>(
                        "GetQuestionTypeById",
                        new { QuestionTypeID = id },
                        commandType: CommandType.StoredProcedure
                    );
                    if (existingQuestionType == null)
                    {
                        _logger.LogWarning($"Không tìm thấy QuestionType với ID: {id}");
                        return false;
                    }

                    var result = await connection.ExecuteAsync(
                        "DeleteQuestionType",
                        new { QuestionTypeID = id },
                        commandType: CommandType.StoredProcedure
                    );
                    return result > 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Lỗi khi xóa QuestionType với ID {id}: {ex.Message}");
                    throw new Exception("Lỗi khi xóa QuestionType", ex);
                }
            }
        }
    }
}
