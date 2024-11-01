using sourc_backend_stc.Models;
using System.Threading.Tasks;

namespace sourc_backend_stc.Services
{
    public interface IAuthService
    {
        Task<string> AuthenticateAsync(LoginReq login);
    }
}
