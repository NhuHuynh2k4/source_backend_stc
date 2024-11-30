using sourc_backend_stc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sourc_backend_stc.Services
{
    public interface IQuestionService
    {
        Task<IEnumerable<QuestionReadAllRes>> GetAllQuestions();
        Task<QuestionReadAllRes> GetQuestionById(int questionId);
        Task<int> CreateQuestion(Question_CreateReq createReq);
        Task<bool> UpdateQuestion(Question_UpdateReq updateReq);
        Task<bool> DeleteQuestion(int questionId);
    }
}
