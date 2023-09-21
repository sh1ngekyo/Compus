using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Compus.Application.Abstractions;

public interface IHttpContextWrapper
{
    bool IsAuthenticated();
    Task SignOutAsync(string scheme); 
    Task SignInAsync(string scheme, ClaimsPrincipal claimsPrincipal, AuthenticationProperties properties);
}
