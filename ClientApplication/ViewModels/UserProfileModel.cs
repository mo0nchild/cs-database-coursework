namespace ClientApplication.ViewModels
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using System.ComponentModel.DataAnnotations;
    using DAModels = DatabaseAccess.Models;
    public sealed partial class UserProfileModel : System.Object
    {
        public enum PageMode : byte { Settings, Contacts }

        [BindingBehaviorAttribute(BindingBehavior.Never)]
        public DAModels::Contact Contact { get; set; } = new();

        public static readonly System.Int32 MaxRecordOnPage = 15, MinRecordOnPage = 4;
        public System.Int32 RecordOnPage { get; set; } = UserProfileModel.MinRecordOnPage;

        public System.String SortingType { get; set; } = "Без сортировки";
        public System.String ProfileType { get; set; } = "Все контакты";
        public System.String SearchQuery { get; set; } = string.Empty;

        public UserProfileModel.PageMode Mode { get; set; } = PageMode.Contacts;
        public System.Boolean HasError { get; set; } = default!;
        public System.String? ErrorMessage { get; set; } = default;
        public System.Int32 CurrentPage { get; set; } = default;
        public System.Int32 PagesCount { get; set; } = default;

        public UserProfileModel() : base() { }
    }
}
