using Receptek.Front.Components;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers(); // Ez a szolgáltatás teszi lehetővé, hogy a Razor komponensekben használhassuk a [ApiController] attribútummal ellátott vezérlőket,
                                   // például a FrontAuthController-t, amely a bejelentkezési logikát kezeli

// Nevesített HttpClient szolgáltatás hozzáadása, amelyet a helyi Controller hív.
builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("http://localhost:44319/");
});

// Megmarad a razor oldalaknak.
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:44319/"),
    Timeout = TimeSpan.FromSeconds(30) // Ne várjon túl sokáig a válaszra
});

//builder.Services.AddRazorComponents()
//    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // Ebben az alkalmazásban a cookie-alapú hitelesítést használjuk
    .AddCookie(options => // Cookie-alapú hitelesítéshez szükséges metódusok:cookie létrehozás, olvasás, ...,  útvonalak
    {
        options.LoginPath = "/login"; // Ha a felhasználó nincs hitelesítve, akkor ide lesz átirányítva
        options.AccessDeniedPath = "/access-denied"; // Ha a felhasználó hitelesített, de nincs jogosultsága egy erőforráshoz, akkor ide lesz átirányítva
    });

builder.Services.AddAuthorization(); // Ez a szolgáltatás felelős a jogosultságok kezeléséért, például hogy egy adott felhasználó hozzáférhet-e egy adott erőforráshoz
builder.Services.AddCascadingAuthenticationState(); // Ez a szolgáltatás lehetővé teszi, hogy a hitelesítési állapotot (például hogy a felhasználó be van-e jelentkezve)
                                                    // átadjuk a Razor komponenseknek, így azok ennek megfelelően tudnak viselkedni
                                                    // (például megjeleníteni egy "Bejelentkezés" gombot, ha a felhasználó nincs bejelentkezve, vagy egy "Kijelentkezés" gombot, ha be van jelentkezve).

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Futtasd le a szolgáltatásokat minden kérésnél, hogy a hitelesítési és jogosultsági logika érvényesüljön
app.UseAuthentication(); // Ki vagy?
app.UseAuthorization();  // Mit szabad neked?

// Előbb a middleware-eket kell meghívni, utána a Controllerek.

app.MapControllers(); // Ez a sor teszi lehetővé, hogy a Razor komponensekben használhassuk a [ApiController] attribútummal ellátott vezérlőket,

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
