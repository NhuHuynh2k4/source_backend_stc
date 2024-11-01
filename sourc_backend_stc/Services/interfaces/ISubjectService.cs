using sourc_backend_stc.Models;
namespace sourc_backend_stc.Services
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectReadAllRes>> GetAllSubjectsAsync();
        Task<SubjectReadAllRes> GetSubjectByIdAsync(int subjectId);
        Task<bool> CreateSubjectAsync(Subject_CreateReq request);
        Task<bool> UpdateSubjectAsync(int subjectId, Subject_UpdateReq request);
        Task<bool> DeleteSubjectAsync(int subjectId);
    }
}