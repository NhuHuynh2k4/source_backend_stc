using sourc_backend_stc.Models;

namespace sourc_backend_stc.Services
{
    public interface IClassStudentService
    {
        ClassStudentResponse GetClassStudentById(int id);
        void CreateClassStudent(ClassStudent_CreateReq request);
        void UpdateClassStudent(int id, ClassStudent_CreateReq request);
        void DeleteClassStudent(int id);
    }
}
