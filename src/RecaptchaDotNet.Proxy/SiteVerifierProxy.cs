using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using RecaptchaDotNet.Abstractions;

namespace RecaptchaDotNet.Proxy
{
    public class SiteVerifierProxy : ISiteVerifier
    {
        private readonly GetHttpClient _getHttpClient;
        private readonly GetRecaptchaSettings _getRecaptchaSettings;
        private readonly DeserializeFromJson _deserialize;

        public SiteVerifierProxy(GetHttpClient getHttpClient, GetRecaptchaSettings getRecaptchaSettings,
            DeserializeFromJson deserialize)
        {
            _getHttpClient = getHttpClient;
            _getRecaptchaSettings = getRecaptchaSettings;
            _deserialize = deserialize;
        }

        public async Task<VerificationResponse> VerifyAsync(string response)
        {
            var client = _getHttpClient();
            var settings = _getRecaptchaSettings();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("secret", settings.SecretKey),
                new KeyValuePair<string, string>("response", response),
            });
            var r = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
            var json = await r.Content.ReadAsStringAsync();
            var x = (VerificationResponse) _deserialize(json, typeof(VerificationResponse));
            return x;
        }
    }
}