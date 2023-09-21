using Compus.Application.Abstractions;
using Compus.Domain.Client;
using Compus.Domain.Server.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Compus.Web.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SignInController : ControllerBase
    {
        private readonly IAuthService _authService;

        public SignInController(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<ClientLoginModel> Login(ClientLoginModel loginModel)
        {
            loginModel.Status = SignInStatus.Succesful;
            var sessionCaptcha = HttpContext.Session.GetString(nameof(ClientLoginModel.Captcha))!;
            HttpContext.Session.Remove(nameof(ClientLoginModel.Captcha));
            if (!_authService.ValidateCaptcha(loginModel.Captcha!, sessionCaptcha))
            {
                loginModel.Status = SignInStatus.Failed;
                loginModel.Message = "Wrong captcha";
                return loginModel;
            }
            if (!await _authService.SignInAsync(loginModel.UserName!, loginModel.Password!, loginModel.Persist))
            {
                loginModel.Status = SignInStatus.Failed;
                loginModel.Message = "Wrong username or password";
            }
            return loginModel;
        }

        public async void Logout() => await _authService.SignOutAsync();

        public bool IsLogin() => _authService.Authenticated;

        public IActionResult RequestCaptcha([FromServices] ICaptchaService captchaService)
        {
            HttpContext.Session.SetString(
                nameof(ClientLoginModel.Captcha),
                captchaService.Generate(200, 60, out var images).ToLowerInvariant());

            return File(images, "image/png");
        }
    }
}
