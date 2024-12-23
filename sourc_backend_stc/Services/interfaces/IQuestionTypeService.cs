using sourc_backend_stc.Models;

namespace sourc_backend_stc.Services
{
    public interface IQuestionTypeService
    {
        Task<IEnumerable<QuestionTypeResponse>> GetAllQuestionType();
        Task<QuestionTypeResponse> GetQuestionTypeById(int id);
        Task<bool> CreateQuestionType(QuestionType_CreateReq request);
        Task<bool> UpdateQuestionType(QuestionType_UpdateReq req);
        Task<bool> DeleteQuestionType(int id);
        // byte[] ExportQuestionTypeToExcel(List<QuestionType_ReadAllRes> questionTypes);
        byte[] ExportQuestionTypeToExcel(List<QuestionTypeResponse> questionTypeList);
    }
}
