namespace ClientApplication.ViewModels
{
    using DAModels = DatabaseAccess.Models;
    public partial class AdministratorModel : System.Object
    {
        public List<DAModels::Contact> ContactsList { get; set; } = new();
        public DAModels::Contact Contact { get; set; } = new();
        public System.String SortingType { get; set; } = "Без сортировки";
        public System.String ProfileType { get; set; } = "Все контакты";

        public System.String ErrorMessage { get; set; } = default!;
        public System.Int32 SelectedRecord { get; set; } = default!;
        public System.String FormRequestLink { get; set; } = default!;
        public AdministratorModel() : base() { }
    }
}
