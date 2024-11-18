using sourc_backend_stc.Models;
namespace sourc_backend_stc.Services
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectReadAllRes>> GetAllSubjectsAsync();
        Task<SubjectReadAllRes> GetSubjectByIdAsync(int subjectId);
        Task<int> CreateSubjectAsync(Subject_CreateReq request);
        Task<bool> UpdateSubjectAsync(Subject_UpdateReq request);
        Task<bool> DeleteSubjectAsync(int subjectId);
    }
}