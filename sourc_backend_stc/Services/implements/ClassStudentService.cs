using sourc_backend_stc.Models;
using sourc_backend_stc.Data;
using sourc_backend_stc.Utils;

namespace sourc_backend_stc.Services
{
    public class ClassStudentService : IClassStudentService
    {
        private readonly AppDbContext _context;

    public ClassStudentService(AppDbContext context)
    {
        _context = context;
    }

    public ClassStudentResponse GetClassStudentById(int id)
    {
        var validationResult = ErrorHandling.ValidateId(id);
        if (!validationResult.IsValid)
            throw new Exception(ErrorCodes.GetErrorMessage(ErrorCodes.InvalidId));

        // Truy vấn dữ liệu từ cơ sở dữ liệu
        var classStudent = _context.ClassStudents.FirstOrDefault(cs => cs.ClassStudentID == id);
        
        if (classStudent == null)
            throw new Exception(ErrorCodes.GetErrorMessage(ErrorCodes.ResourceNotFound));

        // Chuyển đổi dữ liệu từ ClassStudent sang ClassStudentResponse
        return new ClassStudentResponse
        {
            ClassStudentID = classStudent.ClassStudentID,
            ClassID = classStudent.ClassID,
            StudentID = classStudent.StudentID,
            CreateDate = classStudent.CreateDate,
            UpdateDate = classStudent.UpdateDate
        };
    }

        public void CreateClassStudent(ClassStudent_CreateReq request)
        {
            // Logic thêm mới dữ liệu vào database
            // Thêm xử lý logic để lưu vào database
        }

        public void UpdateClassStudent(int id, ClassStudent_CreateReq request)
        {
            var validationResult = ErrorHandling.ValidateId(id);
            if (!validationResult.IsValid)
                throw new Exception(ErrorCodes.GetErrorMessage(ErrorCodes.InvalidId));

            // Logic cập nhật dữ liệu trong database
        }

        public void DeleteClassStudent(int id)
        {
            var validationResult = ErrorHandling.ValidateId(id);
            if (!validationResult.IsValid)
                throw new Exception(ErrorCodes.GetErrorMessage(ErrorCodes.InvalidId));

            // Logic xóa dữ liệu trong database
        }
    }
}
