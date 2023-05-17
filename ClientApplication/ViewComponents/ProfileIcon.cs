using DatabaseAccess;
using DatabaseAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace ClientApplication.ViewComponents
{
    [ViewComponentAttribute]
    public partial class ProfileIconViewComponent : ViewComponent
    {
        protected IDbContextFactory<DatabaseContext> DatabaseFactory { get; set; } = default!;
        public ProfileIconViewComponent(IDbContextFactory<DatabaseContext> factory) : base() 
        { this.DatabaseFactory = factory; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var profileId = int.Parse(this.HttpContext.User.FindFirst(ClaimTypes.PrimarySid)!.Value);
            Userpicture? userPicture = default!;
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                userPicture = (await dbcontext.Contacts.Include(item => item.Userpicture)
                    .FirstAsync(item => item.Contactid == profileId)).Userpicture;

                if (userPicture == null) throw new Exception("Изображение профиля не найдено");
            }
            return base.View("ProfileIcon", userPicture);
        }
    }
}
