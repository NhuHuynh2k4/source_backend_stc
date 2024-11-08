using sourc_backend_stc.Models;

namespace sourc_backend_stc.Services
{
    public interface IAnswerService
    {
        Task<bool> CreateAnswer(Answer_CreateReq answerDto);
        Task<bool> UpdateAnswer(Answer_UpdateReq updateReq);
        Task<bool> DeleteAnswer(int answerId);
        Task<Answer_ReadAllRes> GetAnswerById(int answerId);
        Task<IEnumerable<Answer_ReadAllRes>> GetAllAnswers();
    }

}
