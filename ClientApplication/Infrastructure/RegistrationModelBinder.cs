using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace ClientApplication.Infrastructure
{
    using DAModels = DatabaseAccess.Models;
    public partial class RegistrationModelBinder : System.Object, IModelBinder
    {
        protected IModelBinder FallbackBinder { get; private set; } = default!;
        protected ILogger<RegistrationModelBinder> Logger { get; private set; } = default!;
        public RegistrationModelBinder(IModelBinder fallbackBinder, ILogger<RegistrationModelBinder> logger) : base()
        { (this.FallbackBinder, this.Logger) = (fallbackBinder, logger); }

        public sealed class RegistrationModelBinderProvider : System.Object, IModelBinderProvider
        {
            public IModelBinder? GetBinder(ModelBinderProviderContext context)
            {
                var loggerFactory = context.Services.GetService<ILoggerFactory>();
                var fallbackBinder = new SimpleTypeModelBinder(typeof(DAModels::Contact), loggerFactory!);

                var modelBinder = new RegistrationModelBinder(fallbackBinder, loggerFactory!
                    .CreateLogger<RegistrationModelBinder>());
                return context.Metadata.ModelType == typeof(DAModels::Contact) ? modelBinder : null;
            }
            public RegistrationModelBinderProvider() : base() { }
        }
        public virtual Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ValueProviderResult GetProviderValue(string name) => bindingContext.ValueProvider.GetValue(name);
            var modelValues = new Dictionary<string, ValueProviderResult>()
            {
                ["login"] = GetProviderValue("login"), ["password"] = GetProviderValue("password"),
                ["phone"] = GetProviderValue("phone"), ["email"] = GetProviderValue("email"),
                ["gender"] = GetProviderValue("gender"),["birthday"] = GetProviderValue("birthday"),

                ["surname"] = GetProviderValue("surname"), ["name"] = GetProviderValue("name"),
                ["patronymic"] = GetProviderValue("patronymic"),
            };
            foreach(KeyValuePair<string, ValueProviderResult> item in modelValues)
            {
                if (item.Value == ValueProviderResult.None) bindingContext.Result = ModelBindingResult.Failed();
            }
            if (!DateTime.TryParse(modelValues["birthday"].FirstValue, out var birthdayValue))
            {
                return this.FallbackBinder.BindModelAsync(bindingContext);
            }
            bindingContext.Result = ModelBindingResult.Success(new DAModels::Contact()
            {
                Authorization = new DAModels::Authorization() {
                    Login = modelValues["login"].FirstValue!, Password = modelValues["password"].FirstValue!,
                },
                Surname = modelValues["surname"].FirstValue!, Name = modelValues["name"].FirstValue!,
                Patronymic = modelValues["patronymic"].FirstValue!, Birthday = birthdayValue,

                Phonenumber = modelValues["phone"].FirstValue!, Emailaddress = modelValues["email"].FirstValue!,
                Gendertype = new DAModels::Gendertype() { Gendertypename = modelValues["gender"].FirstValue! },
            });
            return Task.CompletedTask;
        }
    }
}
