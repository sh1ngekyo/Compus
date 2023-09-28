using Compus.Application.Abstractions;
using Compus.Domain.Client;
using Compus.Domain.Server.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Compus.Web.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) 
            => _authService = authService;

        public async Task<SignInViewModel> SignIn(SignInViewModel signInVM)
        {
            signInVM.Status = SignInStatus.Authorized;
            var sessionCaptcha = HttpContext.Session.GetString(nameof(SignInViewModel.Captcha))!;
            HttpContext.Session.Remove(nameof(SignInViewModel.Captcha));
            if (!_authService.ValidateCaptcha(signInVM.Captcha!, sessionCaptcha))
            {
                signInVM.Status = SignInStatus.NotAuthorized;
                signInVM.ErrorMessage = "Wrong Captcha";
                return signInVM;
            }
            if (!await _authService.SignInAsync(signInVM.UserName!, signInVM.Password!, signInVM.Persist))
            {
                signInVM.Status = SignInStatus.NotAuthorized;
                signInVM.ErrorMessage = "Wrong UserName or Password";
            }
            return signInVM;
        }

        public async new void SignOut() => await _authService.SignOutAsync();

        public bool Authorized() => _authService.Authorized;

        public IActionResult RequestCaptcha([FromServices] ICaptchaService captchaService)
        {
            HttpContext.Session.SetString(
                nameof(SignInViewModel.Captcha),
                captchaService.Generate(200, 60, out var images).ToLowerInvariant());

            return File(images, "image/png");
        }
    }
}
