using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Net.Http.Json;
using Receptek.Front.MODEL;

namespace Receptek.Front.Controllers
{
    [ApiController]
    [Route("auth")]
    public class FrontAuthController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public FrontAuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

       
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            
            var client = _httpClientFactory.CreateClient("api");

            
            var response = await client.PostAsJsonAsync("api/auth/login", model);

            
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                return Unauthorized(string.IsNullOrWhiteSpace(err)
                    ? "Sikertelen bejelentkezés."
                    : err);
            }

            
            var loginResult = await response.Content.ReadFromJsonAsync<LoginResponse>();

            
            if (loginResult == null || string.IsNullOrWhiteSpace(loginResult.token))
            {
                return Unauthorized("Az API nem adott vissza tokent.");
            }

            
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(loginResult.token);

            
            var claims = new List<Claim>(jwt.Claims);

            
            if (!claims.Any(c => c.Type == ClaimTypes.Name))
            {
                var userNameClaim = claims.FirstOrDefault(c =>
                    c.Type == "unique_name" || c.Type == "name");

                if (userNameClaim != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, userNameClaim.Value));
                }
            }

           
            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            
            var principal = new ClaimsPrincipal(identity);

            
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            
            return Ok();
        }


        
        [HttpPost("login-form")]
        public async Task<IActionResult> LoginForm([FromForm] LoginRequest model, [FromForm] string? returnUrl)
        {
            var client = _httpClientFactory.CreateClient("api");

            var payload = new 
            {
                username = model.Username,
                email = "",
                password = model.Password
            };

            var response = await client.PostAsJsonAsync("api/auth/login", payload);

            if (!response.IsSuccessStatusCode)
            {
                return Redirect($"{returnUrl}?error=1");
            }

            var loginResult = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (loginResult == null || string.IsNullOrWhiteSpace(loginResult.token))
            {
                return Redirect($"{returnUrl}?error=1");
            }

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(loginResult.token);

            var claims = new List<Claim>(jwt.Claims);

            if (!claims.Any(c => c.Type == ClaimTypes.Name))
            {
                var userNameClaim = claims.FirstOrDefault(c =>
                    c.Type == "unique_name" || c.Type == "name");

                if (userNameClaim != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, userNameClaim.Value));
                }
            }

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return LocalRedirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl);
        }

        [HttpPost("logout-form")]
        public async Task<IActionResult> LogoutForm([FromForm] string? returnUrl)
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return LocalRedirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl);
        }




    }
}