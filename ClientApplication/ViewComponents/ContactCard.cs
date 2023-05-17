using DatabaseAccess;
using DatabaseAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClientApplication.ViewComponents
{
    [ViewComponentAttribute]
    public partial class ContactCardViewComponent : ViewComponent
    {
        protected IDbContextFactory<DatabaseContext> DatabaseFactory { get; set; } = default!;
        public ContactCardViewComponent(IDbContextFactory<DatabaseContext> factory) : base() 
        { this.DatabaseFactory = factory; }

        public async Task<IViewComponentResult> InvokeAsync(Contact contactModel)
        {
            var profileId = int.Parse(this.HttpContext.User.FindFirst(ClaimTypes.PrimarySid)!.Value);
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                var friendRecord = dbcontext.Friends.FirstOrDefault(item => (item.Contactid2 == profileId 
                    && item.Contactid1 == contactModel.Contactid)
                    || (item.Contactid2 == contactModel.Contactid && item.Contactid1 == profileId));

                if (friendRecord == null) throw new Exception("Запись о контакте не найдена");
                await dbcontext.Datingtypes.LoadAsync();
                this.ViewBag.DatingType = friendRecord.Datingtype!.Typeofdating;
            }
            return base.View("ContactCard", contactModel);
        }
    }
}
