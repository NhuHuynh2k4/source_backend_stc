using sourc_backend_stc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sourc_backend_stc.Services
{
    public interface IQuestionService
    {
        Task<IEnumerable<QuestionReadAllRes>> GetAllQuestions();
        Task<QuestionReadAllRes> GetQuestionById(int questionId);
        Task<int> CreateQuestion(Question_CreateReq request);
        Task<bool> UpdateQuestion(Question_UpdateReq request);
        Task<bool> DeleteQuestion(int questionId);
    }
}
