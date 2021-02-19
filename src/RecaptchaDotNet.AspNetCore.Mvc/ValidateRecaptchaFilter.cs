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
            throw new InvalidOperationException();
        }
    }
}