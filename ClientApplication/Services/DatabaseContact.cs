using ClientApplication.Controllers;
using ClientApplication.ViewModels;
using DatabaseAccess;
using DatabaseAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace ClientApplication.Services
{
    using DatabaseContactLogger = ILogger<UserContactsController>;
    using DAModels = DatabaseAccess.Models;
    public interface IDatabaseContact : System.IDisposable
    {
        public abstract Task<IDatabaseContact.ErrorStatus?> SetFriendShip(int contactId1, int contactId2,
            DAModels::Datingtype datingtype);
        public abstract Task<DAModels::Contact?> GetContact(int contactId, string search);

        public abstract Task<IDatabaseContact.ErrorStatus?> EditContact(DAModels::Contact contactModel);
        public abstract Task<IDatabaseContact.ErrorStatus?> RemoveContact(int contactId);

        public abstract Task<DAModels::Contact> InitializeContact();
        public record class ErrorStatus(string Message);
    }
    public partial class DatabaseContact : System.Object, Services.IDatabaseContact
    {
        protected IDbContextFactory<DatabaseContext> DatabaseFactory { get; private set; } = default!;

        protected readonly DatabaseContactLogger Logger = default!;
        public DatabaseContact(IDbContextFactory<DatabaseContext> factory, DatabaseContactLogger logger) 
            : base() { this.DatabaseFactory = factory; this.Logger = logger; }

        public void Dispose() { this.Logger.LogInformation("DatabaseContact Disposed"); }
        public async Task<IDatabaseContact.ErrorStatus?> EditContact(DAModels::Contact contactModel)
        {
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                dbcontext.Contacts.Where(record => record.Contactid == contactModel.Contactid).ExecuteUpdate(prop => prop
                    .SetProperty(item => item.Surname, item => contactModel.Surname)
                    .SetProperty(item => item.Name, item => contactModel.Name)
                    .SetProperty(item => item.Patronymic, item => contactModel.Patronymic)
                    .SetProperty(item => item.Birthday, item => contactModel.Birthday)

                    .SetProperty(item => item.Emailaddress, item => contactModel.Emailaddress)
                    .SetProperty(item => item.Phonenumber, item => contactModel.Phonenumber)
                    .SetProperty(item => item.Lastupdate, item => DateTime.Now)
                    .SetProperty(item => item.Familystatus, item => contactModel.Familystatus));

                var userContact = await dbcontext.Contacts.Include(prop => prop.Employees).Include(prop => prop.Hobbies)
                    .Include(prop => prop.Humanqualities).FirstAsync(item => item.Contactid == contactModel.Contactid);

                if (userContact == null) return new IDatabaseContact.ErrorStatus("Запись о контакте не найдена");

                userContact.Gendertype = (await dbcontext.Gendertypes.Where((DAModels::Gendertype item)
                    => item.Gendertypename == contactModel.Gendertype.Gendertypename).FirstOrDefaultAsync())!;

                userContact.Userpicture = (await dbcontext.Userpictures.Where((DAModels::Userpicture item)
                    => item.Filepath == contactModel.Userpicture!.Filepath).FirstOrDefaultAsync());

                var contactLocation = await dbcontext.Locations.FindAsync(userContact.Locationid ?? 0);
                if (contactModel.Location != null)
                {
                    var contactCity = await dbcontext.Cities.FirstAsync((DAModels::City city) => city.Cityname
                        == contactModel.Location.City.Cityname);
                    var newLocation = new DAModels::Location() { City = contactCity, Street = contactModel.Location.Street };

                    if (contactLocation == null) { await dbcontext.AddAsync(userContact.Location = newLocation); }
                    else { contactLocation.Street = newLocation.Street; contactLocation.City = contactCity; }
                }
                else if(userContact.Locationid != null)
                {
                    await dbcontext.Locations.Where((DAModels::Location item) => item.Locationid ==
                        (userContact.Locationid)).ExecuteDeleteAsync();

                    userContact.Locationid = null; userContact.Location = default!;
                } 
                await dbcontext.SaveChangesAsync();

                var result = await dbcontext.Employees.Where((DAModels::Employee item) => item.Contactid == 
                    contactModel.Contactid).ExecuteDeleteAsync();
                await dbcontext.SaveChangesAsync();
                foreach (DAModels::Employee employee in contactModel.Employees)
                {
                    var contactPost = await dbcontext.Posts.FirstAsync(city => city.Postname == employee.Post!.Postname);
                    var newEmployee = new DAModels::Employee()
                    {
                        Contactid = userContact.Contactid, Post = contactPost, Status = employee.Status,
                        Companyname = employee.Companyname,
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
                dbcontext.Contacts.Update(userContact); await dbcontext.SaveChangesAsync();
            }
            return default(IDatabaseContact.ErrorStatus);
        }

        public async Task<IDatabaseContact.ErrorStatus?> RemoveContact(int contactId)
        {
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                
            }
            return default(IDatabaseContact.ErrorStatus);
        }

        public async Task<IDatabaseContact.ErrorStatus?> SetFriendShip(int contactId1, int contactId2,
            DAModels::Datingtype datingtype)
        {
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                var profile1 = await dbcontext.Contacts.Include(item => item.FriendContactid1Navigations)
                    .FirstOrDefaultAsync(item => item.Contactid == contactId1);

                var profile2 = await dbcontext.Contacts.Include(item => item.FriendContactid1Navigations)
                    .FirstOrDefaultAsync(item => item.Contactid == contactId2);
                if(profile1 == null || profile2 == null) return new IDatabaseContact.ErrorStatus("Контакт не найден");

                var collision1 = profile1.FriendContactid1Navigations.Where((DAModels::Friend item) => item.Contactid2
                    == profile2.Contactid).Count();
                var collision2 = profile2.FriendContactid1Navigations.Where((DAModels::Friend item) => item.Contactid2
                    == profile1.Contactid).Count();

                if (collision1 > 0 && collision2 > 0) return new IDatabaseContact.ErrorStatus("Контакты уже связаны");

                var datingRecord = await dbcontext.Datingtypes.FirstOrDefaultAsync(item => item.Typeofdating ==
                    datingtype.Typeofdating);
                if (datingRecord == null || (profile1.Contactid == profile2.Contactid))
                {
                    return new IDatabaseContact.ErrorStatus("Связь не возможно установить");
                }
                var resultModel = new DAModels::Friend()
                {
                    Contactid1 = contactId1, Contactid2 = contactId2, Datingtype = datingRecord, 
                    Starttime = DateOnly.FromDateTime(DateTime.Now)
                };
                await dbcontext.Friends.AddAsync(resultModel); await dbcontext.SaveChangesAsync();
            }
            return default(IDatabaseContact.ErrorStatus);
        }

        public async Task<DAModels::Contact?> GetContact(int contactId, string searchQuery)
        {
            DAModels::Contact contactModel = default!;
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                contactModel = (await dbcontext.Contacts.Include(prop => prop.Gendertype)
                    .Include(prop => prop.Location).ThenInclude(prop => prop!.City)
                    .Include(prop => prop.Employees).ThenInclude(prop => prop!.Post)
                    .Include(prop => prop.FriendContactid1Navigations).ThenInclude(prop => prop.Contactid2Navigation)
                        .ThenInclude(prop => prop.Authorization)

                    .Include(prop => prop.Userpicture).Include(prop => prop.Authorization)
                    .Include(prop => prop.Humanqualities).Include(prop => prop.Hobbies)
                    .Where(item => item.Contactid == contactId).FirstOrDefaultAsync())!;

                if (contactModel == null) { return default(DAModels::Contact); }
                foreach(DAModels::Friend record in contactModel.FriendContactid1Navigations)
                {
                    await dbcontext.Userpictures.Where((DAModels::Userpicture item) => item.Userpictureid ==
                        record.Contactid2Navigation.Userpictureid).LoadAsync();

                    await dbcontext.Datingtypes.Where((DAModels::Datingtype item) => item.Datingtypeid ==
                        record.Datingtypeid).LoadAsync();
                }
                contactModel.FriendContactid1Navigations = contactModel.FriendContactid1Navigations
                    .Where(record => SelectionExpression(record.Contactid2Navigation)).ToList();
            }
            bool SelectionExpression(DAModels::Contact record)
            {
                return searchQuery == default || Regex.IsMatch(record.Emailaddress, searchQuery)
                    || Regex.IsMatch($"{record.Name} {record.Surname}", searchQuery)
                    || (record.Phonenumber != null && Regex.IsMatch(record.Phonenumber, searchQuery)
                );
            }
            return (DAModels::Contact) contactModel;
        }

        public async Task<DAModels::Contact> InitializeContact()
        {
            var resultModel = new DAModels::Contact()
            {
                Birthday = DateTime.Now, Emailaddress = string.Empty, Phonenumber = string.Empty,
                Surname = string.Empty, Name = string.Empty, Patronymic = string.Empty, 
            };
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                resultModel.Gendertype = await dbcontext.Gendertypes.FirstAsync();
                resultModel.Userpicture = await dbcontext.Userpictures.FirstAsync();
            }
            return (DAModels::Contact) resultModel;
        }
    }
}
