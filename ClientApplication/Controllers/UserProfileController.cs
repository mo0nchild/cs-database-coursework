using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientApplication.Controllers
{
    [RouteAttribute("user")]
    public partial class UserProfileController : Controller
    {
        private readonly ILogger<UserProfileController> _logger;
        public UserProfileController(ILogger<UserProfileController> logger) : base() 
        {
            this._logger = logger;
        }

        [RouteAttribute("profile", Name = "profile")]
        [HttpGetAttribute]
        [AuthorizeAttribute(Policy = "Administrator")]
        [AuthorizeAttribute(Policy = "DefaultUser")]
        public IActionResult DetailsInfo()
        {
            this._logger.LogInformation("asdasd");
            foreach (var item in this.HttpContext.User.Claims)
            {
                this._logger.LogInformation(item.Type);
            }
            return View();
        }
    }
}
