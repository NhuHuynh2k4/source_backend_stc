using Microsoft.Data.SqlClient;
using sourc_backend_stc.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
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
                    // Log lỗi ra console hoặc file log để biết chi tiết lỗi từ database
                    Console.WriteLine($"Lỗi khi truy vấn database: {ex.Message}");
                    throw new Exception("Lỗi khi lấy danh sách ClassStudent từ database.");
                }
            }
        }


        // Lấy thông tin ClassStudent theo ID
        public async Task<ClassStudent_ReadAllRes> GetClassStudentById(int classStudentId)
        {
            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    var result = await connection.QueryFirstOrDefaultAsync<ClassStudent_ReadAllRes>(
                        "GetClassStudentById", // Tên stored procedure
                        new { Class_StudentID = classStudentId }, // Truyền tham số đúng tên
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
                    // Kiểm tra nếu dữ liệu yêu cầu cập nhật không hợp lệ
                    if (updateReq == null)
                    {
                        throw new ArgumentException("Dữ liệu yêu cầu không hợp lệ.");
                    }

                    // Thực hiện cập nhật thông qua stored procedure
                    var result = await connection.ExecuteAsync(
                        "UpdateClassStudent",updateReq,
                        commandType: CommandType.StoredProcedure
                    );

                    return result > 0; // Trả về true nếu cập nhật thành công
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi khi cập nhật ClassStudent", ex);
                }
            }
        }

        // Xóa ClassStudent
        public async Task<bool> DeleteClassStudent(int classStudentId)
        {
            using (var connection = DatabaseConnection.GetConnection(_configuration))
            {
                await connection.OpenAsync();

                try
                {
                    // Kiểm tra xem ClassStudent có tồn tại hay không
                    var existingClassStudent = await connection.QueryFirstOrDefaultAsync<ClassStudent>(
                        "GetClassStudentById", // Giả sử bạn có một stored procedure hoặc query để lấy ClassStudent theo ID
                        new { Class_StudentID = classStudentId },
                        commandType: CommandType.StoredProcedure
                    );

                    // Nếu không tìm thấy ClassStudent, ném ra ngoại lệ
                    if (existingClassStudent == null)
                    {
                        throw new Exception($"Không tìm thấy ClassStudent với ID {classStudentId}. Không thể xóa.");
                    }

                    // Nếu tồn tại ClassStudent, thực hiện xóa
                    var result = await connection.ExecuteAsync(
                        "DeleteClassStudent",  // Tên của stored procedure xóa ClassStudent
                        new { Class_StudentID = classStudentId },
                        commandType: CommandType.StoredProcedure
                    );

                    return result > 0; // Trả về true nếu xóa thành công
                }
                catch (Exception ex)
                {
                    // Lỗi khác trong quá trình xóa
                    throw new Exception("Lỗi khi xóa ClassStudent", ex);
                }
            }
        }
    }
}
