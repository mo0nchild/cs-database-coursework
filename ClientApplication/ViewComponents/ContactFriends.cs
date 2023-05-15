using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatabaseAccess;
using DatabaseAccess.Models;
using System.Collections.Immutable;

namespace ClientApplication.ViewComponents
{
    [ViewComponentAttribute]
    public partial class ContactFriendsViewComponent : ViewComponent
    {
        protected Services.IDatabaseContact DatabaseContact { get; private set; } = default!;
        public ContactFriendsViewComponent(Services.IDatabaseContact factory) : base()
        { this.DatabaseContact = factory; }
        public async Task<IViewComponentResult> InvokeAsync(int profileId)
        {
            var recordContact = await this.DatabaseContact.GetContact(profileId, string.Empty);
            return base.View("FriendView", recordContact == null ? new List<Contact>() { }
                : recordContact.FriendContactid1Navigations
                .Select(item => item.Contactid2Navigation).ToList());
        }
    }
}
