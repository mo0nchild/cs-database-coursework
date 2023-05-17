using ClientApplication.Filters;
using ClientApplication.Services;
using ClientApplication.ViewModels;
using DatabaseAccess;
using DatabaseAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ClientApplication.Controllers
{
    using ControllerLogger = ILogger<AdministratorController>;

    [ControllerAttribute, RouteAttribute(ControllerRoute), AuthorizeAttribute(Policy = "Administrator")]
    public partial class AdministratorController : Controller
    {
        public const string ControllerRoute = "admin";
        protected Services.IDatabaseContact DatabaseContact { get; private set; } = default!;
        protected IDbContextFactory<DatabaseContext> DatabaseFactory { get; private set; } = default!;

        protected readonly ILogger<AdministratorController> _logger = default!;
        public AdministratorController(Services.IDatabaseContact dbcontact, ControllerLogger logger,
            IDbContextFactory<DatabaseContext> factory) : base()
        { (this.DatabaseContact, this._logger, this.DatabaseFactory) = (dbcontact, logger, factory); }

        [HttpGetAttribute, RouteAttribute("adminpanel", Name = "adminpanel")]
        public async Task<IActionResult> AdministratorPanel(AdministratorModel? model)
        {
            var profileId = int.Parse(this.HttpContext.User.FindFirst(ClaimTypes.PrimarySid)!.Value);

            if (model == null) model = new AdministratorModel();
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                model.ContactsList = dbcontext.Contacts.Include(item => item.Gendertype)
                    .Include(item => item.Authorization).Include(item => item.Userpicture)
                    .Where(item => item.Contactid != profileId).OrderBy(item => item.Contactid).ToList();

                model.ContactsList = model.ContactsList.Where(item => model.ProfileType switch
                {
                    "Все контакты" => true, "Созданные" => item.Authorization == null,
                    "Аккаунты" => item.Authorization != null, _ => true,
                }).ToList();

                model.ContactsList = (model.SortingType switch
                {
                    "Без сортировки" => model.ContactsList.OrderBy(item => item.Contactid),
                    "По дате изменения" => model.ContactsList.OrderBy(item => item.Lastupdate),
                    "По дате рождения" => model.ContactsList.OrderBy(item => item.Birthday),

                    "По имени" => model.ContactsList.OrderBy(item => item.Name),
                    _ => model.ContactsList.OrderBy(item => item.Contactid)
                }).ToList();
            }
            return base.View(model);
        }

        [HttpGetAttribute, RouteAttribute("contacteditor", Name = "contacteditor")]
        public async Task<IActionResult> ContactEditor(AdministratorModel model)
        {
            model.Contact = (await this.DatabaseContact.GetContact(model.SelectedRecord, string.Empty))!;
            model.FormRequestLink = $"admin/updatecontact";

            if (model.Contact == null) return base.RedirectToRoute("profile", new ViewModels.UserProfileModel()
            { Mode = UserBaseModel.PageMode.Contacts, ErrorMessage = "Контакт не найден" });

            string modelSerialize = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
            { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return base.View(JsonConvert.DeserializeObject<AdministratorModel>(modelSerialize));
        }

        [HttpPostAttribute, RouteAttribute("updatecontact", Name = "updatecontact")]
        [ProfileModelFilter("contacteditor")]
        public async Task<IActionResult> UpdateContact(Contact contactModel)
        {
            IDatabaseContact.ErrorStatus? errorMessage = default!;
            var resultModel = new AdministratorModel() { };

            if ((errorMessage = await this.DatabaseContact.EditContact(contactModel)) != null)
            {
                resultModel.ErrorMessage = errorMessage.Message;
            }
            return base.RedirectToRoute("contacteditor", new AdministratorModel()
            { SelectedRecord = contactModel.Contactid });
        }

        [HttpGetAttribute, RouteAttribute("getusers", Name = "getusers")]
        public async Task<IActionResult> GetDocument([FromServices] IDocumentContact generator)
        {
            List<Contact> contacsList = default!;
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                contacsList = await dbcontext.Contacts.Include(item => item.Gendertype).ToListAsync();
            }
            return base.File(await generator.GetDocument(contacsList), "application/octet-stream", "report.xlsx");
        }
    }
}
