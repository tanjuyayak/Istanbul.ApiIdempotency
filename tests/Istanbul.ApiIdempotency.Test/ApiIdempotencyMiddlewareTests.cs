using Istanbul.ApiIdempotency.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Istanbul.ApiIdempotency.Test
{
    public class ApiIdempotencyMiddlewareTests
    {
        [Fact]
        public async Task RequestDelegate_Should_Be_Called_Only_When_ApiIdempotencyAttribute_Not_Found()
        {
            var requestDelegateMock = new Mock<RequestDelegate>();
            var apiIdempotencyDataStoreProviderMock = new Mock<IApiIdempotencyDataStoreProvider>();
            var apiIdempotencyInternalOptionsMock = new Mock<IOptions<ApiIdempotencyInternalOptions>>();

            var apiIdempotencyMiddleware = new ApiIdempotencyMiddleware(requestDelegateMock.Object);

            var context = new DefaultHttpContext();

            var initialEndpoint = new Endpoint(c => Task.CompletedTask, EndpointMetadataCollection.Empty, "Test endpoint");
            var initialFeature = new EndpointFeature
            {
                Endpoint = initialEndpoint
            };
            context.Features.Set<IEndpointFeature>(initialFeature);

            await apiIdempotencyMiddleware.InvokeAsync(context, apiIdempotencyDataStoreProviderMock.Object, apiIdempotencyInternalOptionsMock.Object);
            requestDelegateMock.Verify(next => next(It.IsAny<HttpContext>()), Times.Once);
        }

        [Fact]
        public async Task Invoke_Should_Throw_Exception_When_TimeToLive_Equal_Zero()
        {
            var requestDelegateMock = new Mock<RequestDelegate>();
            var apiIdempotencyDataStoreProviderMock = new Mock<IApiIdempotencyDataStoreProvider>();
            var apiIdempotencyInternalOptionsMock = new Mock<IOptions<ApiIdempotencyInternalOptions>>();

            var apiIdempotencyMiddleware = new ApiIdempotencyMiddleware(requestDelegateMock.Object);

            var context = new DefaultHttpContext();

            var endpointMetadataCollection = new EndpointMetadataCollection(new object[] { new ApiIdempotencyAttribute(0) });

            var initialEndpoint = new Endpoint(c => Task.CompletedTask, endpointMetadataCollection, "Test endpoint");
            var initialFeature = new EndpointFeature
            {
                Endpoint = initialEndpoint
            };
            context.Features.Set<IEndpointFeature>(initialFeature);

            var exception = await Record.ExceptionAsync(() => apiIdempotencyMiddleware.InvokeAsync(context, apiIdempotencyDataStoreProviderMock.Object, apiIdempotencyInternalOptionsMock.Object));

            Assert.NotNull(exception);
            Assert.Equal(typeof(ArgumentException), exception.GetType());
        }

        [Fact]
        public async Task Invoke_Should_Throw_Exception_When_TimeToLive_Lower_Than_Zero()
        {
            var requestDelegateMock = new Mock<RequestDelegate>();
            var apiIdempotencyDataStoreProviderMock = new Mock<IApiIdempotencyDataStoreProvider>();
            var apiIdempotencyInternalOptionsMock = new Mock<IOptions<ApiIdempotencyInternalOptions>>();

            var apiIdempotencyMiddleware = new ApiIdempotencyMiddleware(requestDelegateMock.Object);

            var context = new DefaultHttpContext();

            var endpointMetadataCollection = new EndpointMetadataCollection(new object[] { new ApiIdempotencyAttribute(-5) });

            var initialEndpoint = new Endpoint(c => Task.CompletedTask, endpointMetadataCollection, "Test endpoint");
            var initialFeature = new EndpointFeature
            {
                Endpoint = initialEndpoint
            };
            context.Features.Set<IEndpointFeature>(initialFeature);

            var exception = await Record.ExceptionAsync(() => apiIdempotencyMiddleware.InvokeAsync(context, apiIdempotencyDataStoreProviderMock.Object, apiIdempotencyInternalOptionsMock.Object));

            Assert.NotNull(exception);
            Assert.Equal(typeof(ArgumentException), exception.GetType());
        }

        [Fact]
        public async Task Invoke_Should_Throw_Exception_When_Idempotency_Header_Key_Not_Found()
        {
            var requestDelegateMock = new Mock<RequestDelegate>();
            var apiIdempotencyDataStoreProviderMock = new Mock<IApiIdempotencyDataStoreProvider>();
            var apiIdempotencyInternalOptionsMock = new Mock<IOptions<ApiIdempotencyInternalOptions>>();
            apiIdempotencyInternalOptionsMock.Setup(s => s.Value).Returns(new ApiIdempotencyInternalOptions
            {
                IdempotencyHeaderKey = "test-header-key"
            });

            var apiIdempotencyMiddleware = new ApiIdempotencyMiddleware(requestDelegateMock.Object);

            var context = new DefaultHttpContext();

            var endpointMetadataCollection = new EndpointMetadataCollection(new object[] { new ApiIdempotencyAttribute(10) });

            var initialEndpoint = new Endpoint(c => Task.CompletedTask, endpointMetadataCollection, "Test endpoint");
            var initialFeature = new EndpointFeature
            {
                Endpoint = initialEndpoint
            };
            context.Features.Set<IEndpointFeature>(initialFeature);

            var exception = await Record.ExceptionAsync(() => apiIdempotencyMiddleware.InvokeAsync(context, apiIdempotencyDataStoreProviderMock.Object, apiIdempotencyInternalOptionsMock.Object));

            Assert.NotNull(exception);
            Assert.Equal(typeof(ArgumentException), exception.GetType());
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("  ")]
        public async Task Invoke_Should_Throw_Exception_When_Idempotency_Header_Key_Found_But_Not_Null_Or_Empty_Or_OnlyWhiteSpaces(string failureHeaderKeyValue)
        {
            var headerKey = "test-header-key";

            var requestDelegateMock = new Mock<RequestDelegate>();
            var apiIdempotencyDataStoreProviderMock = new Mock<IApiIdempotencyDataStoreProvider>();
            var apiIdempotencyInternalOptionsMock = new Mock<IOptions<ApiIdempotencyInternalOptions>>();
            apiIdempotencyInternalOptionsMock.Setup(s => s.Value).Returns(new ApiIdempotencyInternalOptions
            {
                IdempotencyHeaderKey = headerKey
            });

            var apiIdempotencyMiddleware = new ApiIdempotencyMiddleware(requestDelegateMock.Object);

            var context = new DefaultHttpContext();
            context.Request.Headers.Add(headerKey, failureHeaderKeyValue);

            var endpointMetadataCollection = new EndpointMetadataCollection(new object[] { new ApiIdempotencyAttribute(10) });

            var initialEndpoint = new Endpoint(c => Task.CompletedTask, endpointMetadataCollection, "Test endpoint");
            var initialFeature = new EndpointFeature
            {
                Endpoint = initialEndpoint
            };
            context.Features.Set<IEndpointFeature>(initialFeature);

            var exception = await Record.ExceptionAsync(() => apiIdempotencyMiddleware.InvokeAsync(context, apiIdempotencyDataStoreProviderMock.Object, apiIdempotencyInternalOptionsMock.Object));

            Assert.NotNull(exception);
            Assert.Equal(typeof(ArgumentException), exception.GetType());
        }

        [Fact]
        public async Task Invoke_Should_Set_Http_Response_As_425_In_Case_Key_Exists_In_DatStore_But_Not_Response()
        {
            var headerKey = "test-header-key";
            var headerKeyValue = "abc123";

            var requestDelegateMock = new Mock<RequestDelegate>();
            var apiIdempotencyDataStoreProviderMock = new Mock<IApiIdempotencyDataStoreProvider>();
            apiIdempotencyDataStoreProviderMock.Setup(s => s.TryCheckKeyExistsAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(new ApiIdempotencyResult
            {
                KeyExists = true,
                ResponseCache = null
            });

            var apiIdempotencyInternalOptionsMock = new Mock<IOptions<ApiIdempotencyInternalOptions>>();
            apiIdempotencyInternalOptionsMock.Setup(s => s.Value).Returns(new ApiIdempotencyInternalOptions
            {
                IdempotencyHeaderKey = headerKey
            });

            var apiIdempotencyMiddleware = new ApiIdempotencyMiddleware(requestDelegateMock.Object);

            var context = new DefaultHttpContext();
            context.Request.Headers.Add(headerKey, headerKeyValue);

            var endpointMetadataCollection = new EndpointMetadataCollection(new object[] { new ApiIdempotencyAttribute(10) });

            var initialEndpoint = new Endpoint(c => Task.CompletedTask, endpointMetadataCollection, "Test endpoint");
            var initialFeature = new EndpointFeature
            {
                Endpoint = initialEndpoint
            };
            context.Features.Set<IEndpointFeature>(initialFeature);

            await apiIdempotencyMiddleware.InvokeAsync(context, apiIdempotencyDataStoreProviderMock.Object, apiIdempotencyInternalOptionsMock.Object);

            requestDelegateMock.Verify(next => next(It.IsAny<HttpContext>()), Times.Never);
            Assert.Equal(425, context.Response.StatusCode);
        }

        [Fact]
        public async Task Invoke_Should_Set_Http_Response_Body_Status_And_Headers_As_Expected_In_Case_Key_Exists_In_DatStore_With_Data()
        {
            var headerKey = "test-header-key";
            var headerKeyValue = "abc123";

            var requestDelegateMock = new Mock<RequestDelegate>();
            var apiIdempotencyDataStoreProviderMock = new Mock<IApiIdempotencyDataStoreProvider>();
            apiIdempotencyDataStoreProviderMock.Setup(s => s.TryCheckKeyExistsAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(new ApiIdempotencyResult
            {
                KeyExists = true,
                ResponseCache = new ApiResponseCache
                {
                    HttpStatusCode = 200,
                    ResponseBody = "testhttpresponsebody",
                    ResponseHeaders = new System.Collections.Generic.Dictionary<string, string>
                    {
                        ["header1"] = "header1value",
                        ["header2"] = "header2value"
                    }
                }
            });

            var apiIdempotencyInternalOptionsMock = new Mock<IOptions<ApiIdempotencyInternalOptions>>();
            apiIdempotencyInternalOptionsMock.Setup(s => s.Value).Returns(new ApiIdempotencyInternalOptions
            {
                IdempotencyHeaderKey = headerKey
            });

            var apiIdempotencyMiddleware = new ApiIdempotencyMiddleware(requestDelegateMock.Object);

            var context = new DefaultHttpContext();
            context.Request.Headers.Add(headerKey, headerKeyValue);

            context.Response.Body = new MemoryStream();

            var endpointMetadataCollection = new EndpointMetadataCollection(new object[] { new ApiIdempotencyAttribute(10) });

            var initialEndpoint = new Endpoint(c => Task.CompletedTask, endpointMetadataCollection, "Test endpoint");
            var initialFeature = new EndpointFeature
            {
                Endpoint = initialEndpoint
            };
            context.Features.Set<IEndpointFeature>(initialFeature);

            await apiIdempotencyMiddleware.InvokeAsync(context, apiIdempotencyDataStoreProviderMock.Object, apiIdempotencyInternalOptionsMock.Object);

            requestDelegateMock.Verify(next => next(It.IsAny<HttpContext>()), Times.Never);

            Assert.Equal(200, context.Response.StatusCode);
            Assert.Equal("header1value", context.Response.Headers["header1"]);
            Assert.Equal("header2value", context.Response.Headers["header2"]);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(context.Response.Body);
            var responseBody = reader.ReadToEnd();

            Assert.Equal("testhttpresponsebody", responseBody);
        }

        private class EndpointFeature : IEndpointFeature
        {
            public Endpoint? Endpoint { get; set; }
        }
    }
}