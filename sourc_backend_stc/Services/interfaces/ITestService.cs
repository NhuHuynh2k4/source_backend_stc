
using sourc_backend_stc.Models;

namespace sourc_backend_stc.Services
{
    public interface ITestService
    {
        Task<bool> CreateTest(Test_CreateReq testDto);
        Task<bool> UpdateTest(int testId, Test_UpdateReq updateReq);
        Task<bool> DeleteTest(int testId);
        Task<Test_ReadAllRes> GetTestById(int testId);
        Task<IEnumerable<Test_ReadAllRes>> GetAllTests();
    }

}
