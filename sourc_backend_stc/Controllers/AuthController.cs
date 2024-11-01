using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using sourc_backend_stc.Models;
using sourc_backend_stc.Services;
using Microsoft.AspNetCore.Authorization;

namespace sourc_backend_stc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginReq request)
        {
            if (request == null)
            {
                return BadRequest("Yêu cầu không hợp lệ.");
            }

            var token = await _authService.AuthenticateAsync(request);
            if (token == null)
            {
                return Unauthorized("Email hoặc mật khẩu không chính xác.");
            }

            return Ok(new { Message = "Đăng nhập thành công", Token = token });
        }
    }
}
