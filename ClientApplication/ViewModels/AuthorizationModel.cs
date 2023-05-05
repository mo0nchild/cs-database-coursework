using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ClientApplication.ViewModels
{
    public sealed class AuthorizationModel : System.Object
    {
        public enum AuthorizationMode : System.SByte { Login, Registration };

        public System.Boolean HasError { get; set; } = default;
        public System.String? ErrorCause { get; set; } = default!;
        public AuthorizationMode Mode { get; set; } = AuthorizationMode.Login;

        public AuthorizationModel(bool has_error) : base() { this.HasError = has_error; }
        public AuthorizationModel() : this(default(bool)) { }
    }
}
