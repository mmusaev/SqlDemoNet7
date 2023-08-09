using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Net.Http.Headers;
using Okta.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



// create an HttpClient used for accessing the API
builder.Services.AddHttpClient("APIClient", client =>
{

});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddOktaMvc(new OktaMvcOptions
    {
        OktaDomain = builder.Configuration.GetValue<string>("Okta:OktaDomain"),

        ClientId = builder.Configuration.GetValue<string>("Okta:ClientId"),

        ClientSecret = builder.Configuration.GetValue<string>("Okta:ClientSecret"),

        AuthorizationServerId = builder.Configuration.GetValue<string>("Okta:AuthorizationServerId"),

        CallbackPath = builder.Configuration.GetValue<string>("Okta:CallbackPath"),

        Scope = builder.Configuration.GetValue<string>("Okta:Scopes").Split(",").Select(p => p).ToList()
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Country/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=SignIn}/{id?}");

app.Run();
