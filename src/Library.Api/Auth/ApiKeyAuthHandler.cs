using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Library.Api.Auth
{
    public class ApiKeyAuthHandler(
        IOptionsMonitor<ApiKeyAuthSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<ApiKeyAuthSchemeOptions>(options, logger, encoder)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(HeaderNames.Authorization, out StringValues value))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
            }

            var header = value.ToString();
            if (header != Options.ApiKey)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
            }

            var claims = new[] {
            new Claim(ClaimTypes.Email, "enesmete@kafali.com"),
            new Claim(ClaimTypes.Name, "enesmetek.com") };

            var claimsIdentity = new ClaimsIdentity(claims, "ApiKey");

            var ticket = new AuthenticationTicket(
                new ClaimsPrincipal(claimsIdentity), Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
