using ClientApplication.Filters;
using ClientApplication.ViewModels;
using DatabaseAccess;
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

    [ControllerAttribute, RouteAttribute("contact"), AuthorizeAttribute(Policy = "DefaultUser")]
    public partial class UserContactsController : Controller
    {
        protected Services.IDatabaseContact DatabaseContact { get; private set; } = default!;
        protected IDbContextFactory<DatabaseContext> DatabaseFactory { get; private set; } = default!;

        protected readonly ILogger<UserContactsController> _logger = default!;
        public UserContactsController(Services.IDatabaseContact dbcontact, ControllerLogger logger,
            IDbContextFactory<DatabaseContext> factory) : base()
        { (this.DatabaseContact, this._logger, this.DatabaseFactory) = (dbcontact, logger, factory); }

        [HttpGetAttribute, RouteAttribute("buildcontact", Name = "buildcontact")]
        public async Task<IActionResult> BuildContact(UserContactsModel? model)
        {
            if (model == null) model = new UserContactsModel();
            model.Contact = await this.DatabaseContact.InitializeContact();

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
        public async Task<IActionResult> AddContact([FromForm] DAModels::Contact contactModel,
            [FromForm]string datingType)
        {
            return base.RedirectToRoute("buildcontact");
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

    }
}
