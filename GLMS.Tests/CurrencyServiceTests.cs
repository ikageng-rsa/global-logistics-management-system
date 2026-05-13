using GLMS.Web.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLMS.Tests
{
    public class CurrencyServiceTests
    {
        private readonly Mock<ILogger<ExchangeRateService>> _loggerMock;

        public CurrencyServiceTests()
        {
            _loggerMock = new Mock<ILogger<ExchangeRateService>>();
        }
    }
}
