using DatabaseAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClientApplication.Middleware
{
    public static class CheckProfileExtensions : System.Object
    {
        public static IApplicationBuilder UseCheckProfile(this IApplicationBuilder builder)
        { 
            return builder.UseMiddleware<Middleware.CheckProfileMiddleware>(); 
        }
    }
    public partial class CheckProfileMiddleware : System.Object
    {
        protected readonly RequestDelegate requestDelegate = default!;
        protected IDbContextFactory<DatabaseContext> DatabaseFactory { get; set; } = default!;

        public CheckProfileMiddleware(RequestDelegate next, IDbContextFactory<DatabaseContext> factory) 
            : base() { (this.requestDelegate, this.DatabaseFactory) = (next, factory); }

        public async Task InvokeAsync(HttpContext context, ILogger<CheckProfileMiddleware> logger)
        {
            var profileId = context.User.FindFirst(ClaimTypes.PrimarySid);
            if (profileId == null || !int.TryParse(profileId.Value, out var profileIdValue)) 
            { 
                await this.requestDelegate.Invoke(context); return;
            }
            logger.LogWarning($"Check profileId: {profileId.Value}");
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                if(dbcontext.Contacts.Find(profileIdValue) == null)
                {
                    logger.LogWarning($"Remove cookie profileId: {profileId.Value}");

                    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    context.Response.Redirect("/authorization", true); return;
                }
            }
            await this.requestDelegate.Invoke(context);
        }
    }
}
