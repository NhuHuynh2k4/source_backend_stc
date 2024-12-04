
using sourc_backend_stc.Models;
using static sourc_backend_stc.Services.StudentService;

namespace sourc_backend_stc.Services
{
    public interface IStudentService
    {
        Task<CreateStudentResult> CreateStudent(Student_CreateReq createReq);
        Task<UpdateStudentResult> UpdateStudent(Student_UpdateReq updateReq);
        Task<bool> DeleteStudent(int StudentID);
        Task<Student_ReadAllRes> GetStudentById(int StudentID);
        Task<IEnumerable<Student_ReadAllRes>> GetAllStudent();
        byte[] ExportStudentToExcel(List<Student_ReadAllRes> student);
    }

}