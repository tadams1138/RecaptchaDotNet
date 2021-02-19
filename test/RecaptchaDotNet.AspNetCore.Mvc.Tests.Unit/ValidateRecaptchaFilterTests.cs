using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using RecaptchaDotNet.Abstractions;
using Xunit;

namespace RecaptchaDotNet.AspNetCore.Mvc.Tests.Unit
{
    public class ValidateRecaptchaFilterTests
    {
        private const string ErrorMessage = "test Error Message";
        private const string HostName = "test HostName";
        private const string ReCaptchaResponse = "test Response";
        private const string FormKey = "g-recaptcha-response";
        private readonly Mock<ISiteVerifier> _stubVerifySite;
        private readonly ValidateRecaptchaFilter _filter;
        private readonly ActionExecutingContext _context;

        public ValidateRecaptchaFilterTests()
        {
            _stubVerifySite = new Mock<ISiteVerifier>();
            var options = new ValidateRecaptchaFilterOptions
            {
                ErrorMessage = ErrorMessage,
                HostName = HostName
            };
            _filter = new ValidateRecaptchaFilter(_stubVerifySite.Object, options);
            _context = CreateContext();
        }

        [Fact]
        public async Task GivenGoodResponse_VerifyAsync_ModelStateIsValid()
        {
            // Arrange
            ConfigureSiteVerifier(new VerificationResponse
            {
                Success = true,
                HostName = HostName
            });

            // Act
            await OnActionExecutionAsync();

            // Assert
            _context.ModelState.IsValid.Should().BeTrue();
        }
        
        [Theory]
        [InlineData(false, HostName)]
        [InlineData(true, "something else")]
        public async Task GivenBadResponse_VerifyAsync_ModelStateIsInvalid(bool success, string hostName)
        {
            // Arrange
            ConfigureSiteVerifier(new VerificationResponse
            {
                Success = success,
                HostName = hostName
            });

            // Act
            await OnActionExecutionAsync();

            // Assert
            AssertModelIsNotValid();
        }

        private static ActionExecutingContext CreateContext()
        {
            var form = new Dictionary<string, StringValues>
            {
                {FormKey, ReCaptchaResponse}
            };
            var httpContext = new HttpContextImpl();
            httpContext.Request.Form = new FormCollection(form);
            var routeData = new RouteData();
            var actionDescriptor = new ActionDescriptor();
            var modelStateDictionary = new ModelStateDictionary();
            var actionContext = new ActionContext(httpContext, routeData, actionDescriptor, modelStateDictionary);
            var filters = new List<IFilterMetadata>();
            var actionArguments = new Dictionary<string, object>();
            var context = new ActionExecutingContext(actionContext, filters, actionArguments, null);
            return context;
        }

        private void AssertModelIsNotValid()
        {
            _context.ModelState.IsValid.Should().BeFalse();
            _context.ModelState.Keys.Should().Contain(FormKey);
            _context.ModelState[FormKey].Errors.Should().Contain(x => x.ErrorMessage == ErrorMessage);
        }

        private void ConfigureSiteVerifier(VerificationResponse verificationResponse)
        {
            _stubVerifySite.Setup(x => x.VerifyAsync(ReCaptchaResponse)).ReturnsAsync(verificationResponse);
        }

        private async Task OnActionExecutionAsync()
        {
            var nextCalled = false;
            await _filter.OnActionExecutionAsync(_context, () =>
            {
                nextCalled = true;
                return Task.FromResult((ActionExecutedContext) null);
            });
            nextCalled.Should().BeTrue();
        }

        private class FormCollection : IFormCollection
        {
            private readonly IDictionary<string, StringValues> _dictionary;

            public FormCollection(IDictionary<string, StringValues> dictionary)
            {
                _dictionary = dictionary;
            }
            public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
            {
                return _dictionary.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public bool ContainsKey(string key)
            {
                return _dictionary.ContainsKey(key);
            }

            public bool TryGetValue(string key, out StringValues value)
            {
                return _dictionary.TryGetValue(key, out value);
            }

            public int Count => _dictionary.Count;
            public ICollection<string> Keys => _dictionary.Keys;

            public StringValues this[string key] => _dictionary[key];

            public IFormFileCollection Files => throw new NotImplementedException();
        }

        private class HttpRequestImpl: HttpRequest
        {
            public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = new CancellationToken())
            {
                throw new NotImplementedException();
            }

            public override HttpContext HttpContext => throw new NotImplementedException();
            public override string Method { get; set; }
            public override string Scheme { get; set; }
            public override bool IsHttps { get; set; }
            public override HostString Host { get; set; }
            public override PathString PathBase { get; set; }
            public override PathString Path { get; set; }
            public override QueryString QueryString { get; set; }
            public override IQueryCollection Query { get; set; }
            public override string Protocol { get; set; }
            public override IHeaderDictionary Headers => throw new NotImplementedException();
            public override IRequestCookieCollection Cookies { get; set; }
            public override long? ContentLength { get; set; }
            public override string ContentType { get; set; }
            public override Stream Body { get; set; }
            public override bool HasFormContentType => throw new NotImplementedException();
            public override IFormCollection Form { get; set; }
        }

        private class HttpContextImpl : HttpContext
        {
            public override void Abort()
            {
                throw new NotImplementedException();
            }

            public override IFeatureCollection Features => throw new NotImplementedException();
            public override HttpRequest Request { get; } = new HttpRequestImpl();
            public override HttpResponse Response => throw new NotImplementedException();
            public override ConnectionInfo Connection => throw new NotImplementedException();
            public override WebSocketManager WebSockets => throw new NotImplementedException();
            
            [Obsolete]
            public override AuthenticationManager Authentication => throw new NotImplementedException();
            public override ClaimsPrincipal User { get; set; }
            public override IDictionary<object, object> Items { get; set; }
            public override IServiceProvider RequestServices { get; set; }
            public override CancellationToken RequestAborted { get; set; }
            public override string TraceIdentifier { get; set; }
            public override ISession Session { get; set; }
        }
    }
}