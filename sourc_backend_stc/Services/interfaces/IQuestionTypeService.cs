using sourc_backend_stc.Models;

namespace sourc_backend_stc.Services
{
    public interface IQuestionTypeService
    {
        IEnumerable<QuestionTypeResponse> GetAllQuestionType();
        QuestionTypeResponse GetQuestionTypeById(int id);
        void CreateQuestionType(QuestionType_CreateReq request);
        void UpdateQuestionType(int id, QuestionType_CreateReq request);
        void DeleteQuestionType(int id);
    }
}
