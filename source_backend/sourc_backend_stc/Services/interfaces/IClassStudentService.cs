using sourc_backend_stc.Models;

namespace sourc_backend_stc.Services
{
    public interface IClassStudentService
    {
        Task<ClassStudent_ReadAllRes> GetClassStudentById(int id); // Change return type to Task<ClassStudent_ReadAllRes>
        Task<IEnumerable<ClassStudent_ReadAllRes>> GetAllClassStudent(); // Change return type to Task<IEnumerable<ClassStudent_ReadAllRes>>
        Task<bool> CreateClassStudent(ClassStudent_CreateReq request); // Change return type to Task<bool>
        Task<bool> UpdateClassStudent(int id, ClassStudent_UpdateReq updateReq);
        Task<bool> DeleteClassStudent(int id); // Change return type to Task<bool>
    }
}
