using DatabaseAccess;
using DatabaseAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;

namespace ClientApplication.ViewComponents
{
    [ViewComponentAttribute]
    public partial class FriendMessagesViewComponent : ViewComponent
    {
        protected IDbContextFactory<DatabaseContext> DatabaseFactory { get; set; } = default!;
        public FriendMessagesViewComponent(IDbContextFactory<DatabaseContext> factory) : base() 
        { this.DatabaseFactory = factory; }
        public async Task<IViewComponentResult> InvokeAsync(int profileId)
        {
            var currentId = int.Parse(this.HttpContext.User.FindFirst(ClaimTypes.PrimarySid)!.Value);
            List<Message> viewModel = new();
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                await dbcontext.Contacts.Where(item => item.Contactid == profileId).LoadAsync();
                var friendRecord = await dbcontext.Friends.FirstAsync(item =>  (item.Contactid1 == profileId 
                    && item.Contactid2 == currentId) || (item.Contactid2 == profileId && item.Contactid1 == currentId));

                if (friendRecord == null) return base.View("MessagesView", new List<Message>() { });
                viewModel = dbcontext.Messages.Where(item => item.Friendid == friendRecord.Friendid)
                    .OrderBy(item => item.Sendtime).ToList();
            }
            return base.View("MessagesView", viewModel);
        }
    }
}
