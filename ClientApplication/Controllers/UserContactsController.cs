using ClientApplication.Filters;
using ClientApplication.Services;
using ClientApplication.ViewModels;
using DatabaseAccess;
using DatabaseAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ClientApplication.Controllers
{
    using DAModels = DatabaseAccess.Models;
    using ControllerLogger = ILogger<UserContactsController>;

    [ControllerAttribute, RouteAttribute(ControllerRoute), AuthorizeAttribute(Policy = "DefaultUser")]
    public partial class UserContactsController : Controller
    {
        public const string ControllerRoute = "contact";
        protected Services.IDatabaseContact DatabaseContact { get; private set; } = default!;
        protected IDbContextFactory<DatabaseContext> DatabaseFactory { get; private set; } = default!;

        protected readonly ILogger<UserContactsController> _logger = default!;
        public UserContactsController(Services.IDatabaseContact dbcontact, ControllerLogger logger,
            IDbContextFactory<DatabaseContext> factory) : base()
        { (this.DatabaseContact, this._logger, this.DatabaseFactory) = (dbcontact, logger, factory); }

        [HttpGetAttribute, RouteAttribute("buildcontact", Name = "buildcontact")]
        public async Task<IActionResult> BuildContact(UserContactsModel model)
        {
            if (model.SelectedContact == default) model.Contact = await DatabaseContact.InitializeContact();
            else model.Contact = (await this.DatabaseContact.GetContact(model.SelectedContact, string.Empty))!;

            Console.WriteLine($"\nselected: {model.SelectedContact}");
            Console.WriteLine($"\nmodel error: {model.ErrorMessage}");

            model.FormRequestLink = string.Format("{0}/{1}", UserContactsController.ControllerRoute,
                model.SelectedContact == default ? "addcontact" : "editcontact");

            Console.WriteLine($"\nFormRequestLink:{model.FormRequestLink}");

            if (model!.Contact == null) return base.RedirectToRoute("profile", new UserProfileModel()
            { ErrorMessage = "Контакт не найден" });
            else model.IsAccount = model.Contact.Authorization != null;

            Console.WriteLine($"\nerror: {model.ErrorMessage}");

            var authorizatedProfile = this.HttpContext.User.FindFirst(ClaimTypes.PrimarySid)!;
            var friend = model.Contact.FriendContactid1Navigations.FirstOrDefault(item =>
                item.Contactid2Navigation.Contactid == int.Parse(authorizatedProfile.Value));

            if (friend != null) model.DatingType = friend.Datingtype!;
            this.ViewBag.EditingModel = new ProfileEditingModel();
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

            string modelSerialize = JsonConvert.SerializeObject(model, new JsonSerializerSettings() 
                { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            this.ViewBag.EditingModel = JsonConvert.DeserializeObject<ProfileEditingModel>(viewbagSerialize);
            return base.View(JsonConvert.DeserializeObject<UserContactsModel>(modelSerialize));
        }

        [HttpPostAttribute, RouteAttribute("addcontact", Name = "addcontact")]
        [ProfileModelFilter("buildcontact")]
        public async Task<IActionResult> AddContact([FromForm] DAModels::Contact contactModel,
            [FromForm]string datingtype)
        {
            var authorizatedProfile = this.HttpContext.User.FindFirst(ClaimTypes.PrimarySid)!;

            Console.WriteLine($"\nid1:{contactModel}, id2: {int.Parse(authorizatedProfile.Value)}");
            var resultModel = new ViewModels.UserProfileModel()
            {
                Mode = UserBaseModel.PageMode.Contacts, ErrorMessage = default!, HasError = default,
            };
            var error = await this.DatabaseContact.AddContact(contactModel, int.Parse(authorizatedProfile.Value),
                new DAModels::Datingtype() { Typeofdating = datingtype });

            if ((resultModel.ErrorMessage = error?.Message) != null) return base.RedirectToRoute("profile", resultModel);
            resultModel.ErrorMessage = string.Format("Контакт успешно создан");

            return base.RedirectToRoute("profile", resultModel);
        }

        [HttpPostAttribute, RouteAttribute("editcontact", Name = "editcontact")]
        [ProfileModelFilter("buildcontact")]
        public async Task<IActionResult> EditContact([FromForm] DAModels::Contact contactModel,
            [FromForm] string datingtype)
        {
            var profileId = this.HttpContext.User.FindFirst(ClaimTypes.PrimarySid)!;
            var record = await this.DatabaseContact.GetContact(contactModel.Contactid, string.Empty);

            if (record == null) return base.RedirectToRoute("profile", new ViewModels.UserProfileModel()
            { Mode = UserBaseModel.PageMode.Contacts, ErrorMessage = "Контакт не найден" });

            var errorModel = default(IDatabaseContact.ErrorStatus);
            if(record.Authorization == null && (errorModel = await this.DatabaseContact.EditContact(contactModel)) != null)
            {
                return base.RedirectToRoute("profile", new ViewModels.UserProfileModel()
                { Mode = UserBaseModel.PageMode.Contacts, ErrorMessage = errorModel.Message });
            }
            if(datingtype != null) errorModel = await this.DatabaseContact.SetFriendShip(int.Parse(profileId.Value),
                record.Contactid, new DAModels::Datingtype() { Typeofdating = datingtype });

            if (errorModel != null) base.RedirectToRoute("buildcontact", new UserContactsModel()
            { SelectedContact = contactModel.Contactid, ErrorMessage = errorModel.Message });

            return base.RedirectToRoute("buildcontact", new UserContactsModel() 
            { SelectedContact = contactModel.Contactid });
        }

        [HttpPostAttribute, RouteAttribute("referencelink", Name = "referencelink")]
        public async Task<IActionResult> UseReferenceLink([FromForm]string link, [FromForm]string datingtype)
        {
            var profileId = this.HttpContext.User.FindFirst(ClaimTypes.PrimarySid)!;
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                var record = await dbcontext.Contacts.Include(item => item.Authorization)
                    .Where(item => item.Authorization != null).FirstOrDefaultAsync(
                    item => item.Emailaddress == link || item.Authorization!.Referenceguid == link);

                if(record == null) return base.RedirectToRoute("profile", new UserProfileModel()
                { Mode = UserProfileModel.PageMode.Contacts, HasError = true, ErrorMessage = "Контакт не найден" });

                var error1 = await this.DatabaseContact.SetFriendShip(int.Parse(profileId.Value), record.Contactid,
                    new DAModels::Datingtype() { Typeofdating = datingtype });
                var error2 = await this.DatabaseContact.SetFriendShip(record.Contactid, int.Parse(profileId.Value),
                    new DAModels::Datingtype() { Typeofdating = datingtype });

                if (error1 != null || error2 != null) return base.RedirectToRoute("profile", new UserProfileModel()
                { Mode = UserProfileModel.PageMode.Contacts, HasError = true, ErrorMessage = error1?.Message ?? error2?.Message });
            }
            return base.RedirectToRoute("profile", new UserProfileModel()
            { Mode = UserProfileModel.PageMode.Contacts, HasError = false, ErrorMessage = "Связь установлена" });
        }

        [HttpPostAttribute, RouteAttribute("textmessage", Name = "textmessage")]
        public async Task<IActionResult> TextMessage([FromForm] string text, int friend)
        {
            if(text == null) return base.RedirectToRoute("buildcontact", new UserContactsModel() { SelectedContact = friend });

            var profileId = int.Parse(this.HttpContext.User.FindFirst(ClaimTypes.PrimarySid)!.Value);
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                var friendRecord = await dbcontext.Friends.FirstAsync(item => (item.Contactid1 == profileId
                    && item.Contactid2 == friend) || (item.Contactid2 == profileId && item.Contactid1 == friend));

                dbcontext.Messages.AddRange(new DAModels::Message() 
                { 
                    Sendtime = DateTime.Now, Messagebody = text, Friendid = friendRecord.Friendid, Contactid = profileId
                });
                await dbcontext.SaveChangesAsync();
            }
            return base.RedirectToRoute("buildcontact", new UserContactsModel() { SelectedContact = friend });
        }
    }
}
