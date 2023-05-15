using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.RegularExpressions;

namespace ClientApplication.Filters
{
    // TODO: сделать фильтр пригодным для работы в контроллере Authorization
    public partial class ProfileModelFilter : System.Attribute, IActionFilter
    {
        protected readonly string routeToRedirect = default!;
        public ProfileModelFilter(string routeToRedirect) : base() { this.routeToRedirect = routeToRedirect; }
        public virtual void OnActionExecuting(ActionExecutingContext context)
        {
            var idValue = context.HttpContext.Request.Form["id"].FirstOrDefault();
            if (context.HttpContext.Request.Form["validate"].FirstOrDefault() == null) return;

            var phoneValue = context.HttpContext.Request.Form["phone"].FirstOrDefault();
            var emailValue = context.HttpContext.Request.Form["email"].FirstOrDefault();

            var surnameValue = context.HttpContext.Request.Form["surname"].FirstOrDefault();
            var nameValue = context.HttpContext.Request.Form["name"].FirstOrDefault();
            var streetValue = context.HttpContext.Request.Form["street"].FirstOrDefault();

            var errorModel = new ViewModels.UserBaseModel()
            {
                HasError = true, Mode = ViewModels.UserProfileModel.PageMode.Settings, ErrorMessage = string.Empty,
                SelectedContact = int.Parse(idValue ?? "0")
            };
            foreach (var propertyValue in new[] { surnameValue, nameValue })
            {
                if (propertyValue != null && Regex.IsMatch(propertyValue, @"[А-Яа-я\w]{5,}")) continue;

                errorModel.ErrorMessage = $"Введено неверное значение ФИО: " +
                        $"{propertyValue?.Substring(0, propertyValue.Length > 20 ? 20 : propertyValue.Length)}";

                context.Result = new RedirectToRouteResult(this.routeToRedirect, errorModel); return;
            }
            if (streetValue != null && !Regex.IsMatch(streetValue, @"[А-Яа-я\w]{5,}"))
            {
                errorModel.ErrorMessage = $"Введено неверное значение улицы: {streetValue}";
                context.Result = new RedirectToRouteResult(this.routeToRedirect, errorModel); return;
            }
            var emailMatch = emailValue != null && Regex.IsMatch(emailValue, @"^\w{6,}@(mail|gmail|yandex){1}.(ru|com){1}$");
            var phoneMatch = phoneValue != null && Regex.IsMatch(phoneValue, @"^\+7[0-9]{10}$");

            if (!emailMatch || (!phoneMatch && phoneValue != null))
            {
                errorModel.ErrorMessage = $"Введено неверное значение контактов: {(!emailMatch ? emailValue : phoneValue)}";
                context.Result = new RedirectToRouteResult(this.routeToRedirect, errorModel); return;
            }
            
        }
        public virtual void OnActionExecuted(ActionExecutedContext context) { }
    }
}
