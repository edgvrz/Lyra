using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Lyra.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Lyra.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser>   _userManager;
        private readonly ILogger<RegisterModel>      _logger;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger)
        {
            _userManager   = userManager;
            _signInManager = signInManager;
            _logger        = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public string? ReturnUrl { get; set; }
        public IList<AuthenticationScheme>? ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required]
            [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar contraseña")]
            [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl      = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl    ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!ModelState.IsValid) return Page();

            var user = new IdentityUser
            {
                UserName = Input.Email,
                Email    = Input.Email
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("Usuario creado: {Email}", Input.Email);

                // ─── ASIGNACIÓN DE ROL POR DOMINIO ───────────────────────
                string rolAsignado;

                if (Input.Email.EndsWith("@admin.com", StringComparison.OrdinalIgnoreCase))
                {
                    rolAsignado = UserRoles.Admin;
                }
                else if (Input.Email.EndsWith("@tienda.com", StringComparison.OrdinalIgnoreCase))
                {
                    rolAsignado = UserRoles.Tienda;
                }
                else
                {
                    rolAsignado = UserRoles.Cliente;
                }

                await _userManager.AddToRoleAsync(user, rolAsignado);
                _logger.LogInformation("Rol '{Rol}' asignado a {Email}", rolAsignado, Input.Email);
                // ─────────────────────────────────────────────────────────

                await _signInManager.SignInAsync(user, isPersistent: false);

                // Redirigir según rol
                if (rolAsignado == UserRoles.Admin)
                    return RedirectToAction("Dashboard", "Admin");

                if (rolAsignado == UserRoles.Tienda)
                    return RedirectToAction("Dashboard", "Tienda");

                // Cliente → completar perfil
                return RedirectToAction("CompletarPerfil", "Cliente");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return Page();
        }
    }
}