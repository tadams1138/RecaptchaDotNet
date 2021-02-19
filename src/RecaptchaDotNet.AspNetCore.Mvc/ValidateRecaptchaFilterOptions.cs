namespace RecaptchaDotNet.AspNetCore.Mvc
{
    public class ValidateRecaptchaFilterOptions
    {
        public string HostName { get; set; }
        public string ErrorMessage { get; set; } = "reCAPTCHA verification failed.";
    }
}