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
    using ControllerLogger = ILogger<UserContactsController>;

    [ControllerAttribute, RouteAttribute("user"), AuthorizeAttribute(Policy = "DefaultUser")]
    public partial class UserContactsController : Controller
    {
        protected IDbContextFactory<DatabaseContext> DatabaseFactory { get; private set; } = default!;
        private readonly ILogger<UserContactsController> _logger;
        public UserContactsController(IDbContextFactory<DatabaseContext> factory, ControllerLogger logger)
            : base() { (this.DatabaseFactory, this._logger) = (factory, logger); }

        public async Task<IActionResult> EditContact()
        {
            await Console.Out.WriteLineAsync();
            return base.View();
        }

    }
}
