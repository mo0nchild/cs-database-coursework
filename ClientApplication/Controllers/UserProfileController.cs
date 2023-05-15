using ClientApplication.Filters;
using ClientApplication.Services;
using ClientApplication.ViewModels;
using DatabaseAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ClientApplication.Controllers
{
    using DAModels = DatabaseAccess.Models;
    using ControllerLogger = ILogger<UserProfileController>;

    [ControllerAttribute, RouteAttribute(ControllerRoute), AuthorizeAttribute(Policy = "DefaultUser")]
    public partial class UserProfileController : Controller
    {
        public const string ControllerRoute = "user";
        protected Services.IDatabaseContact DatabaseContact { get; private set; } = default!;
        protected IDbContextFactory<DatabaseContext> DatabaseFactory { get; private set; } = default!;

        protected readonly ILogger<UserProfileController> _logger = default!;
        public UserProfileController(Services.IDatabaseContact dbcontact, ControllerLogger logger, 
            IDbContextFactory<DatabaseContext> factory) : base() 
            { (this.DatabaseContact, this._logger, this.DatabaseFactory) = (dbcontact, logger, factory); }

        [HttpGetAttribute, RouteAttribute("profile", Name = "profile")]
        public async Task<IActionResult> ProfileInfo(UserProfileModel? model)
        {
            var authorizatedProfile = this.HttpContext.User.FindFirst(ClaimTypes.PrimarySid)!;
            this.ViewBag.IsAdmin = this.HttpContext.User.FindAll(ClaimTypes.Role)
                .Where(item => item.Value == "Admin").Count() > 0;
            if (model == null) model = new UserProfileModel();

            model.SearchQuery = model.SearchQuery == default ? default! : model.SearchQuery.Replace('+', ' ').Trim();
            model.FormRequestLink = $"{UserProfileController.ControllerRoute}/profileupdate";
            model.Contact = (await this.DatabaseContact.GetContact(int.Parse(authorizatedProfile.Value), model.SearchQuery))!;

            this.ViewBag.EditingModel = new ProfileEditingModel();
            if (model.Contact == null) { return base.LocalRedirect("/logout"); }

            model.Contact.FriendContactid1Navigations = model.Contact.FriendContactid1Navigations
                .Where(delegate (DAModels::Friend record)
            {
                var friendContact = record.Contactid2Navigation;
                return model.ProfileType switch
                {
                    "Все контакты" => true, "Созданные" => friendContact.Authorization == null,
                    "Аккаунты" => friendContact.Authorization != null,  _ => true,
                };
            }).ToImmutableList();
            model.PagesCount = (int)Math.Ceiling(model.Contact.FriendContactid1Navigations.Count()
                / (double)model.RecordOnPage);

            DAModels::Contact GetContact(DAModels::Friend record) => record.Contactid2Navigation;
            model.Contact.FriendContactid1Navigations = (model.SortingType switch
            {
                "Без сортировки" =>
                    model.Contact.FriendContactid1Navigations.OrderBy(item => GetContact(item).Contactid),
                "По дате изменения" =>
                    model.Contact.FriendContactid1Navigations.OrderBy(item => GetContact(item).Lastupdate.ToString()),
                "По дате рождения" =>
                    model.Contact.FriendContactid1Navigations.OrderBy(item => GetContact(item).Birthday.ToString()),

                "По имени" => model.Contact.FriendContactid1Navigations.OrderBy(item => GetContact(item).Name),
                _ => model.Contact.FriendContactid1Navigations.OrderBy(item => GetContact(item).Contactid)
            })
            .Skip(model.CurrentPage * model.RecordOnPage).Take(model.RecordOnPage).ToImmutableList();

            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                this.ViewBag.EditingModel.GenderTypes = await dbcontext.Gendertypes.ToListAsync();
                this.ViewBag.EditingModel.Cities = await dbcontext.Cities.ToListAsync();
                this.ViewBag.EditingModel.Pictures = await dbcontext.Userpictures.ToListAsync();

                this.ViewBag.EditingModel.QualityTypes = await dbcontext.Humanqualities.ToListAsync();
                this.ViewBag.EditingModel.HobbyTypes = await dbcontext.Hobbies.ToListAsync();
                this.ViewBag.EditingModel.Postes = await dbcontext.Posts.ToListAsync();
                this.ViewBag.EditingModel.Datingtypes = await dbcontext.Datingtypes.ToListAsync();
            }
            string viewbagSerialize = JsonConvert.SerializeObject(this.ViewBag.EditingModel,
                 new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            string modelSerialize = JsonConvert.SerializeObject(model,
                 new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            this.ViewBag.EditingModel = JsonConvert.DeserializeObject<ProfileEditingModel>(viewbagSerialize);
            return base.View(JsonConvert.DeserializeObject<UserProfileModel>(modelSerialize));
        }

        [HttpPostAttribute, RouteAttribute("profileupdate", Name = "profileupdate")]
        [ProfileModelFilter("profile")]
        public async Task<IActionResult> ProfileInfo([FromForm] DAModels::Contact contactModel)
        {
            if (contactModel is null) return base.RedirectToAction("ProfileInfo", "UserProfile");
            var errorMessage = await this.DatabaseContact.EditContact(contactModel);

            if(errorMessage != null) return base.RedirectToRoute("profile", new UserProfileModel()
            {
                HasError = true, ErrorMessage = errorMessage.Message, Mode = UserProfileModel.PageMode.Settings
            });
            return base.RedirectToAction("ProfileInfo", "UserProfile", new UserProfileModel() 
            { Mode = UserProfileModel.PageMode.Settings });
        }

        [HttpGetAttribute, RouteAttribute("deletecontact/{contactid}", Name = "deletecontact")]
        public async Task<IActionResult> RemoveContact(int contactid)
        {
            IDatabaseContact.ErrorStatus? errorMessage = default!;
            try {
                if ((errorMessage = await this.DatabaseContact.RemoveContact(contactid)) != null) 
                { base.RedirectToRoute("profile", new UserProfileModel() { ErrorMessage = errorMessage.Message }); }
            }
            catch(System.Exception error)
            { 
                return base.RedirectToRoute("profile", new UserProfileModel() { ErrorMessage = error.Message }); 
            }
            return base.RedirectToRoute("profile", new UserProfileModel() { });
        }
    }
}
