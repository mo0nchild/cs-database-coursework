using ClientApplication.Controllers;
using DatabaseAccess;
using DatabaseAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientApplication.Services
{
    using DatabaseContactLogger = ILogger<UserContactsController>;
    using DAModels = DatabaseAccess.Models;
    public interface IDataContact : System.IDisposable
    {
        public void EditContact(System.Int32 contactId, DAModels::Contact contact);
        public void RemoveContact(System.Int32 contactId);
    }
    public partial class DatabaseContact : System.Object, Services.IDataContact
    {
        protected IDbContextFactory<DatabaseContext> DatabaseFactory { get; private set; } = default!;

        protected readonly DatabaseContactLogger Logger = default!;
        public DatabaseContact(IDbContextFactory<DatabaseContext> factory, DatabaseContactLogger logger) 
            : base() { this.DatabaseFactory = factory; this.Logger = logger; }

        public void Dispose() { this.Logger.LogInformation("DatabaseContact Disposed"); }

        public void EditContact(int contactId, Contact contact)
        {
            
        }

        public void RemoveContact(int contactId)
        {
            
        }

        
    }
}
