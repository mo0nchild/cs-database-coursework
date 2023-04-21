namespace ClientApplication.ViewModels
{
    public sealed class AuthorizationModel : System.Object
    {
        public enum AuthorizationMode : System.SByte { Login, Registration };

        public System.Boolean HasError { get; private set; } = default;
        public System.String? ErrorCause { get; init; } = default!;
        public AuthorizationMode Mode { get; init; } = AuthorizationMode.Login;

        public AuthorizationModel(bool has_error) : base() { this.HasError = has_error; }
        public AuthorizationModel() : this(default(bool)) { }
    }
}
