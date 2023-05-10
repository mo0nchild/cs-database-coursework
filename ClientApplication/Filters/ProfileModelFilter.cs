using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.RegularExpressions;

namespace ClientApplication.Filters
{
    public partial class ProfileModelFilter : System.Attribute, IActionFilter
    {
        public ProfileModelFilter() : base() { }
        
        public virtual void OnActionExecuting(ActionExecutingContext context)
        {
            var emailValue = context.HttpContext.Request.Form["email"].FirstOrDefault();
            var phoneValue = context.HttpContext.Request.Form["phone"].FirstOrDefault();

            var surnameValue = context.HttpContext.Request.Form["surname"].FirstOrDefault();
            var nameValue = context.HttpContext.Request.Form["name"].FirstOrDefault();

            //foreach (var propertyValue in new[] { surnameValue, nameValue })
            //{
            //    if (propertyValue != null && Regex.IsMatch(propertyValue, @"[А-Яа-я\w]{5,}")) continue;
            //    //return new ViewModels.AuthorizationModel(true)
            //    //{
            //    //    ErrorCause = @$"Данные неверно введены{(propertyValue == null ? "" : $": {propertyValue}")}",
            //    //    Mode = ViewModels.AuthorizationModel.AuthorizationMode.Registration
            //    //};
            //    context.Result = new RedirectToActionResult();
            //}
            //var emailMatch = Regex.IsMatch(emailValue, @"^\w{6,}@(mail|gmail|yandex){1}.(ru|com){1}$");
            //var phoneMatch = Regex.IsMatch(phoneValue, @"^\+7[0-9]{10}$");

            //user.Phonenumber = (user.Phonenumber == string.Empty) ? null! : user.Phonenumber;
            //if (!emailMatch || (!phoneMatch && user.Phonenumber != null))
            //{
            //    return new ViewModels.AuthorizationModel(true)
            //    {
            //        ErrorCause = (!emailMatch ? "Неверный адрес почты" : (!phoneMatch ? "Неверный телефон" : null)),
            //        Mode = ViewModels.AuthorizationModel.AuthorizationMode.Registration,
            //    };
            //}
        }
        public virtual void OnActionExecuted(ActionExecutedContext context) { }
    }
}
