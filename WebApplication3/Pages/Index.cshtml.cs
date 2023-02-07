using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebApplication3.Model;


namespace WebApplication3.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly IHttpContextAccessor _httpContext;

        private readonly UserManager<ApplicationUser> userManager;



        public IndexModel(ILogger<IndexModel> logger, IHttpContextAccessor httpContext, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _httpContext = httpContext;
            this.userManager = userManager;
        }

        public void OnGet()
        {

        }
    }
}