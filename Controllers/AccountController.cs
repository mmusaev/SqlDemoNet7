using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Okta.AspNetCore;

namespace SqlDemo.Controllers;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> SignIn()
    {
        if (!HttpContext.User.Identity.IsAuthenticated)
        {
            var properties = new AuthenticationProperties();
            return Challenge(properties, OktaDefaults.MvcAuthenticationScheme);
        }

        if (HttpContext.User.Identity is { IsAuthenticated: true })
        {
            return RedirectToAction("Index", "Country");
        }

        return View();
    }

    [HttpPost(Name = "signin-oidc-okta")]
    [ValidateAntiForgeryToken]
    public IActionResult SignIn([FromForm] string sessionToken)
    {
        if (!HttpContext.User.Identity.IsAuthenticated)
        {
            var properties = new AuthenticationProperties();
            properties.Items.Add("sessionToken", sessionToken);
            return Challenge(properties, OktaDefaults.MvcAuthenticationScheme);
        }

        return RedirectToAction("Index", "Country");
    }

    [HttpPost(Name = "signout-oidc-okta")]
    public IActionResult SignOut()
    {
        return new SignOutResult(
            new[]
            {
                OktaDefaults.MvcAuthenticationScheme,
                CookieAuthenticationDefaults.AuthenticationScheme,
            },
            new AuthenticationProperties { RedirectUri = "/Country/" });
    }
}