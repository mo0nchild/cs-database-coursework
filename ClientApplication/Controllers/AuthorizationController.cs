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

    using static ClientApplication.ViewModels.AuthorizationModel;

    [ControllerAttribute]
    public partial class AuthorizationController : Controller
    {
        protected IDbContextFactory<DatabaseContext> DatabaseFactory { get; private set; } = default!;

        private readonly ILogger<AuthorizationController> _logger;
        public AuthorizationController(IDbContextFactory<DatabaseContext> factory, ControllerLogger logger)
            : base() { (this.DatabaseFactory, this._logger) = (factory, logger); }

        [RouteAttribute("authorization", Name = "authorization")]
        [HttpGetAttribute]
        public async Task<IActionResult> Authorization(ViewModels.AuthorizationModel? model)
        {
            using var dbcontext = await this.DatabaseFactory.CreateDbContextAsync();
            this.ViewBag.GenderTypes = dbcontext.Gendertypes.Select(item => item.Gendertypename).ToList();

            if (this.HttpContext.User.Identity != null && this.HttpContext.User.Identity.IsAuthenticated)
            {
                return base.LocalRedirect("/user/profile");
            }
            return base.View(model ?? new ViewModels.AuthorizationModel());
        }

        [RouteAttribute("login", Name = "login"), HttpPostAttribute]
        public async Task<IActionResult> Login([FromForm]string login, [FromForm]string password)
        {
            this._logger.LogInformation($"Login: {login}; Password: {password}");
            DAModels::Authorization? userProfile = default;
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                userProfile = dbcontext.Authorizations.Include((item) => item.Contact)
                    .Where((item) => (item.Login == login || item.Contact.Emailaddress == login) 
                    && item.Password == password).FirstOrDefault();

                if (userProfile == null) return base.RedirectToRoute("authorization", new AuthorizationModel
                { ErrorCause = "", Mode = AuthorizationMode.Login, HasError = true });
            }
            var profilePrinciple = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.Role, "User"),
                new Claim(ClaimTypes.PrimarySid, userProfile.Contact.Contactid.ToString())
            }, 
            CookieAuthenticationDefaults.AuthenticationScheme);
            if(userProfile.Isadmin) profilePrinciple.AddClaim(new Claim(ClaimTypes.Role, "Admin"));

            await this.HttpContext.SignInAsync(new ClaimsPrincipal(profilePrinciple));
            return base.RedirectToAction("ProfileInfo", "UserProfile");
        }
        private AuthorizationModel? ValidateModel(DAModels::Authorization auth, DAModels::Contact user)
        {
            foreach (var propertyValue in new[] { auth.Login, auth.Password, user.Surname, user.Name })
            {
                if (propertyValue != null && Regex.IsMatch(propertyValue, @"[А-Яа-я\w]{4,}")) continue;
                return new ViewModels.AuthorizationModel(true)
                {
                    ErrorCause = @$"Данные неверно введены{(propertyValue == null ? "" : $": {propertyValue}")}",
                    Mode = ViewModels.AuthorizationModel.AuthorizationMode.Registration
                };
            }
            var emailMatch = Regex.IsMatch(user.Emailaddress, @"^\w{6,}@(mail|gmail|yandex){1}.(ru|com){1}$");
            var phoneMatch = Regex.IsMatch(user.Phonenumber!, @"^\+7[0-9]{10}$");

            user.Phonenumber = (user.Phonenumber == string.Empty) ? null! : user.Phonenumber;
            if (!emailMatch || (!phoneMatch && user.Phonenumber != null))
            {
                return new ViewModels.AuthorizationModel(true)
                {
                    ErrorCause = (!emailMatch ? "Неверный адрес почты" : (!phoneMatch ? "Неверный телефон" : null)),
                    Mode = ViewModels.AuthorizationModel.AuthorizationMode.Registration,
                };
            }
            return default(AuthorizationModel);
        }
        [RouteAttribute("registration", Name = "registration")]
        [HttpPostAttribute]
        public async Task<IActionResult> Registration([FromForm] DAModels::Contact profile,
            [FromForm, Bind("Login", "Password")] DAModels::Authorization authorization)
        {
            if (profile is null) return base.RedirectToAction("Authorization", new RouteValueDictionary()
            {["haserror"] = true, ["mode"] = AuthorizationMode.Registration, ["errorcause"] = "Данные не установлены"});

            var modelValidation = this.ValidateModel(authorization, profile);
            if (modelValidation != null) return base.RedirectToAction("Authorization", new RouteValueDictionary()
            { 
                ["haserror"] = modelValidation.HasError, ["mode"] = modelValidation.Mode, 
                ["errorcause"] = modelValidation.ErrorCause 
            });
            var referenceCollision = default(int) + 1;
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                profile.Gendertype = (await dbcontext.Gendertypes.Where((DAModels::Gendertype item) 
                    => item.Gendertypename == profile.Gendertype.Gendertypename).FirstOrDefaultAsync())!;

                profile.Lastupdate = DateTime.Now;
                profile.Userpicture = await dbcontext.Userpictures.FirstAsync();
                while (referenceCollision != 0) 
                {
                    authorization!.Referenceguid = Guid.NewGuid().ToString();

                    referenceCollision = dbcontext.Authorizations.Where(
                        (DAModels::Authorization item) => item.Referenceguid == authorization.Referenceguid).Count();
                }
                profile.Authorization = default(DAModels::Authorization);
                await dbcontext.Contacts.AddAsync(authorization.Contact = profile);

                await dbcontext.Authorizations.AddAsync(authorization);
                try { await dbcontext.SaveChangesAsync(); } catch (System.Exception error)
                {
                    return base.RedirectToAction("Authorization", new RouteValueDictionary()
                    { ["haserror"] = true, ["mode"] = AuthorizationMode.Registration, ["errorcause"] = error.Message }); 
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
            return base.RedirectToAction("ProfileInfo", "UserProfile");
        }

        [RouteAttribute("logout", Name = "logout"), HttpGetAttribute]
        public async Task<IActionResult> Logout()
        {
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return base.LocalRedirect("/");
        }
    }
}
