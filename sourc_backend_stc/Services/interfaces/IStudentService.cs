
using sourc_backend_stc.Models;

namespace sourc_backend_stc.Services
{
    public interface IStudentService
    {
        Task<bool> CreateStudent(Student_CreateReq createReq);
        Task<bool> UpdateStudent(int StudentID, Student_UpdateReq updateReq);
        Task<bool> DeleteStudent(int StudentID);
        Task<Student_ReadAllRes> GetStudentById(int StudentID);
        Task<IEnumerable<Student_ReadAllRes>> GetAllStudent();
    }

}