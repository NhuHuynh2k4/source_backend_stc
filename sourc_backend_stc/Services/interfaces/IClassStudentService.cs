using sourc_backend_stc.Models;

namespace sourc_backend_stc.Services
{
    public interface IClassStudentService
    {
        ClassStudent_ReadAllRes GetClassStudentById(int id);
        IEnumerable<ClassStudent_ReadAllRes> GetAllClassStudents(); // Thay đổi kiểu trả về
        void CreateClassStudent(ClassStudent_CreateReq request);
        void UpdateClassStudent(int id, ClassStudent_CreateReq request);
        void DeleteClassStudent(int id);
    }
}
