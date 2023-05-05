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
        public async Task<IActionResult> ProfileInfo()
        {
            var authorizatedProfile = this.HttpContext.User.FindFirst(ClaimTypes.PrimarySid)!;
            this.ViewBag.EditingModel = new ProfileEditingModel();

            DAModels::Contact profileModel = default!;
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                profileModel = await dbcontext.Contacts.Include(prop => prop.Gendertype)
                    .Include(prop => prop.Location).ThenInclude(prop => prop!.City)
                    .Include(prop => prop.Employees).ThenInclude(prop => prop!.Post)
                    .Include(prop => prop.Userpicture)
                    .Include(prop => prop.Humanqualities).Include(prop => prop.Hobbies)
                    .Where(item => item.Contactid == int.Parse(authorizatedProfile.Value)).FirstAsync();

                this.ViewBag.EditingModel.GenderTypes = await dbcontext.Gendertypes.ToListAsync();
                this.ViewBag.EditingModel.Cities = await dbcontext.Cities.ToListAsync();
                this.ViewBag.EditingModel.Pictures = await dbcontext.Userpictures.ToListAsync();
                this.ViewBag.EditingModel.QualityTypes = await dbcontext.Humanqualities.ToListAsync();
                this.ViewBag.EditingModel.HobbyTypes = await dbcontext.Hobbies.ToListAsync();
            }
            string viewbagSerialize = JsonConvert.SerializeObject(this.ViewBag.EditingModel,
                 new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            string modelSerialize = JsonConvert.SerializeObject(profileModel,
                 new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            this.ViewBag.EditingModel = JsonConvert.DeserializeObject<ProfileEditingModel>(viewbagSerialize);
            return base.View(JsonConvert.DeserializeObject<DAModels::Contact>(modelSerialize));
        }

        [HttpPostAttribute, RouteAttribute("profile", Name = "profile")]
        public async Task<IActionResult> ProfileInfo([FromForm] string name)
        {
            await Console.Out.WriteLineAsync(name);
            return base.RedirectToAction("ProfileInfo", "UserProfile");
        }
    }
}
