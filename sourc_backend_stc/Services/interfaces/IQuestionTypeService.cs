using sourc_backend_stc.Models;

namespace sourc_backend_stc.Services
{
    public interface IQuestionTypeService
    {
        Task<IEnumerable<QuestionTypeResponse>> GetAllQuestionType();
        Task<QuestionTypeResponse> GetQuestionTypeById(int id);
        Task<bool> CreateQuestionType(QuestionType_CreateReq request);
        Task<bool> UpdateQuestionType(int id, QuestionType_UpdateReq request);
        Task<bool> DeleteQuestionType(int id);
    }
}
