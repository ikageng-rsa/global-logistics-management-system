using GLMS.Tests.Helpers;
using GLMS.Web.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace GLMS.Tests
{
    public class CurrencyServiceTests
    {
        private readonly Mock<ILogger<ExchangeRateService>> _loggerMock;

        public CurrencyServiceTests()
        {
            _loggerMock = new Mock<ILogger<ExchangeRateService>>();
        }

        [Fact]
        public async Task ConvertUsdToZarAsync_ValidAmount_ReturnsConvertedValue()
        {
            var handler = new MockHttpMessageHandler(18.50m);
            var httpClient = new HttpClient(handler);
            var service = new ExchangeRateService(httpClient, _loggerMock.Object);

            var result = await service.ConvertUsdToZarAsync(100m);

            Assert.Equal(1850.00m, result);
        }

        [Fact]
        public async Task ConvertUsdToZarAsync_ZeroAmount_ReturnsZero()
        {
            var handler = new MockHttpMessageHandler(18.50m);
            var httpClient = new HttpClient(handler);
            var service = new ExchangeRateService(httpClient, _loggerMock.Object);

            var result = await service.ConvertUsdToZarAsync(0m);

            Assert.Equal(0m, result);
        }

        [Fact]
        public async Task ConvertUsdToZarAsync_NegativeAmount_ReturnsZero()
        {
            var handler = new MockHttpMessageHandler(18.50m);
            var httpClient = new HttpClient(handler);
            var service = new ExchangeRateService(httpClient, _loggerMock.Object);

            var result = await service.ConvertUsdToZarAsync(-50m);

            Assert.Equal(0m, result);
        }

        [Fact]
        public async Task ConvertUsdToZarAsync_ApiFailure_UsesFallbackRate()
        {
            var handler = new FailingHttpMessageHandler();
            var httpClient = new HttpClient(handler);
            var service = new ExchangeRateService(httpClient, _loggerMock.Object);

            var result = await service.ConvertUsdToZarAsync(100m);

            Assert.Equal(1850.00m, result);
        }
    }
}
