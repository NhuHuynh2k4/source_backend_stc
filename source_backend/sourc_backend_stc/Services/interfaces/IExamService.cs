using sourc_backend_stc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sourc_backend_stc.Services
{
    public interface IExamService
    {
        Task<bool> CreateExam(Exam_CreateReq examDto);
        Task<bool> UpdateExam(Exam_UpdateReq updateReq);
        Task<bool> DeleteExam(int examId);
        Task<Exam_ReadAllRes> GetExamById(int examId);
        Task<IEnumerable<Exam_ReadAllRes>> GetAllExams();
        byte[] ExportExamsToExcel(List<Exam_ReadAllRes> exams);
    }
}