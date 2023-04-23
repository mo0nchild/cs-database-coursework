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
            DAModels::Contact profileModel = default!;
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                profileModel = await dbcontext.Contacts.Include(prop => prop.Gendertype)
                    .Include(prop => prop.Location).ThenInclude(prop => prop!.City)
                    .Include(prop => prop.Employees)
                    .Where(item => item.Contactid == int.Parse(authorizatedProfile.Value)).FirstAsync();
            }
            var modelSerialize = JsonConvert.SerializeObject(profileModel,
                 new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return base.View(JsonConvert.DeserializeObject<DAModels::Contact>(modelSerialize));
        }

        [HttpPostAttribute, RouteAttribute("profile", Name = "profile")]
        public async Task<IActionResult> ProfileInfo([FromForm] string name)
        {
            await Console.Out.WriteLineAsync(name);
            return base.RedirectToAction("ProfileInfo", "UserProfile");
        }

        //[HttpGetAttribute, RouteAttribute("loadContacts", Name = "profile")]
        //[AuthorizeAttribute(Policy = "DefaultUser")]
        //public async Task<IActionResult> GetContactsList([FromRoute]ContactInfoModel.PageDisplay pageDisplay)
        //{
        //    var profileContacts = new List<DAModels::Contact>();
        //    using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
        //    {
        //        var pagesCount = Math.Ceiling(dbcontext.Contacts.Count() / (double)pageDisplay.ItemAmount);
        //        var selectedItems = dbcontext.Contacts
        //            .Skip(pageDisplay.ItemAmount * pageDisplay.PageAmount)
        //            .Take(pageDisplay.ItemAmount);
        //        foreach (var item in await selectedItems.ToListAsync()) profileContacts.Add(item);
        //    }
        //    return base.RedirectToAction("DetailsInfo", new ContactInfoModel()
        //    { Contacts = profileContacts, PageDiplay = pageDisplay });
        //}
    }
}
