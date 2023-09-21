using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Services.Auth;
using Compus.Application.Abstractions;
using Compus.Domain.Server;
using Microsoft.AspNetCore.Authentication;
using NSubstitute;

namespace Compus.Tests.Application.Auth;

public class SignInTest
{
    private User[] CreateTestUsers()
        => new User[]
           {
               new()
               {
                   Id = default,
                   UserName = nameof(User.UserName),
                   Password = nameof(User.Password),
               }
           };

    private IHttpContextWrapper MockHttpContext()
    {
        var contextWrapper = Substitute.For<IHttpContextWrapper>();
        contextWrapper.SignInAsync(
            Arg.Any<string>(),
            Arg.Any<ClaimsPrincipal>(),
            Arg.Any<AuthenticationProperties>())
            .Returns(Task.CompletedTask);
        return contextWrapper;
    }

    [Fact]
    public async Task SignIn_ShouldReturnTrueWhenUserExists()
    {
        var config = new ServerConfig
        {
            Users = CreateTestUsers(),
            EnableAuthorization = false,
        };
        var authService = new AuthService(MockHttpContext(), config);

        var result = await authService.SignInAsync(nameof(User.UserName), nameof(User.Password), false);

        Assert.True(result);
    }

    [Fact]
    public async Task SignIn_ShouldReturnFalseWhenUserNotFound()
    {
        var config = new ServerConfig
        {
            Users = new User[] {},
            EnableAuthorization = false,
        };
        var authService = new AuthService(MockHttpContext(), config);

        var result = await authService.SignInAsync(nameof(User.UserName), nameof(User.Password), false);

        Assert.False(result);
    }
}
