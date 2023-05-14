namespace ClientApplication.ViewModels
{
    using DAModels = DatabaseAccess.Models;
    public partial class ProfileEditingModel : System.Object
    {
        public List<DAModels::Gendertype> GenderTypes { get; set; } = default!;
        public List<DAModels::Humanquality> QualityTypes { get; set; } = default!;
        public List<DAModels::Hobby> HobbyTypes { get; set; } = default!;

        public List<DAModels::Userpicture> Pictures { get; set; } = default!;
        public List<DAModels::City> Cities { get; set; } = default!;
        public List<DAModels::Post> Postes { get; set; } = default!;
        public List<DAModels::Datingtype> Datingtypes { get; set; } = default!;

        public ProfileEditingModel() : base() { }
    }
}
