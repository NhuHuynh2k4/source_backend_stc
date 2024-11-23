using sourc_backend_stc.Models;

namespace sourc_backend_stc.Services
{
    public interface ITestQuestionService
    {
        Task<bool> CreateTestQuestion(TestQuestion_CreateReq testQuestionDto);
        Task<bool> UpdateTestQuestion(TestQuestion_UpdateReq updateReq);
        Task<bool> DeleteTestQuestion(int testQuestionId);
        Task<TestQuestion_ReadAllRes> GetTestQuestionById(int testQuestionId);
        Task<IEnumerable<TestQuestion_ReadAllRes>> GetAllTestQuestions();
    }

}
