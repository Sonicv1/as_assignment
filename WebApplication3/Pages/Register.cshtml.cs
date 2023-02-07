using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Model;
using WebApplication3.ViewModels;
using Newtonsoft.Json;

namespace WebApplication3.Pages
{
    public class RegisterModel : PageModel
    {

        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }

        private readonly IHttpContextAccessor _httpContext;


        [BindProperty]
        public Register RModel { get; set; }

        private IWebHostEnvironment _environment;

        public RegisterModel(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager, IWebHostEnvironment environment, IHttpContextAccessor httpContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _environment = environment;
            _httpContext = httpContext;
        }





        public async Task<IActionResult> OnGetAsync()
        {
            if (signInManager.IsSignedIn(User)) {
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
                var userExist = await userManager.FindByEmailAsync(RModel.Email);

                if (userExist != null)
                {
                    return BadRequest("Email already exists");
                }
                else
                {
                    var ImageURL = "";

                    if (RModel.PhotoPath != null)
                    {
                        if (RModel.PhotoPath.Length > 2 * 1024 * 1024)
                        {
                            ModelState.AddModelError("Upload",
                            "File size cannot exceed 2MB.");
                            return Page();
                        }
                        var Upload = RModel.PhotoPath;
                        var uploadsFolder = "uploads";
                        var imageFile = Guid.NewGuid() + Path.GetExtension(
                        Upload.FileName);
                        var imagePath = Path.Combine(_environment.ContentRootPath,
                        "wwwroot", uploadsFolder, imageFile);
                        using var fileStream = new FileStream(imagePath,
                        FileMode.Create);
                        await Upload.CopyToAsync(fileStream);
                        ImageURL = string.Format("/{0}/{1}", uploadsFolder,
                        imageFile);
                    }
                    var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                    var protector = dataProtectionProvider.CreateProtector("MySecretKey");



                    var user = new ApplicationUser()
                    {
                        UserName = RModel.Email,
                        FullName = RModel.FullName,
                        Email = RModel.Email,
                        PhoneNumber = RModel.PhoneNumber,
                        CreditCard = protector.Protect(RModel.CreditCardNo),
                        Gender = RModel.Gender,
                        DeliveryAddress = RModel.DeliveryAddress,
                        PhotoPath = ImageURL.ToString(),
                        AboutMe = RModel.AboutMe


                    };

                    var result = await userManager.CreateAsync(user, RModel.Password);
                    if (result.Succeeded)
                    {
                        _httpContext.HttpContext.Session.SetString("Name", RModel.Email);
                        await signInManager.SignInAsync(user, false);
                        return RedirectToPage("Index");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return Page();

        }




    }
}
