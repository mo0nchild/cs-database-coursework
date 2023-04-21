using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using DatabaseAccess;
using ClientApplication.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.RegularExpressions;

namespace ClientApplication.Controllers
{
    using DAModels = DatabaseAccess.Models;
    using ControllerLogger = ILogger<AuthorizationController>;
    [ControllerAttribute]
    public partial class AuthorizationController : Controller
    {
        protected IDbContextFactory<DatabaseContext> DatabaseFactory { get; private set; } = default!;

        private readonly ILogger<AuthorizationController> _logger;
        public AuthorizationController(IDbContextFactory<DatabaseContext> factory, ControllerLogger logger)
            : base() { (this.DatabaseFactory, this._logger) = (factory, logger); }

        [RouteAttribute("authorization", Name = "authorization")]
        [HttpGetAttribute]
        public IActionResult Authorization(ViewModels.AuthorizationModel? authorizationModel)
        {
            if (this.HttpContext.User.Identity != null && this.HttpContext.User.Identity.IsAuthenticated)
            {
                return base.LocalRedirect("/user/profile");
            }
            return base.View(authorizationModel ?? new ViewModels.AuthorizationModel());
        }

        [RouteAttribute("login", Name = "login")]
        [HttpPostAttribute]
        public async Task<IActionResult> Login([FromForm]string login, [FromForm]string password)
        {
            this._logger.LogInformation($"Login: {login}; Password: {password}");
            DAModels::Authorization? userProfile = default;
            using (var dbcontext = this.DatabaseFactory.CreateDbContext())
            {
                userProfile = dbcontext.Authorizations.Include((item) => item.Contact)
                    .Where((item) => item.Login == login && item.Password == password).FirstOrDefault();

                if (userProfile == null) return base.View("Authorization", new ViewModels.AuthorizationModel(true));
            }
            var profilePrinciple = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.Role, userProfile.Isadmin ? "Admin": "User"),
                new Claim(ClaimTypes.PrimarySid, userProfile.Contact.Contactid.ToString())
            }, 
            CookieAuthenticationDefaults.AuthenticationScheme);

            await this.HttpContext.SignInAsync(new ClaimsPrincipal(profilePrinciple));
            return base.RedirectToAction("DetailsInfo", "UserProfile");
        }

        [RouteAttribute("registration", Name = "registration")]
        [HttpPostAttribute]
        public async Task<IActionResult> Registration([FromForm] DAModels::Contact profile)
        {
            var emailMatch = Regex.IsMatch(profile.Emailaddress, @"^\w+@(mail|gmail|yandex){1}.(ru|com){1}$");
            var phoneMatch = Regex.IsMatch(profile.Phonenumber!, @"^\+7[0-9]{1}$");

            profile.Phonenumber = (profile.Phonenumber == string.Empty) ? null! : profile.Phonenumber;
            if (!emailMatch || (!phoneMatch && profile.Phonenumber != null))
            {
                var routeModel = new ViewModels.AuthorizationModel(true)
                {
                    ErrorCause = (!emailMatch ? "Неверный адрес почты" : (!phoneMatch ? "Неверный телефон" : null)),
                    Mode = ViewModels.AuthorizationModel.AuthorizationMode.Registration,
                };
                return base.View("Authorization", routeModel);
            }
            DAModels::Authorization authorization = profile.Authorization!;
            using (var dbcontext = this.DatabaseFactory.CreateDbContext())
            {
                profile.Gendertype = (await dbcontext.Gendertypes.Where((DAModels::Gendertype item) 
                    => item.Gendertypename == profile.Gendertype.Gendertypename).FirstOrDefaultAsync())!;

                profile.Authorization = default(DAModels::Authorization);
                await dbcontext.Contacts.AddAsync(authorization.Contact = profile);

                await dbcontext.Authorizations.AddAsync(authorization);
                try { await dbcontext.SaveChangesAsync(); } catch (System.Exception error)
 {
                    var errorModel = new ViewModels.AuthorizationModel(true) 
                    {  ErrorCause = error.Message, Mode = AuthorizationModel.AuthorizationMode.Registration };
                    return base.View("Authorization", errorModel); 
                }
            }
            var profile_principle = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.Role, authorization.Isadmin ? "Admin": "User"),
                new Claim(ClaimTypes.PrimarySid, authorization.Contact.Contactid.ToString())
            },
            CookieAuthenticationDefaults.AuthenticationScheme);
            this._logger.LogInformation($"Account was registered: {authorization.Login}");

            await this.HttpContext.SignInAsync(new ClaimsPrincipal(profile_principle));
            return base.RedirectToAction("DetailsInfo", "UserProfile");
        }

        [RouteAttribute("logout", Name = "logout")]
        [HttpGetAttribute()]
        public async Task<IActionResult> Logout()
        {
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return base.LocalRedirect("/");
        }
    }
}
