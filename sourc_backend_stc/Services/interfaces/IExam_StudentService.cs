
using sourc_backend_stc.Models;

namespace sourc_backend_stc.Services
{
    public interface IExam_StudentService
    {
        Task<bool> CreateExam_Student(Exam_Student_CreateReq createReq);
        Task<bool> UpdateExam_Student(Exam_Student_UpdateReq updateReq);
        Task<bool> DeleteExam_Student(int Exam_StudentID);
        Task<Exam_StudentRes> GetExam_StudentById(int Exam_StudentID);
        Task<IEnumerable<Exam_StudentRes>> GetAllExam_Student();
    }

}