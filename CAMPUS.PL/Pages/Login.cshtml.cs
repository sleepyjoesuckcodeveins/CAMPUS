using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Auth0.AspNetCore.Authentication;

namespace CAMPUS.PL.Pages
{
    public class LoginModel : PageModel
    {
        public Task OnGetAsync(string returnUrl = "/")
        {
            var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
                .WithRedirectUri(returnUrl)
                .Build();
            return HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);

        }
    }
}   
