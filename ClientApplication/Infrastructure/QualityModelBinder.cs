using DatabaseAccess.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace ClientApplication.Infrastructure
{
    using Qualities = List<DatabaseAccess.Models.Humanquality>;
    using DAModels = DatabaseAccess.Models;
    public partial class QualityModelBinder : BaseModelBinder<Qualities, QualityModelBinder>, IModelBinder
    {
        public QualityModelBinder(ILogger<QualityModelBinder> logger) : base(logger) { }
        public sealed class QualityModelBinderProvider : System.Object, IModelBinderProvider
        {
            public QualityModelBinderProvider() : base() { }
            public IModelBinder? GetBinder(ModelBinderProviderContext context)
            {
                var loggerFactory = context.Services.GetService<ILoggerFactory>();
                var modelBinder = new QualityModelBinder(loggerFactory!.CreateLogger<QualityModelBinder>());

                return context.Metadata.BinderType == typeof(Qualities) ? modelBinder : null;
            }
        }
        public override Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var qualityLength = bindingContext.ValueProvider.GetValue("quality_length");
            if (qualityLength == ValueProviderResult.None || !int.TryParse(qualityLength.FirstValue, out var length))
            {
                return Task.FromResult(bindingContext.Result = ModelBindingResult.Failed());
            }
            this.BindingResult.Clear();
            for (var index = default(int); index < length; index++)
            {
                var employeeValues = bindingContext.ValueProvider.GetValue($"quality[{index}]");
                if (employeeValues == ValueProviderResult.None)
                {
                    return Task.FromResult(bindingContext.Result = ModelBindingResult.Success(this.BindingResult));
                }
                this.BindingResult.Add(new DAModels::Humanquality() { Qualityname = employeeValues.FirstValue! });
            }
            return Task.FromResult(bindingContext.Result = ModelBindingResult.Success(this.BindingResult));
        }
    }
}
