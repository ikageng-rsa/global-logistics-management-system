using System.Text.Json;
using GLMS.Web.Services.Contracts;

namespace GLMS.Web.Services
{
    public class ExchangeRateService : ICurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExchangeRateService> _logger;

        // Free API — no key required
        private const string ApiUrl = "https://open.er-api.com/v6/latest/USD";

        public ExchangeRateService(HttpClient httpClient, ILogger<ExchangeRateService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(ApiUrl);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var document = JsonDocument.Parse(json);

                var rate = document.RootElement
                    .GetProperty("rates")
                    .GetProperty("ZAR")
                    .GetDecimal();

                return rate;
            }
            catch (Exception ex)
            {
                // Graceful fallback — log the error and return a safe default rate
                _logger.LogWarning("Currency API unavailable. Using fallback rate. Error: {Message}", ex.Message);
                return 18.50m;
            }
        }

        public async Task<decimal> ConvertUsdToZarAsync(decimal amountUsd)
        {
            if (amountUsd <= 0)
                return 0;

            var rate = await GetUsdToZarRateAsync();
            return Math.Round(amountUsd * rate, 2);
        }
    }
}