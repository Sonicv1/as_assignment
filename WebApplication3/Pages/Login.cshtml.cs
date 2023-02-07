using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Model;
using WebApplication3.ViewModels;
using System.Net.Http;
using Newtonsoft.Json;
using reCAPTCHA.AspNetCore;
using Griesoft.AspNetCore.ReCaptcha;



namespace WebApplication3.Pages
{
    public class LoginModel : PageModel
    {

        [BindProperty]
        public Login LModel { get; set; }
        private readonly SignInManager<ApplicationUser> signInManager;

        private readonly HttpClient _httpClient;

        private readonly IHttpContextAccessor _httpContext;


        private readonly IRecaptchaService _recaptcha;

        public const string SessionKeyName = "_Name";
        public const string SessionKeyAge = "_Age";

        private readonly ILogger<LoginModel> _logger;




        public LoginModel(SignInManager<ApplicationUser> signInManager, HttpClient httpClient, IRecaptchaService recaptchaService, ILogger<LoginModel> logger, IHttpContextAccessor httpContext)
        {
            this.signInManager = signInManager;
            _httpClient = httpClient;
            _recaptcha = recaptchaService;
            _logger = logger;
            _httpContext = httpContext;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (signInManager.IsSignedIn(User))
            {
                return BadRequest("Already logged in");
            }
            else
            {
                return Page();
            }

        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {

                var token = LModel.CaptchaToken;
                var response = await _httpClient.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={"6LfUAFUkAAAAANNVreYNpYGLc4cy_IhmyIz2tFEk"}&response={token}");
                var result = JsonConvert.DeserializeObject<GoogleCaptchaResponse>(response);

                Console.WriteLine(result.Score);
                Console.WriteLine(result.Success);
                if (!result.Success)
                {
                    Console.WriteLine(result.Score);
                    // Handle reCAPTCHA failure
                    return BadRequest("reCAPTCHA failed");
                }
                else
                {
                    var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, LModel.RememberMe, false);

                    if (identityResult.Succeeded)
                    {
                        _httpContext.HttpContext.Session.SetString("Name", LModel.Email);
                        return RedirectToPage("Index");
                    }
                    else
                    {
                        return BadRequest("Username or Password incorrect");
                    }
                    ModelState.AddModelError("", "Username or Password incorrect");
                }

            }
            return Page();
        }

        public class GoogleCaptchaResponse
        {
            public double Score { get; set; }

            public bool Success { get; set; }
        }

    }
}
