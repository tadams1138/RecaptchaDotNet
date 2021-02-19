using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using RecaptchaDotNet.Abstractions;

namespace RecaptchaDotNet.AspNetCore.Mvc
{
    public class ValidateRecaptchaFilter : IAsyncActionFilter
    {
        private const string FormKey = "g-recaptcha-response";
        private readonly ISiteVerifier _siteVerifier;
        private readonly ValidateRecaptchaFilterOptions _options;

        public ValidateRecaptchaFilter(ISiteVerifier siteVerifier, ValidateRecaptchaFilterOptions options)
        {
            _siteVerifier = siteVerifier ?? throw new ArgumentNullException(nameof(siteVerifier));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var gRecaptchaResponse = context.HttpContext.Request.Form[FormKey];
            var verificationResponse = await _siteVerifier.VerifyAsync(gRecaptchaResponse);
            if (!verificationResponse.Success || verificationResponse.HostName != _options.HostName)
            {
                context.ModelState.AddModelError(FormKey, _options.ErrorMessage);
            }

            await next();
        }
    }
}