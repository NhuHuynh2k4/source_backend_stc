using sourc_backend_stc.Models;
namespace sourc_backend_stc.Services
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectReadAllRes>> GetAllSubjectsAsync();
        Task<SubjectReadAllRes> GetSubjectByIdAsync(int subjectId);
        Task<int> CreateSubjectAsync(Subject_CreateReq createReq);
        Task<bool> UpdateSubjectAsync(Subject_UpdateReq updateReq);
        Task<bool> DeleteSubjectAsync(int subjectId);
    }
}