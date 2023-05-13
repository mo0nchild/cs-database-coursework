using ClientApplication.Filters;
using ClientApplication.ViewModels;
using DatabaseAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;

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

            Console.WriteLine($"current_page: {model?.CurrentPage}");
            Console.WriteLine($"sortingtype: {model?.SortingType}");
            Console.WriteLine($"profile_type: {model?.ProfileType}");
            Console.WriteLine($"search_query: {model?.SearchQuery}");
            Console.WriteLine($"search_query: {model?.SearchQuery}");

            if (model == null) model = new UserProfileModel();
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                model.Contact = (await dbcontext.Contacts.Include(prop => prop.Gendertype)
                    .Include(prop => prop.Location).ThenInclude(prop => prop!.City)
                    .Include(prop => prop.Employees).ThenInclude(prop => prop!.Post)

                    .Include(prop => prop.FriendContactid1Navigations).ThenInclude(prop => prop.Contactid2Navigation)
                        .ThenInclude(prop => prop.Authorization)
                    .Include(prop => prop.Userpicture).Include(prop => prop.Authorization)

                    .Include(prop => prop.Humanqualities).Include(prop => prop.Hobbies)
                    .Where(item => item.Contactid == int.Parse(authorizatedProfile.Value)).FirstOrDefaultAsync())!;
                //if (model.Contact == null) { return base.LocalRedirect("/logout"); }

                model.SearchQuery = model.SearchQuery == default ? default! : model.SearchQuery.Replace('+', ' ').Trim();
                model.Contact.FriendContactid1Navigations = model.Contact.FriendContactid1Navigations
                    .Where(delegate (DAModels::Friend record)
                {
                    var friendContact = record.Contactid2Navigation;
                    return model.SearchQuery == default || Regex.IsMatch(friendContact.Emailaddress, model.SearchQuery)
                        || Regex.IsMatch($"{friendContact.Name} {friendContact.Surname}", model.SearchQuery)
                        || (friendContact.Phonenumber != null && Regex.IsMatch(friendContact.Phonenumber, model.SearchQuery));
                })
                .ToList().Where(delegate (DAModels::Friend record)
                {
                    var friendContact = record.Contactid2Navigation;
                    return model.ProfileType switch { "Все контакты" => true, "Созданные" => friendContact.Authorization == null,
                        "Аккаунты" => friendContact.Authorization != null, _ => true, 
                    };
                }).ToImmutableList();
                model.PagesCount = (int)Math.Ceiling(model.Contact.FriendContactid1Navigations.Count() 
                    / (double)model.RecordOnPage);

                DAModels::Contact GetContact(DAModels::Friend record) => record.Contactid2Navigation;
                model.Contact.FriendContactid1Navigations = (model.SortingType switch {
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

        [HttpPostAttribute, RouteAttribute("profile_update", Name = "profile_update")]
        [ProfileModelFilter("profile_update")]
        public async Task<IActionResult> ProfileInfo([FromForm] DAModels::Contact contactModel)
        {
            if (contactModel is null) return base.RedirectToAction("ProfileInfo", "UserProfile");
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                dbcontext.Contacts.Where(record => record.Contactid == contactModel.Contactid).ExecuteUpdate(prop => prop
                    .SetProperty(item => item.Surname, item => contactModel.Surname)
                    .SetProperty(item => item.Name, item => contactModel.Name)
                    .SetProperty(item => item.Patronymic, item => contactModel.Patronymic)
                    .SetProperty(item => item.Birthday, item => contactModel.Birthday)

                    .SetProperty(item => item.Emailaddress, item => contactModel.Emailaddress)
                    .SetProperty(item => item.Phonenumber, item => contactModel.Phonenumber)
                    .SetProperty(item => item.Familystatus, item => contactModel.Familystatus));

                var userContact = await dbcontext.Contacts.Include(prop => prop.Employees).Include(prop => prop.Hobbies)
                    .Include(prop => prop.Humanqualities).FirstAsync(item => item.Contactid == contactModel.Contactid);

                if (userContact == null) return base.RedirectToRoute("profile", new UserProfileModel()
                {
                    HasError = true, ErrorMessage = "Пользователь не найден",
                });
                userContact.Gendertype = (await dbcontext.Gendertypes.Where((DAModels::Gendertype item)
                    => item.Gendertypename == contactModel.Gendertype.Gendertypename).FirstOrDefaultAsync())!;

                userContact.Userpicture = (await dbcontext.Userpictures.Where((DAModels::Userpicture item)
                    => item.Filepath == contactModel.Userpicture!.Filepath).FirstOrDefaultAsync());

                var contactLocation = await dbcontext.Locations.FindAsync(userContact.Locationid);
                var contactCity = await dbcontext.Cities
                            .FirstAsync(city => city.Cityname == contactModel.Location!.City.Cityname);
                if (contactModel.Location != null)
                {
                    var newLocation = new DAModels::Location()
                    {
                        City = contactCity, Street = contactModel.Location.Street 
                    };
                    if (contactLocation == null) { await dbcontext.AddAsync(userContact.Location = newLocation); }
                    else { contactLocation.Street = newLocation.Street; contactLocation.City = contactCity; }
                }
                await dbcontext.SaveChangesAsync();

                var result = await dbcontext.Employees.Where(item => item.Contactid == contactModel.Contactid)
                    .ExecuteDeleteAsync();
                await dbcontext.SaveChangesAsync();

                foreach (DAModels::Employee employee in contactModel.Employees)
                {
                    var contactPost = await dbcontext.Posts.FirstAsync(city => city.Postname == employee.Post!.Postname);
                    var newEmployee = new DAModels::Employee()
                    {
                        Contactid = userContact.Contactid, Post = contactPost,
                        Status = employee.Status, Companyname = employee.Companyname,
                    };
                    dbcontext.Employees.Add(newEmployee);
                }
                userContact.Hobbies.Clear(); userContact.Humanqualities.Clear();
                foreach (DAModels::Hobby hobby in contactModel.Hobbies)
                {
                    userContact.Hobbies.Add(await dbcontext.Hobbies.FirstAsync(item => item.Hobbyname == hobby.Hobbyname));
                }
                dbcontext.Contacts.Update(userContact); 
                foreach (DAModels::Humanquality hobby in contactModel.Humanqualities)
                {
                    userContact.Humanqualities.Add(
                        await dbcontext.Humanqualities.FirstAsync(item => item.Qualityname == hobby.Qualityname));
                }
                dbcontext.Contacts.Update(userContact);
                await dbcontext.SaveChangesAsync();
            }
            return base.RedirectToAction("ProfileInfo", "UserProfile");
        }
    }
}
