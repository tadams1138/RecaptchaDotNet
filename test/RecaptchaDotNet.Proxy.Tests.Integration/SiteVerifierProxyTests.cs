using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RecaptchaDotNet.Abstractions;
using Xunit;

namespace RecaptchaDotNet.Proxy.Tests.Integration
{
    public class SiteVerifierProxyTests
    {
        private readonly ISiteVerifier _proxy;

        public SiteVerifierProxyTests()
        {
            var configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("secrets.json")
                .Build();
            var recaptchaSettings = new RecaptchaSettings
            {
                SecretKey = configurationRoot["reCAPTCHATestSecretKey"]
            };
            _proxy = new SiteVerifierProxy(() => new HttpClient(), () => recaptchaSettings,
                JsonConvert.DeserializeObject);
        }

        [Fact]
        public async Task GetAsync_ReturnsResponse()
        {
            // Arrange

            // Act
            var response = await _proxy.VerifyAsync("test response... this really doesn't matter");

            // Assert
            response.Success.Should().BeTrue();
            response.HostName.Should().Be("testkey.google.com");
        }
    }
}