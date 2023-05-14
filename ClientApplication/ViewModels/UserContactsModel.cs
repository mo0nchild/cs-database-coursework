using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ClientApplication.ViewModels
{
    using static ClientApplication.ViewModels.UserProfileModel;
    using DAModels = DatabaseAccess.Models;
    public partial class UserBaseModel : System.Object
    {
        public enum PageMode : byte { Settings, Contacts }

        public System.Boolean HasError { get; set; } = default!;
        public System.String? ErrorMessage { get; set; } = default;
        public UserProfileModel.PageMode Mode { get; set; } = PageMode.Contacts;

        public UserBaseModel() : base() { }
    }

    public partial class UserContactsModel : ViewModels.UserBaseModel
    {
        [BindingBehaviorAttribute(BindingBehavior.Never)]
        public DAModels::Contact Contact { get; set; } = new();

        public UserContactsModel() : base() { }
    }
}
