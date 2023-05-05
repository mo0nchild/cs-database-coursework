using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ClientApplication.Infrastructure
{
    using Employees = List<DatabaseAccess.Models.Employee>;
    using DAModels = DatabaseAccess.Models;
    public partial class EmployeeModelBinder : BaseModelBinder<Employees, EmployeeModelBinder>, IModelBinder
    {
        public EmployeeModelBinder(ILogger<EmployeeModelBinder> logger) : base(logger) { }
        public sealed class EmployeeModelBinderProvider : System.Object, IModelBinderProvider
        {
            public EmployeeModelBinderProvider() : base() { }
            public IModelBinder? GetBinder(ModelBinderProviderContext context)
            {
                var loggerFactory = context.Services.GetService<ILoggerFactory>();
                var modelBinder = new EmployeeModelBinder(loggerFactory!.CreateLogger<EmployeeModelBinder>());

                return context.Metadata.BinderType == typeof(Employees) ? modelBinder : null;
            }
        }
        public override Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var employeeLength = bindingContext.ValueProvider.GetValue("employee_length");
            if (employeeLength == ValueProviderResult.None || !int.TryParse(employeeLength.FirstValue, out var length))
            {
                return Task.FromResult(bindingContext.Result = ModelBindingResult.Failed());
            }
            for(var index = default(int); index < length; index++)
            {
                var employeeValues = new Dictionary<string, ValueProviderResult>()
                {
                    ["company"] = bindingContext.ValueProvider.GetValue($"employee[{index}].company"),
                    ["post"] = bindingContext.ValueProvider.GetValue($"employee[{index}].post"),

                    ["status"] = bindingContext.ValueProvider.GetValue($"employee[{index}].status"),
                };
                foreach(KeyValuePair<string, ValueProviderResult> providerResult in employeeValues)
                {
                    if(providerResult.Value == ValueProviderResult.None)
                    { return Task.FromResult(bindingContext.Result = ModelBindingResult.Failed()); }
                }
                this.BindingResult.Add(new DAModels::Employee()
                {
                    Companyname = employeeValues["company"].FirstValue!, Status = employeeValues["status"].FirstValue!,
                    Post = new DAModels.Post() { Postname = employeeValues["post"].FirstValue! }
                });
            }
            bindingContext.Result = ModelBindingResult.Success(this.BindingResult);
            return Task.CompletedTask;
        }
    }
}
