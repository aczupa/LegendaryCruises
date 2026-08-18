using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace LegendaryCruises.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public LoginModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [TempData]
        public string? InfoMessage { get; set; }

        public string? ReturnUrl { get; set; }

        public void OnGet(string? returnUrl = null, string? message = null)
        {
            ReturnUrl = returnUrl;

            if (message == "login-required")
            {
                InfoMessage = "Pour ajouter une croisière, vous devez d'abord vous connecter.";
            }
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Aucun compte n'est associé à cet e-mail. Veuillez d'abord vous inscrire.");
                    return Page();
                }

                var result = await _signInManager.PasswordSignInAsync(
                    Input.Email,
                    Input.Password,
                    isPersistent: false,
                    lockoutOnFailure: false
                );

                if (result.Succeeded)
                {
                    return LocalRedirect(ReturnUrl);
                }

                ModelState.AddModelError(string.Empty, "Tentative de connexion invalide. Vérifiez votre e-mail et mot de passe.");
            }

            return Page();
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}
