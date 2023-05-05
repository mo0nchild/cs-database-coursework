using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ClientApplication.Infrastructure
{
    using DAModels = DatabaseAccess.Models;
    public abstract class BaseModelBinder<TModel, TBinder> : object, IModelBinder where TModel : new() 
    {
        protected ILogger<TBinder> Logger { get; private set; } = default!;
        public TModel BindingResult { get; protected set; } = new();
        public BaseModelBinder(ILogger<TBinder> logger) : base() { this.Logger = logger; }
        public abstract Task BindModelAsync(ModelBindingContext bindingContext);
    }
    
    public partial class ContractModelBinder : BaseModelBinder<DAModels::Contact, ContractModelBinder>, IModelBinder
    {
        protected IModelBinder FallbackBinder { get; private set; } = default!;

        private readonly BaseModelBinder<List<DAModels::Hobby>, HobbyModelBinder> hobbyBinder = default!;
        private readonly BaseModelBinder<List<DAModels::Humanquality>, QualityModelBinder> qualityBinder = default!;  

        private readonly BaseModelBinder<List<DAModels::Employee>, EmployeeModelBinder> employeeBinder = default!;
        public ContractModelBinder(IModelBinder fallbackBinder, ILogger<ContractModelBinder> logger) 
            : base(logger) { this.FallbackBinder = fallbackBinder; }

        public ContractModelBinder(IModelBinder fallbackBinder, ILoggerFactory factory) 
            : base(factory.CreateLogger<ContractModelBinder>())
        {
            this.qualityBinder = new QualityModelBinder(factory.CreateLogger<QualityModelBinder>());
            this.hobbyBinder = new HobbyModelBinder(factory.CreateLogger<HobbyModelBinder>());

            this.employeeBinder = new EmployeeModelBinder(factory.CreateLogger<EmployeeModelBinder>());
            this.FallbackBinder = fallbackBinder;
        }
        public sealed class ContractModelBinderProvider : System.Object, IModelBinderProvider
        {
            public IModelBinder? GetBinder(ModelBinderProviderContext context)
            {
                var loggerFactory = context.Services.GetService<ILoggerFactory>();
                var fallbackBinder = new SimpleTypeModelBinder(typeof(DAModels::Contact), loggerFactory!);

                var modelBinder = new ContractModelBinder(fallbackBinder, loggerFactory!);
                return context.Metadata.ModelType == typeof(DAModels::Contact) ? modelBinder : null;
            }
            public ContractModelBinderProvider() : base() { }
        }
        public override Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ValueProviderResult GetProvider(string name) => bindingContext.ValueProvider.GetValue(name);
            string GetValue(string name) => GetProvider(name).FirstValue!;
            foreach (var item in new[] { "phone", "email", "gender", "birthday", "surname", "name", "patronymic", "family" })
            {
                if (GetProvider(item) == ValueProviderResult.None) return this.FallbackBinder.BindModelAsync(bindingContext);
            }
            if (!DateTime.TryParse(GetProvider("birthday").FirstValue, out var birthdayValue))
            {
                return Task.FromResult(bindingContext.Result = ModelBindingResult.Failed());
            }
            this.employeeBinder.BindModelAsync(bindingContext).Wait();
            this.BindingResult = new DAModels::Contact()
            {
                Surname = GetValue("surname"), Name = GetValue("name"), Patronymic = GetValue("patronymic"),
                Birthday = birthdayValue, Phonenumber = GetValue("phone"), Emailaddress = GetValue("email"),

                Employees = this.employeeBinder.BindingResult, Familystatus = GetValue("family"),
                Gendertype = new DAModels::Gendertype() { Gendertypename = GetValue("gender") },
            };
            this.Logger.LogInformation("Contact binder create model");

            this.BindingResult.Userpicture = (GetProvider("picture") == ValueProviderResult.None) ? null
                : new DAModels::Userpicture() { Picturename = GetValue("picture") };
            var locationModelState = true;
            foreach(var locationItem in new string[] { "country", "city", "street" })
            {
                if (GetProvider(locationItem) == ValueProviderResult.None) { locationModelState = false; break; }
            }
            this.BindingResult.Location = (locationModelState == default) ? null : new DAModels::Location()
            {
                City = new DAModels::City() { Cityname = GetValue("city"), Country = GetValue("country") },
                Street = GetValue("street")
            };
            this.hobbyBinder.BindModelAsync(bindingContext).Wait();
            this.BindingResult.Hobbies = this.hobbyBinder.BindingResult;

            this.qualityBinder.BindModelAsync(bindingContext).Wait();
            this.BindingResult.Humanqualities = this.qualityBinder.BindingResult;

            return Task.FromResult(bindingContext.Result = ModelBindingResult.Success(this.BindingResult));
        }
    }
}
