using JuanProject.Migrations;
using JuanProject.Models;
using JuanProject.ViewModels;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MimeKit;
using MimeKit.Text;

namespace JuanProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid) return View(login);
            AppUser user = await _userManager.FindByEmailAsync(login.UsernameorEmail);
            if (user == null)
            {
                user=await _userManager.FindByNameAsync(login.UsernameorEmail);
            }
            if (user==null)
            {
                ModelState.AddModelError("", "Email or password is wrong");
                return View(login);
            }
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError("", "your email is not confirm");
            }
            var result = await _signInManager.PasswordSignInAsync(user, login.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Email or password is wrong");
                return View(login);
            }
            return RedirectToAction("index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = new();
            user.Email = register.Email;
            user.FullName = register.FullName;
            user.UserName = register.Username;

            IdentityResult result = await _userManager.CreateAsync(user, register.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors )
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(register);
            }
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string link = Url.Action(nameof(ConfirmEmail), "account", new { userid = user.Id, token },
                Request.Scheme, Request.Host.ToString());



            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("rovshanakh@code.edu.az"));
            email.To.Add(MailboxAddress.Parse(user.Email));
            email.Subject = "Verify email";

            string body = string.Empty;
            using(StreamReader reader=new StreamReader("wwwroot/Template/Verify.html"))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{{link}}", link);
            body = body.Replace("{{Fullname}}", user.FullName);

            email.Body=new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com",587,SecureSocketOptions.StartTls);
            smtp.Authenticate("rovshanakh@code.edu.az", "vqanyjlbrwjycxab");
            smtp.Send(email);
            smtp.Disconnect(true);

            return RedirectToAction(nameof(VerifyEmail));
        } 

        public IActionResult VerifyEmail()
        {
            return View();
        }
        public async Task<IActionResult> ConfirmEmail(string userid,string token)
        {
            if (userid == null || token == null) return NotFound();

            AppUser user= await _userManager.FindByIdAsync(userid);
            if (user == null) return NotFound();

            await _userManager.ConfirmEmailAsync(user, token);
            _signInManager.SignInAsync(user, false);
            return RedirectToAction(nameof(Login));
        } 
    }
}
