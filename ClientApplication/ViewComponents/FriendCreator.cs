using DatabaseAccess.Models;
using DatabaseAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClientApplication.ViewComponents
{
    [ViewComponentAttribute]
    public partial class FriendCreatorViewComponent : ViewComponent
    {
        protected IDbContextFactory<DatabaseContext> DatabaseFactory { get; set; } = default!;
        public FriendCreatorViewComponent(IDbContextFactory<DatabaseContext> factory) : base()
        { this.DatabaseFactory = factory; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Datingtype> datingTypes = default!;
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                datingTypes = await dbcontext.Datingtypes.ToListAsync();
            }
            return base.View("FriendCreator", datingTypes);
        }
    }
}
