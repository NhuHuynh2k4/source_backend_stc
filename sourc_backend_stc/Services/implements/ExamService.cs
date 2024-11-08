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
    public class ExamService : IExamService
    {
        private readonly IConfiguration _configuration;

        public ExamService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<Exam_ReadAllRes>> GetAllExams()
        {
            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    var result = await connection.QueryAsync<Exam_ReadAllRes>(
                        "GetAllExam",
                        commandType: CommandType.StoredProcedure
                    );

                    return result; // Trả về danh sách kỳ thi
                }
                catch
                {
                    return Enumerable.Empty<Exam_ReadAllRes>(); // Trả về danh sách trống nếu có lỗi
                }
            }
        }

        public async Task<Exam_ReadAllRes> GetExamById(int examId)
        {
            var (isValid, errorMessage) = ErrorHandling.ValidateId(examId);
            if (!isValid)
            {
                return null; // Trả về null nếu ID không hợp lệ
            }

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    var result = await connection.QueryFirstOrDefaultAsync<Exam_ReadAllRes>(
                        "GetExamByID",
                        new { ExamID = examId },
                        commandType: CommandType.StoredProcedure
                    );

                    return result; // Trả về thông tin kỳ thi hoặc null nếu không tìm thấy
                }
                catch
                {
                    return null; // Trả về null nếu có lỗi
                }
            }
        }

        public async Task<bool> CreateExam(Exam_CreateReq examDto)
        {
            if (string.IsNullOrWhiteSpace(examDto.ExamCode) || string.IsNullOrWhiteSpace(examDto.ExamName))
            {
                return false; // Trả về false nếu dữ liệu không hợp lệ
            }

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    var result = await connection.ExecuteAsync("CreateExam", examDto, commandType: CommandType.StoredProcedure);

                    return result > 0; // Trả về true nếu thành công
                }
                catch
                {
                    return false; // Trả về false nếu có lỗi
                }
            }
        }

        public async Task<bool> UpdateExam(Exam_UpdateReq updateReq)
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
                        "UpdateExam",updateReq,
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

        public async Task<bool> DeleteExam(int examId)
        {
            var (isValid, errorMessage) = ErrorHandling.ValidateId(examId);
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
                        "DeleteExam",
                        new { ExamID = examId },
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