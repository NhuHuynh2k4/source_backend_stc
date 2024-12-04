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
    public class SubjectService : ISubjectService
    {
        private readonly IConfiguration _configuration;

        public SubjectService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Lấy danh sách tất cả môn học.
        /// </summary>
        public async Task<IEnumerable<SubjectReadAllRes>> GetAllSubjectsAsync()
        {
            using var connection = DatabaseConnection.GetConnection(_configuration);
            return await connection.QueryAsync<SubjectReadAllRes>(
                "GetAllSubjects",
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Lấy thông tin môn học theo ID.
        /// </summary>
        public async Task<SubjectReadAllRes> GetSubjectByIdAsync(int subjectId)
        {
            using var connection = DatabaseConnection.GetConnection(_configuration);
            return await connection.QueryFirstOrDefaultAsync<SubjectReadAllRes>(
                "GetSubjectById",
                new { SubjectsID = subjectId },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Tạo môn học mới.
        /// </summary>
        public async Task<int> CreateSubjectAsync(Subject_CreateReq request)
        {
            using var connection = DatabaseConnection.GetConnection(_configuration);

            const string query = "EXEC CreateSubject @SubjectsCode, @SubjectsName";
            var parameters = new
            {
                request.SubjectsCode,
                request.SubjectsName
            };

            return await connection.ExecuteScalarAsync<int>(query, parameters);
        }

        /// <summary>
        /// Cập nhật thông tin môn học.
        /// </summary>
        public async Task<bool> UpdateSubjectAsync(Subject_UpdateReq updateReq)
        {
            // Kiểm tra dữ liệu nhập
            var (isValidCode, messageCode) = ErrorHandling.HandleIfEmpty(updateReq.SubjectsCode);
            var (isValidName, messageName) = ErrorHandling.HandleIfEmpty(updateReq.SubjectsName);

            if (!isValidCode || !isValidName)
            {
                ErrorHandling.HandleError(StatusCodes.Status400BadRequest);
                return false;
            }

            using var connection = DatabaseConnection.GetConnection(_configuration);
            await connection.OpenAsync();

            try
            {
                var result = await connection.ExecuteAsync(
                    "UpdateSubject",
                    updateReq,
                    commandType: CommandType.StoredProcedure);

                return result > 0; // Thành công nếu kết quả > 0
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật môn học: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Xóa môn học theo ID.
        /// </summary>
        public async Task<bool> DeleteSubjectAsync(int subjectId)
        {
            using var connection = DatabaseConnection.GetConnection(_configuration);

            const string query = "EXEC DeleteSubject @SubjectsID";
            var parameters = new { SubjectsID = subjectId };

            var result = await connection.ExecuteAsync(query, parameters);
            return result > 0;
        }

        /// <summary>
        /// Xuất danh sách môn học ra file Excel.
        /// </summary>
        public byte[] ExportSubjectsToExcel(List<SubjectReadAllRes> subjects)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Subjects");

            // Tiêu đề các cột
            worksheet.Cells[1, 1].Value = "STT";
            worksheet.Cells[1, 2].Value = "Mã Môn Học";
            worksheet.Cells[1, 3].Value = "Tên Môn Học";

            // Định dạng tiêu đề
            using (var headerRange = worksheet.Cells[1, 1, 1, 3])
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
            for (int i = 0; i < subjects.Count; i++)
            {
                var currentSubject = subjects[i];
                worksheet.Cells[i + 2, 1].Value = i + 1;
                worksheet.Cells[i + 2, 2].Value = currentSubject.SubjectsCode;
                worksheet.Cells[i + 2, 3].Value = currentSubject.SubjectsName;
                using (var dataRange = worksheet.Cells[i + 2, 1, i + 2, 3])
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
