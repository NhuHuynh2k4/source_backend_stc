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
    public class ClassService : IClassService
    {
        private readonly IConfiguration _configuration;

        public ClassService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Các phương thức khác...

        public async Task<IEnumerable<Class_ReadAllRes>> GetAllClasses()
        {
            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Gọi stored procedure để lấy tất cả các lớp học không bị xoá
                    var result = await connection.QueryAsync<Class_ReadAllRes>(
                        "GetAllClass", // Tên stored procedure
                        commandType: CommandType.StoredProcedure // Xác định là stored procedure
                    );

                    return result;
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu cần
                    return Enumerable.Empty<Class_ReadAllRes>(); // Trả về danh sách trống nếu có lỗi
                }
            }
        }


        public async Task<Class_ReadAllRes> GetClassById(int classId)
        {
            // Kiểm tra ID hợp lệ
            var (isValid, errorMessage) = ErrorHandling.ValidateId(classId);
            if (!isValid)
            {
                return null; // Trả về null nếu ID không hợp lệ
            }

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Sử dụng Dapper để gọi stored procedure
                    var result = await connection.QueryFirstOrDefaultAsync<Class_ReadAllRes>(
                        "GetClassByID",  // Tên stored procedure
                        new { ClassID = classId },  // Tham số đầu vào
                        commandType: CommandType.StoredProcedure  // Xác định là stored procedure
                    );

                    return result; // Trả về thông tin lớp học hoặc null nếu không tìm thấy
                }
                catch (Exception ex)
                {
                    // Log exception nếu cần thiết
                    return null; // Trả về null nếu có lỗi xảy ra
                }
            }
        }


        public async Task<bool> CreateClass(Class_CreateReq classDto)
        {
            // Kiểm tra đầu vào
            var (isValidCode, messageCode) = ErrorHandling.HandleIfEmpty(classDto.ClassCode);
            var (isValidName, messageName) = ErrorHandling.HandleIfEmpty(classDto.ClassName);

            if (!isValidCode || !isValidName)
            {
                return ErrorHandling.HandleError(StatusCodes.Status400BadRequest); // Trả về lỗi nếu dữ liệu không hợp lệ
            }

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Sử dụng Dapper để gọi stored procedure
                    var result = await connection.ExecuteAsync("CreateClass", classDto, commandType: CommandType.StoredProcedure);

                    if (result > 0)
                    {
                        return true; // Trả về true nếu thành công
                    }
                    else
                    {
                        return ErrorHandling.HandleError(StatusCodes.Status500InternalServerError); // Trả về lỗi nếu thất bại
                    }
                }
                catch (Exception)
                {
                    // Xử lý và ghi log lỗi nếu có
                    return ErrorHandling.HandleError(StatusCodes.Status500InternalServerError); // Trả về lỗi cho exception
                }
            }
        }

        public async Task<bool> DeleteClass(int classId)
        {
            // Kiểm tra ID hợp lệ
            var (isValid, errorMessage) = ErrorHandling.ValidateId(classId);
            if (!isValid)
            {
                return ErrorHandling.HandleError(StatusCodes.Status400BadRequest); // Trả về lỗi nếu ID không hợp lệ
            }

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Đánh dấu IsDeleted là true cho bản ghi có ID tương ứng
                    var result = await connection.ExecuteAsync(
                        "DeleteClass",
                        new { ClassID = classId },
                        commandType: CommandType.StoredProcedure
                    );

                    return result == 1; // Trả về true nếu cập nhật thành công
                }
                catch (Exception ex)
                {
                    // Xử lý ngoại lệ và ghi log nếu cần thiết
                    return ErrorHandling.HandleError(StatusCodes.Status500InternalServerError); // Trả về lỗi nếu có exception
                }
            }
        }

        public async Task<bool> UpdateClass(Class_UpdateReq updateReq)
        {
            // Kiểm tra đầu vào hợp lệ
            if (string.IsNullOrWhiteSpace(updateReq.ClassCode) || string.IsNullOrWhiteSpace(updateReq.ClassName))
            {
                return false; // Trả về false nếu dữ liệu không hợp lệ
            }

            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Gọi stored procedure để cập nhật lớp học
                    var result = await connection.ExecuteAsync(
                        "UpdateClass", updateReq,
                        commandType: CommandType.StoredProcedure
                    );

                    return result > 0; // Trả về true nếu cập nhật thành công
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu cần
                    return false; // Trả về false nếu có lỗi xảy ra
                }
            }
        }

        public async Task<IEnumerable<Class_ReadAllRes>> SearchClasses(string classCode = null, string className = null, string session = null, string subjectName = null)
        {
            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Tạo câu truy vấn SQL động tùy thuộc vào các tham số tìm kiếm
                    var query = "SELECT * FROM Classes WHERE 1=1";  // Lọc tất cả các lớp, 1=1 là điều kiện luôn đúng

                    // Các tham số tìm kiếm
                    if (!string.IsNullOrEmpty(classCode))
                    {
                        query += " AND ClassCode LIKE @ClassCode";
                    }
                    if (!string.IsNullOrEmpty(className))
                    {
                        query += " AND ClassName LIKE @ClassName";
                    }
                    if (!string.IsNullOrEmpty(session))
                    {
                        query += " AND Session LIKE @Session";
                    }
                    if (!string.IsNullOrEmpty(subjectName))
                    {
                        query += " AND SubjectsName LIKE @SubjectsName";
                    }

                    // Thực thi câu truy vấn
                    var result = await connection.QueryAsync<Class_ReadAllRes>(
                        query,
                        new { ClassCode = $"%{classCode}%", ClassName = $"%{className}%", Session = $"%{session}%", SubjectsName = $"%{subjectName}%" }
                    );

                    return result; // Trả về kết quả tìm kiếm
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu có
                    return Enumerable.Empty<Class_ReadAllRes>(); // Trả về danh sách trống nếu có lỗi
                }
            }
        }

        public byte[] ExportClassesToExcel(List<Class_ReadAllRes> classes)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Classes");

                // Đặt tiêu đề cho các cột
                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Mã lớp";
                worksheet.Cells[1, 3].Value = "Tên lớp";
                worksheet.Cells[1, 4].Value = "Buổi học";
                worksheet.Cells[1, 5].Value = "Chủ đề";

                // Dữ liệu lớp học
                for (int i = 0; i < classes.Count; i++)
                {
                    var currentClass = classes[i];
                    worksheet.Cells[i + 2, 1].Value = i + 1;
                    worksheet.Cells[i + 2, 2].Value = currentClass.ClassCode;
                    worksheet.Cells[i + 2, 3].Value = currentClass.ClassName;
                    worksheet.Cells[i + 2, 4].Value = currentClass.Session;
                    worksheet.Cells[i + 2, 5].Value = currentClass.SubjectsName;
                }

                // Trả về byte array của file Excel
                return package.GetAsByteArray();
            }
        }
    }
}
