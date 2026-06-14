using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace GLMS.Web.Handlers
{
    public class JwtAuthHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtAuthHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Retrieve the JWT stored in the user's authentication cookie claims
            var token = _httpContextAccessor.HttpContext?.User?
                .FindFirst("JWT")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}