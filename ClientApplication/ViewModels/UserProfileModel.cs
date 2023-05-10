namespace ClientApplication.ViewModels
{
    using DAModels = DatabaseAccess.Models;
    public sealed partial class UserProfileModel : System.Object
    {
        public enum PageMode : byte { Settings, Contacts }
        
        public DAModels::Contact Contact { get; set; } = default!;
        public UserProfileModel.PageMode Mode { get; set; } = PageMode.Settings;
        
        public UserProfileModel() : base() { }
    }
}
