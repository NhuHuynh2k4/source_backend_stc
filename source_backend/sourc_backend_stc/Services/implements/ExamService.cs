using Microsoft.Data.SqlClient;
using sourc_backend_stc.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using sourc_backend_stc.Utils;
using OfficeOpenXml;

namespace sourc_backend_stc.Services
{
    public class ExamService : IExamService
    {
        private readonly IConfiguration _configuration;
        private readonly TestService _testService;

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
                        "UpdateExam", updateReq,
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

                    return result == 1; // Trả về true nếu cập nhật thành công
                }
                catch
                {
                    return false; // Trả về false nếu có lỗi
                }
            }
        }

        public byte[] ExportExamsToExcel(List<Exam_ReadAllRes> exams)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Exams");

                // Đặt tiêu đề cho các cột
                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Mã kỳ thi";
                worksheet.Cells[1, 3].Value = "Tên kỳ thi";
                worksheet.Cells[1, 4].Value = "Ngày thi";
                worksheet.Cells[1, 5].Value = "Thời gian thi";
                worksheet.Cells[1, 6].Value = "Tổng điểm";
                worksheet.Cells[1, 7].Value = "Số lượng câu hỏi";
                worksheet.Cells[1, 8].Value = "Bài thi";

                // Dữ liệu lớp học
                for (int i = 0; i < exams.Count; i++)
                {
                    var currentExam = exams[i];
                    worksheet.Cells[i + 2, 1].Value = i + 1;
                    worksheet.Cells[i + 2, 2].Value = currentExam.ExamCode;
                    worksheet.Cells[i + 2, 3].Value = currentExam.ExamName;
                    worksheet.Cells[i + 2, 4].Value = currentExam.ExamDate.ToString("dd-MM-yyyy");
                    worksheet.Cells[i + 2, 5].Value = currentExam.Duration;
                    worksheet.Cells[i + 2, 6].Value = currentExam.TotalMarks;
                    worksheet.Cells[i + 2, 7].Value = currentExam.NumberOfQuestions;

                    // Sử dụng await để gọi hàm bất đồng bộ GetTestNameById
                    worksheet.Cells[i + 2, 8].Value = currentExam.TestID;
                }

                // Trả về byte array của file Excel
                return package.GetAsByteArray();
            }
        }

    }
}