using DatabaseAccess.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace ClientApplication.Infrastructure
{
    using Hobbies = List<DatabaseAccess.Models.Hobby>;
    using DAModels = DatabaseAccess.Models;
    public partial class HobbyModelBinder : BaseModelBinder<Hobbies, HobbyModelBinder>, IModelBinder
    {
        public HobbyModelBinder(ILogger<HobbyModelBinder> logger) : base(logger) { }
        public sealed class HobbyModelBinderProvider : System.Object, IModelBinderProvider
        {
            public HobbyModelBinderProvider() : base() { }
            public IModelBinder? GetBinder(ModelBinderProviderContext context)
            {
                var loggerFactory = context.Services.GetService<ILoggerFactory>();
                var modelBinder = new QualityModelBinder(loggerFactory!.CreateLogger<QualityModelBinder>());

                return context.Metadata.BinderType == typeof(Hobbies) ? modelBinder : null;
            }
        }
        public override Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var hobbyLength = bindingContext.ValueProvider.GetValue("hobby_length");
            if (hobbyLength == ValueProviderResult.None || !int.TryParse(hobbyLength.FirstValue, out var length))
            {
                return Task.FromResult(bindingContext.Result = ModelBindingResult.Failed());
            }
            for (var index = default(int); index < length; index++)
            {
                var hobbyValues = bindingContext.ValueProvider.GetValue($"hobby[{index}]");
                if (hobbyValues == ValueProviderResult.None)
                {
                    return Task.FromResult(bindingContext.Result = ModelBindingResult.Success(this.BindingResult));
                }
                this.BindingResult.Add(new Hobby() { Hobbyname = hobbyValues.FirstValue! });
            }
            return Task.FromResult(bindingContext.Result = ModelBindingResult.Success(this.BindingResult));
        }
    }
}

