using Dapper;
using Microsoft.Data.SqlClient;
using sourc_backend_stc.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using sourc_backend_stc.Utils;
using OfficeOpenXml;

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
        public byte[] ExportQuestionToExcel(List<QuestionReadAllRes> questions)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Question");

            // Tiêu đề các cột
            worksheet.Cells[1, 1].Value = "STT";
            worksheet.Cells[1, 2].Value = "Mã Câu hỏi";
            worksheet.Cells[1, 3].Value = "Tên Câu hỏi";
            worksheet.Cells[1, 4].Value = "Môn Học";
            worksheet.Cells[1, 5].Value = "Loại Câu Hỏi";
            worksheet.Cells[1, 6].Value = "Nội Dung Câu Hỏi";
            worksheet.Cells[1, 7].Value = "Hình Ảnh Câu Hỏi";


            // Định dạng tiêu đề
            using (var headerRange = worksheet.Cells[1, 1, 1, 7])
            {
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Font.Size = 12;
                headerRange.Style.Font.Color.SetColor(System.Drawing.Color.Black); // Black text
                headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White); // White background
                headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                headerRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                headerRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                headerRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                headerRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                headerRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            }
            // Dữ liệu môn học
            for (int i = 0; i < questions.Count; i++)
            {
                var currentQuestions = questions[i];
                worksheet.Cells[i + 2, 1].Value = i + 1;
                worksheet.Cells[i + 2, 2].Value = currentQuestions.QuestionCode;
                worksheet.Cells[i + 2, 3].Value = currentQuestions.QuestionName;
                worksheet.Cells[i + 2, 4].Value = currentQuestions.SubjectsName;
                worksheet.Cells[i + 2, 5].Value = currentQuestions.QuestionName;
                worksheet.Cells[i + 2, 6].Value = currentQuestions.QuestionTextContent;
                worksheet.Cells[i + 2, 7].Value = currentQuestions.QuestionImgContent;
                using (var dataRange = worksheet.Cells[i + 2, 1, i + 2, 7])
                {
                    dataRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    dataRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    dataRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    dataRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    dataRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    dataRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }
            }

            // Tự động điều chỉnh độ rộng cột
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            return package.GetAsByteArray();
        }
    }
}
