using System.Net;
using Compus.Application.Abstractions;
using Compus.Application.Services.Auth;
using Compus.Application.Services.Captcha;
using Compus.Application.Services.Terminal;
using Compus.Application.Services.Terminal.Utils;
using Compus.Domain.Server;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Compus.Web.API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDistributedMemoryCache();

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(60);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.AddControllersWithViews();
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
        {
            options.Events = new CookieAuthenticationEvents
            {
                OnRedirectToLogin = ctx =>
                {
                    if (ctx.Request.Path.StartsWithSegments("/api"))
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    }
                    else
                    {
                        ctx.Response.Redirect(ctx.RedirectUri);
                    }

                    return Task.FromResult(0);
                }
            };
        });

        services.AddRazorPages();

        var config = new ServerConfig();
        Configuration.Bind("ServerConfig", config);
        services.AddSingleton(config);
        services.AddSingleton(new ConnectionManager(config));
        services.AddHttpContextAccessor();
        services.AddScoped<IHttpContextWrapper, HttpContextWrapper>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICaptchaService, CaptchaService>();
        services.AddScoped<ITerminalService, TerminalService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSession();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
            endpoints.MapFallbackToFile("index.html");
        });
    }
}
