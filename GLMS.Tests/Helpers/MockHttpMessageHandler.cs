using System.Net;
using System.Text;

namespace GLMS.Tests.Helpers
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly decimal _rate;

        public MockHttpMessageHandler(decimal rate)
        {
            _rate = rate;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var json = $@"{{
                ""result"": ""success"",
                ""rates"": {{
                    ""ZAR"": {_rate}
                }}
            }}";

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            return Task.FromResult(response);
        }
    }
}