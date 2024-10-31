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

        public ClassStudent_ReadAllRes GetClassStudentById(int id)
        {
            var validationResult = ErrorHandling.ValidateId(id);
            if (!validationResult.IsValid)
                throw new Exception(ErrorCodes.GetErrorMessage(ErrorCodes.InvalidId));

            // Truy vấn dữ liệu từ cơ sở dữ liệu
            var classStudent = _context.ClassStudents.FirstOrDefault(cs => cs.Class_StudentID == id);

            if (classStudent == null)
                throw new Exception(ErrorCodes.GetErrorMessage(ErrorCodes.ResourceNotFound));

            // Chuyển đổi dữ liệu từ ClassStudent sang ClassStudentResponse
            return new ClassStudent_ReadAllRes
            {
                Class_StudentID = classStudent.Class_StudentID,
                ClassID = classStudent.ClassID,
                StudentID = classStudent.StudentID,
                CreateDate = classStudent.CreateDate,
                UpdateDate = classStudent.UpdateDate
            };
        }
        public IEnumerable<ClassStudent_ReadAllRes> GetAllClassStudents()
        {
            // Truy vấn tất cả dữ liệu từ cơ sở dữ liệu
            var classStudents = _context.ClassStudents
                .Where(cs => !cs.IsDelete) // Lọc bỏ những bản ghi đã bị xóa
                .ToList();

            // Chuyển đổi dữ liệu sang ClassStudent_ReadAllRes
            return classStudents.Select(cs => new ClassStudent_ReadAllRes
            {
                Class_StudentID = cs.Class_StudentID, // Đảm bảo sử dụng đúng thuộc tính
                ClassID = cs.ClassID,
                StudentID = cs.StudentID,
                CreateDate = cs.CreateDate,
                UpdateDate = cs.UpdateDate
            });
        }


        public void CreateClassStudent(ClassStudent_CreateReq request)
        {
            var classStudent = new ClassStudent
            {
                ClassID = request.ClassID,
                StudentID = request.StudentID
            };
            _context.ClassStudents.Add(classStudent);
            _context.SaveChanges();
        }

        public void UpdateClassStudent(int id, ClassStudent_CreateReq request)
        {
            var validationResult = ErrorHandling.ValidateId(id);
            if (!validationResult.IsValid)
                throw new Exception(ErrorCodes.GetErrorMessage(ErrorCodes.InvalidId));

            var classStudent = _context.ClassStudents.FirstOrDefault(cs => cs.Class_StudentID == id);

            if (classStudent == null)
                throw new Exception(ErrorCodes.GetErrorMessage(ErrorCodes.ResourceNotFound));

            classStudent.ClassID = request.ClassID;
            classStudent.StudentID = request.StudentID;
            classStudent.UpdateDate = DateTime.Now;

            _context.SaveChanges();
        }

        public void DeleteClassStudent(int id)
        {
            var validationResult = ErrorHandling.ValidateId(id);
            if (!validationResult.IsValid)
                throw new Exception(ErrorCodes.GetErrorMessage(ErrorCodes.InvalidId));

            var classStudent = _context.ClassStudents.FirstOrDefault(cs => cs.Class_StudentID == id);

            if (classStudent == null)
                throw new Exception(ErrorCodes.GetErrorMessage(ErrorCodes.ResourceNotFound));

            classStudent.IsDelete = true; // Đánh dấu đã xóa thay vì xóa vật lý
            _context.SaveChanges();
        }
    }

}
