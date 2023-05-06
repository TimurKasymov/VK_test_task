using DLL.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using UsersAPI.Services;
using UsersAPI.Services.EntityServices.DI;

namespace UsersAPI.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserService _userService;
        private readonly HashService _hashService;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            IUserService userService,
            HashService hashService,
            UrlEncoder encoder,
            ISystemClock clock) :
           base(options, logger, encoder, clock)
        {
            _hashService = hashService;
            _userService = userService;
        }

        private async Task<User?> Authenticate(string login, string password)
        {
            var hashed = _hashService.Hash(password);
            var found = await _userService.GetManyAsync(u => u.Password == hashed && u.Login == login && u.State.State == State.Active);
            return found?.FirstOrDefault();
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Path.Value!.Contains("create"))
            {
                var claimsPrincipal = new ClaimsPrincipal();
                return await Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
            }
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            if (authorizationHeader != null && authorizationHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
            {
                var token = authorizationHeader.Substring("Basic ".Length).Trim();
                var credentialsAsEncodedString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var credentials = credentialsAsEncodedString.Split(':');
                var userFound = await Authenticate(credentials[0], credentials[1]);
                if (userFound != null)
                {
                    var claims = new List<Claim>(){ new Claim("login", credentials[0])};
                    if (userFound.Group.Role == Role.Admin)
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                    var identity = new ClaimsIdentity(claims, "Basic");
                    var claimsPrincipal = new ClaimsPrincipal(identity);
                    return await Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
                }
            }
            Response.StatusCode = 401;
            Response.Headers.Add("WWW-Authenticate", "Basic realm=\"localhost\"");
            return await Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
        }
    }
}
