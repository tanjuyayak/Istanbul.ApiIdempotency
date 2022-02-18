using Istanbul.ApiIdempotency.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Moq;
using System;
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

        private class EndpointFeature : IEndpointFeature
        {
            public Endpoint? Endpoint { get; set; }
        }
    }
}