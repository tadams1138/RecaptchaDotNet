using System;
using Microsoft.AspNetCore.Mvc.Filters;
using RecaptchaDotNet.Abstractions;

namespace RecaptchaDotNet.AspNetCore.Mvc
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class ValidateRecaptchaAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => true;

        public IFilterMetadata CreateInstance(IServiceProvider services)
        {
            var verifySite = services.GetService(typeof(ISiteVerifier)) as ISiteVerifier;
            var options = services.GetService(typeof(ValidateRecaptchaFilterOptions)) as ValidateRecaptchaFilterOptions;
            return new ValidateRecaptchaFilter(verifySite, options);
        }
    }
}