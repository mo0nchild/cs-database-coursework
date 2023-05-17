using DatabaseAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Immutable;

namespace ClientApplication.ViewComponents
{
    [ViewComponentAttribute]
    public partial class GroupbyListViewComponent : ViewComponent
    {
        public record class GroupbyItemModel(string GroupName, int GroupValue);
        public sealed class GroupbyListViewModel : System.Object
        {
            public List<GroupbyItemModel> GenderGroups { get; set; } = new();
            public List<GroupbyItemModel> AccountTypeGroups { get; set; } = new();
            public GroupbyListViewModel() : base() { }
        }
        protected IDbContextFactory<DatabaseContext> DatabaseFactory { get; set; } = default!;
        public GroupbyListViewComponent(IDbContextFactory<DatabaseContext> factory) : base() 
        { this.DatabaseFactory = factory; }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var viewModel = new GroupbyListViewModel();
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                var genderGroupRecord = dbcontext.Contacts.Include(item => item.Gendertype)
                    .GroupBy(item => item.Gendertype.Gendertypename).Select(item => new GroupbyItemModel(item.Key, item.Count()))
                    .ToImmutableList();

                var accountGroupRecord = dbcontext.Contacts.Include(item => item.Authorization)
                    .GroupBy(item => item.Authorization == null ? "Созданные контакты" : "Акканты пользователей")
                    .Select(item => new GroupbyItemModel(item.Key!.ToString()!, item.Count())).ToImmutableList();

                viewModel.AccountTypeGroups.AddRange(accountGroupRecord);
                viewModel.GenderGroups.AddRange(genderGroupRecord);
            }
            return this.View("GroupbyList", viewModel);
        }
    }
}
