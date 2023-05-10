using ClientApplication.ViewModels;
using DatabaseAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Claims;

namespace ClientApplication.Controllers
{
    using DAModels = DatabaseAccess.Models;
    using ControllerLogger = ILogger<UserProfileController>;

    [ControllerAttribute, RouteAttribute("user"), AuthorizeAttribute(Policy = "DefaultUser")]
    public partial class UserProfileController : Controller
    {
        protected IDbContextFactory<DatabaseContext> DatabaseFactory { get; private set; } = default!;
        private readonly ILogger<UserProfileController> _logger;
        public UserProfileController(IDbContextFactory<DatabaseContext> factory, ControllerLogger logger)
            : base() { (this.DatabaseFactory, this._logger) = (factory, logger); }

        [HttpGetAttribute, RouteAttribute("profile", Name = "profile")]
        public async Task<IActionResult> ProfileInfo(UserProfileModel? model)
        {
            var authorizatedProfile = this.HttpContext.User.FindFirst(ClaimTypes.PrimarySid)!;
            this.ViewBag.EditingModel = new ProfileEditingModel();

            if (model == null) model = new UserProfileModel();
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                model.Contact = (await dbcontext.Contacts.Include(prop => prop.Gendertype)
                    .Include(prop => prop.Location).ThenInclude(prop => prop!.City)
                    .Include(prop => prop.Employees).ThenInclude(prop => prop!.Post)
                    .Include(prop => prop.Userpicture)
                    .Include(prop => prop.Authorization)
                    //.Include(prop => prop.FriendContactid1Navigations).ThenInclude(prop => prop.Contactid2Navigation)


                    .Include(prop => prop.Humanqualities).Include(prop => prop.Hobbies)
                    .Where(item => item.Contactid == int.Parse(authorizatedProfile.Value)).FirstOrDefaultAsync())!;
                //if (model.Contact == null) { return base.LocalRedirect("/logout"); }

                this.ViewBag.EditingModel.GenderTypes = await dbcontext.Gendertypes.ToListAsync();
                this.ViewBag.EditingModel.Cities = await dbcontext.Cities.ToListAsync();
                this.ViewBag.EditingModel.Pictures = await dbcontext.Userpictures.ToListAsync();
                this.ViewBag.EditingModel.QualityTypes = await dbcontext.Humanqualities.ToListAsync();
                this.ViewBag.EditingModel.HobbyTypes = await dbcontext.Hobbies.ToListAsync();
                this.ViewBag.EditingModel.Postes = await dbcontext.Posts.ToListAsync();
            }
            string viewbagSerialize = JsonConvert.SerializeObject(this.ViewBag.EditingModel,
                 new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            string modelSerialize = JsonConvert.SerializeObject(model,
                 new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            this.ViewBag.EditingModel = JsonConvert.DeserializeObject<ProfileEditingModel>(viewbagSerialize);
            return base.View(JsonConvert.DeserializeObject<UserProfileModel>(modelSerialize));
        }

        [HttpPostAttribute, RouteAttribute("profile", Name = "profile")]
        public async Task<IActionResult> ProfileInfo([FromForm] DAModels::Contact contactModel)
        {
            await Console.Out.WriteLineAsync();
            return base.RedirectToAction("ProfileInfo", "UserProfile");
        }
    }
}
