using Receptek.Front.Components;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers(); 

builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("https://localhost:44319/");
});

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:44319/"),
    Timeout = TimeSpan.FromSeconds(30) 
});


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => 
    {
        options.LoginPath = "/login"; 
        options.AccessDeniedPath = "/access-denied"; 
    });

builder.Services.AddAuthorization(); 
builder.Services.AddCascadingAuthenticationState(); 

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication(); 
app.UseAuthorization(); 


app.MapControllers(); 

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
