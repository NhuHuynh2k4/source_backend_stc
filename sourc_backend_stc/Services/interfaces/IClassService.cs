
using sourc_backend_stc.Models;

namespace sourc_backend_stc.Services
{
    public interface IClassService
    {
        Task<bool> CreateClass(Class_CreateReq classDto);
        Task<bool> UpdateClass(int classId, Class_UpdateReq updateReq);
        Task<bool> DeleteClass(int classId);
        Task<Class_ReadAllRes> GetClassById(int classId);
        Task<IEnumerable<Class_ReadAllRes>> GetAllClasses();
    }

}
