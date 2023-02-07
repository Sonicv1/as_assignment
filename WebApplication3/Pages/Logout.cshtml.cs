using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Model;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication3.Pages
{
    [Authorize]
    public class LogoutModel : PageModel
    {

        private readonly SignInManager<ApplicationUser> signInManager;

        private readonly IHttpContextAccessor _httpContext;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContext)
        {
            this.signInManager = signInManager;
            _httpContext = httpContext;
        }
        public void OnGet()
        {
        }


        public async Task<IActionResult> OnPostLogoutAsync()
        {
            _httpContext.HttpContext.Session.Clear();
            await signInManager.SignOutAsync();
            return RedirectToPage("Login");
        }
        public async Task<IActionResult> OnPostDontLogoutAsync()
        {
            return RedirectToPage("Index");
        }
    }
}
