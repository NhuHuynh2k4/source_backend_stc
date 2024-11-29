using sourc_backend_stc.Models;

namespace sourc_backend_stc.Services
{
    public interface IClassStudentService
    {
        Task<ClassStudent_ReadAllRes> GetClassStudentById(int id);
        Task<IEnumerable<ClassStudent_ReadAllRes>> GetAllClassStudent();
        Task<bool> CreateClassStudent(ClassStudent_CreateReq request);
        Task<bool> UpdateClassStudent(int id, ClassStudent_UpdateReq updateReq);
        Task<bool> DeleteClassStudent(int id);
        byte[] ExportClassStudentsToExcel(List<ClassStudent_ReadAllRes> classStudents);
    }
}
