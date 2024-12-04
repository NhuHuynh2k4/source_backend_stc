using Microsoft.Data.SqlClient;
using sourc_backend_stc.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using OfficeOpenXml;

using Dapper;
using System.Data;
using Newtonsoft.Json;

namespace sourc_backend_stc.Services
{
    public class ClassStudentService : IClassStudentService
    {
        private readonly IConfiguration _configuration;

        public ClassStudentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Lấy tất cả ClassStudents
        public async Task<IEnumerable<ClassStudent_ReadAllRes>> GetAllClassStudent()
        {
            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    var result = await connection.QueryAsync<ClassStudent_ReadAllRes>(
                        "GetAllClassStudent",
                        commandType: CommandType.StoredProcedure
                    );

                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi truy vấn database: {ex.Message}");
                    throw new Exception("Lỗi khi lấy danh sách ClassStudent từ database.");
                }
            }
        }

        public async Task<ClassStudent_ReadAllRes> GetClassStudentById(int classStudentId)
        {
            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    var result = await connection.QueryFirstOrDefaultAsync<ClassStudent_ReadAllRes>(
                        "GetClassStudentById",
                        new { Class_StudentID = classStudentId },
                        commandType: CommandType.StoredProcedure
                    );

                    if (result == null)
                    {
                        Console.WriteLine($"Không tìm thấy ClassStudent với ID: {classStudentId}");
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi lấy ClassStudent theo ID: {ex.Message}");
                    throw new Exception("Lỗi khi lấy ClassStudent theo ID", ex);
                }
            }
        }

        // Tạo mới ClassStudent
        public async Task<bool> CreateClassStudent(ClassStudent_CreateReq classStudentDto)
        {
            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {

                    var result = await connection.ExecuteAsync(
                        "CreateClassStudent",
                        classStudentDto,
                        commandType: CommandType.StoredProcedure
                    );

                    return result > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi khi tạo mới ClassStudent", ex);
                }
            }
        }

        // Cập nhật ClassStudent
        public async Task<bool> UpdateClassStudent(int id, ClassStudent_UpdateReq updateReq)
        {
            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    
                    if (updateReq == null)
                    {
                        throw new ArgumentException("Dữ liệu yêu cầu không hợp lệ.");
                    }

                    var result = await connection.ExecuteAsync(
                        "UpdateClassStudent",
                        new
                        {
                            Class_StudentID = id,
                            ClassID = updateReq.ClassID,
                            StudentID = updateReq.StudentID
                        },
                        commandType: CommandType.StoredProcedure
                    );

                    return result > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi khi cập nhật ClassStudent", ex);
                }
            }
        }

        public async Task<bool> DeleteClassStudent(int classStudentId)
        {
            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    var existingClassStudent = await connection.QueryFirstOrDefaultAsync<ClassStudent>(
                        "GetClassStudentById",
                        new { Class_StudentID = classStudentId },
                        commandType: CommandType.StoredProcedure
                    );

                    if (existingClassStudent == null)
                    {
                        throw new Exception($"Không tìm thấy ClassStudent với ID {classStudentId}. Không thể xóa.");
                    }

                    var result = await connection.ExecuteAsync(
                        "DeleteClassStudent",
                        new { Class_StudentID = classStudentId },
                        commandType: CommandType.StoredProcedure
                    );

                    return result > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi khi xóa ClassStudent", ex);
                }
            }
        }

        public byte[] ExportClassStudentsToExcel(List<ClassStudent_ReadAllRes> classStudents)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("ClassStudents");

                // Đặt tiêu đề cho các cột
                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Mã lớp";
                worksheet.Cells[1, 3].Value = "Mã học sinh";
                worksheet.Cells[1, 4].Value = "Ngày tạo";
                worksheet.Cells[1, 5].Value = "Ngày cập nhật";

                // Dữ liệu lớp học
                for (int i = 0; i < classStudents.Count; i++)
                {
                    var currentClassStudent = classStudents[i];
                    worksheet.Cells[i + 2, 1].Value = i + 1;
                    worksheet.Cells[i + 2, 2].Value = currentClassStudent.ClassID;
                    worksheet.Cells[i + 2, 3].Value = currentClassStudent.StudentID;
                    worksheet.Cells[i + 2, 4].Value = currentClassStudent.CreateDate;
                    worksheet.Cells[i + 2, 5].Value = currentClassStudent.UpdateDate;
                }

                // Trả về byte array của file Excel
                return package.GetAsByteArray();
            }
        }
    }

}
