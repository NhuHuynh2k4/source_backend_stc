using sourc_backend_stc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sourc_backend_stc.Services
{
    public interface IMarkService
    {
        Task<bool> CreateMark(Mark_CreateReq markDto);
        Task<bool> UpdateMark(Mark_UpdateReq updateReq);
        Task<bool> DeleteMark(int markId);
        Task<Mark_ReadAllRes> GetMarkById(int markId);
        Task<IEnumerable<Mark_ReadAllRes>> GetAllMarks();
    }
}