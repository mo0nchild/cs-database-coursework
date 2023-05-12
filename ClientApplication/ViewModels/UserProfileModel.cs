namespace ClientApplication.ViewModels
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using DAModels = DatabaseAccess.Models;
    public sealed partial class UserProfileModel : System.Object
    {
        public enum PageMode : byte { Settings, Contacts }

        [BindingBehaviorAttribute(BindingBehavior.Never)]
        public DAModels::Contact Contact { get; set; } = new();

        public System.String SortingType { get; set; } = "Без сортировки";
        public System.String ProfileType { get; set; } = "Все контакты";

        public UserProfileModel.PageMode Mode { get; set; } = PageMode.Settings;
        public System.Boolean HasError { get; set; } = default!;
        public System.String? ErrorMessage { get; set; } = default;

        public UserProfileModel() : base() { }
    }
}
