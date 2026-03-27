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
    /// Cokiek elkészítése, olvasása, törlése, ... a Blazor Front alkalmazásban.
    public class FrontAuthController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public FrontAuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Bejelentkezési végpont a Blazor Front alkalmazásban.
        /// Ez a metódus:
        /// 1. Meghívja a Backend API login végpontját
        /// 2. Megkapja a JWT tokent
        /// 3. A tokenből kinyeri a felhasználó adatait (claims)
        /// 4. Cookie alapú hitelesítéssel bejelentkezteti a felhasználót a Front alkalmazásban
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            /// <summary>
            /// Létrehozunk egy HttpClient példányt a "api" nevű konfiguráció alapján.
            /// Ez a Backend API hívására szolgál.
            /// </summary>
            var client = _httpClientFactory.CreateClient("api");

            /// <summary>
            /// Meghívjuk a Backend API login végpontját.
            /// A felhasználónév és jelszó JSON formátumban kerül elküldésre.
            /// </summary>
            var response = await client.PostAsJsonAsync("api/auth/login", model);

            /// <summary>
            /// Ha a Backend API nem adott sikeres választ,
            /// akkor a hibaüzenetet visszaadjuk a kliensnek.
            /// </summary>
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                return Unauthorized(string.IsNullOrWhiteSpace(err)
                    ? "Sikertelen bejelentkezés."
                    : err);
            }

            /// <summary>
            /// A Backend API válaszából kiolvassuk a tokent.
            /// A LoginResponse osztály tartalmazza a token mezőt.
            /// </summary>
            var loginResult = await response.Content.ReadFromJsonAsync<LoginResponse>();

            /// <summary>
            /// Ha nincs token, akkor nem tudjuk folytatni a bejelentkezést.
            /// </summary>
            if (loginResult == null || string.IsNullOrWhiteSpace(loginResult.token))
            {
                return Unauthorized("Az API nem adott vissza tokent.");
            }

            /// <summary>
            /// A JWT token feldolgozása:
            /// - dekódoljuk
            /// - kinyerjük belőle a claim-eket (felhasználó adatai)
            /// </summary>
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(loginResult.token);

            /// <summary>
            /// A tokenből származó claim-eket listába gyűjtjük.
            /// Ezek lesznek a felhasználó azonosító adatai.
            /// </summary>
            var claims = new List<Claim>(jwt.Claims);

            /// <summary>
            /// Biztosítjuk, hogy legyen Name claim (felhasználónév),
            /// mert a Blazor UI ezt használja (pl. menüben kiírás).
            /// </summary>
            if (!claims.Any(c => c.Type == ClaimTypes.Name))
            {
                var userNameClaim = claims.FirstOrDefault(c =>
                    c.Type == "unique_name" || c.Type == "name");

                if (userNameClaim != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, userNameClaim.Value));
                }
            }

            /// <summary>
            /// Létrehozzuk a ClaimsIdentity-t a cookie hitelesítési sémával.
            /// Ez mondja meg, hogy cookie-alapú auth-ot használunk.
            /// </summary>
            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            /// <summary>
            /// A ClaimsPrincipal a teljes felhasználói identitás,
            /// amely tartalmazza a claim-eket és az identity-t.
            /// </summary>
            var principal = new ClaimsPrincipal(identity);

            /// <summary>
            /// A felhasználó tényleges bejelentkeztetése:
            /// - létrejön az auth cookie
            /// - a böngésző eltárolja
            /// - a következő kéréseknél automatikusan elküldi
            /// </summary>
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            /// <summary>
            /// Sikeres bejelentkezés esetén OK választ adunk vissza.
            /// </summary>
            return Ok();
        }


        /// <summary>
        /// Böngészőből érkező form POST alapú bejelentkezés.
        /// A sikeres backend login után cookie-val bejelentkezteti a felhasználót,
        /// majd átirányít a megadott oldalra.
        /// </summary>
        [HttpPost("login-form")]
        public async Task<IActionResult> LoginForm([FromForm] LoginRequest model, [FromForm] string? returnUrl)
        {
            var client = _httpClientFactory.CreateClient("api");

            var payload = new // Az eredeti user helyett, mivel nam adunk meg emailt.
            {
                username = model.Username,
                email = "",
                password = model.Password
            };

            var response = await client.PostAsJsonAsync("api/auth/login", payload);

            if (!response.IsSuccessStatusCode)
            {
                return Redirect($"/login?error=1");
            }

            var loginResult = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (loginResult == null || string.IsNullOrWhiteSpace(loginResult.token))
            {
                return Redirect($"/login?error=1");
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