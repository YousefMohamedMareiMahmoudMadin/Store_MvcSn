using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store_Mvc.Models;
using Store_Mvc.Services;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Store_Mvc.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly IConfiguration configuration;
		private readonly EmailSender _emailSender;

		public AccountController(UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager, IConfiguration configuration, EmailSender emailSender)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.configuration = configuration;
			this._emailSender = emailSender;
		}

		public IActionResult Register()
		{
			if (signInManager.IsSignedIn(User))
			{
				return RedirectToAction("Index", "Home");
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterDto registerDto)
		{
			if (signInManager.IsSignedIn(User))
			{
				return RedirectToAction("Index", "Home");
			}

			if (!ModelState.IsValid)
			{
				return View(registerDto);
			}

			var user = new ApplicationUser()
			{
				FirstName = registerDto.FirstName,
				LastName = registerDto.LastName,
				UserName = registerDto.Email,
				Email = registerDto.Email,
				PhoneNumber = registerDto.PhoneNumber,
				Address = registerDto.Address,
				CreatedAt = DateTime.Now,
			};

			var result = await userManager.CreateAsync(user, registerDto.Password);

			if (result.Succeeded)
			{
				await userManager.AddToRoleAsync(user, "client");

				string fullName = user.FirstName + " " + user.LastName;
				string subject = "Welcome to Our Store";
				string message = $@"
					<div style='font-family: Arial, sans-serif;'>
						<h1>Welcome {fullName}!</h1>
						<p>Your account has been created successfully.</p>
						<p>You can now log in and start shopping for your favorite items.</p>
						<br>
						<p>Best Regards,<br>Store Management Team</p>
					</div>";

				await _emailSender.SendEmail(fullName, user.Email, subject, message);

				TempData["SuccessMessage"] = "Registration successful! Please login.";
				return RedirectToAction("Login", "Account");
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}
			return View(registerDto);
		}

		public async Task<IActionResult> Logout()
		{
			if (signInManager.IsSignedIn(User))
			{
				await signInManager.SignOutAsync();
			}
			return RedirectToAction("Index", "Home");
		}

		public IActionResult Login()
		{
			if (signInManager.IsSignedIn(User))
			{
				return RedirectToAction("Index", "Home");
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginDto loginDto)
		{
			if (!ModelState.IsValid)
			{
				return View(loginDto);
			}

			var user = await userManager.FindByEmailAsync(loginDto.Email);
			if (user == null)
			{
				ViewBag.ErrorMessage = "Invalid login attempt.";
				return View(loginDto);
			}

			var result = await signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password,
				loginDto.RememberMe, false);

			if (result.Succeeded)
			{
				return RedirectToAction("Index", "Home");
			}

			ViewBag.ErrorMessage = "Invalid login attempt.";
			return View(loginDto);
		}

		public IActionResult ForgotPassword()
		{
			if (signInManager.IsSignedIn(User))
			{
				return RedirectToAction("Index", "Home");
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ForgotPassword([Required, EmailAddress] string email)
		{
			ViewBag.Email = email;

			if (!ModelState.IsValid)
			{
				ViewBag.EmailError = "Invalid Email Address";
				return View();
			}

			var user = await userManager.FindByEmailAsync(email);

			if (user != null)
			{
				var token = await userManager.GeneratePasswordResetTokenAsync(user);
				string resetUrl = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme) ?? "URL Error";

				string username = user.FirstName + " " + user.LastName;
				string subject = "Password Reset Request";
				string message = $@"
					<h3>Dear {username},</h3>
					<p>Click the button below to reset your password:</p>
					<a href='{resetUrl}' style='padding: 10px 20px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px; display: inline-block;'>Reset My Password</a>
					<br><br>
					<p>Or copy this link to your browser:</p>
					<p>{resetUrl}</p>
                    <br>
                    <p>Best Regards,<br>Store Support Team</p>";

				await _emailSender.SendEmail(username, email, subject, message);
			}

			ViewBag.SuccessMessage = "Please check your email to reset your password!";
			return View();
		}

		public IActionResult ResetPassword(string? token, string? email)
		{
			if (token == null || email == null)
			{
				return RedirectToAction("Index", "Home");
			}

			var model = new PasswordResetDto
			{
				Token = token,
				Email = email
			};

			return View("PasswordReset", model);
		}

		[HttpPost]
		public async Task<IActionResult> ResetPassword(PasswordResetDto model)
		{
			if (!ModelState.IsValid)
			{
				return View("PasswordReset", model);
			}

			var user = await userManager.FindByEmailAsync(model.Email);
			if (user == null)
			{
				ViewBag.ErrorMessage = "Invalid request.";
				return View("PasswordReset", model);
			}

			var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);

			if (result.Succeeded)
			{
				ViewBag.SuccessMessage = "Password reset successfully!";
			}
			else
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}

			return View("PasswordReset", model);
		}
	}
}