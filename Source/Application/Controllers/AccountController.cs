using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Models.Views.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AuthenticationOptions = Application.Models.Configuration.AuthenticationOptions;

namespace Application.Controllers
{
	public class AccountController : SiteController
	{
		#region Constructors

		public AccountController(IOptionsMonitor<AuthenticationOptions> authenticationOptionsMonitor, ILoggerFactory loggerFactory) : base(loggerFactory)
		{
			this.AuthenticationOptionsMonitor = authenticationOptionsMonitor ?? throw new ArgumentNullException(nameof(authenticationOptionsMonitor));
		}

		#endregion

		#region Properties

		protected internal virtual IOptionsMonitor<AuthenticationOptions> AuthenticationOptionsMonitor { get; }

		#endregion

		#region Methods

		public virtual async Task<IActionResult> AccessDenied()
		{
			return await Task.FromResult(this.View());
		}

		[SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates")]
		[SuppressMessage("Usage", "CA2254:Template should be a static expression")]
		protected internal virtual string ResolveAndValidateReturnUrl(string returnUrl)
		{
			returnUrl = this.ResolveReturnUrl(returnUrl);

			// ReSharper disable InvertIf
			if(!this.Url.IsLocalUrl(returnUrl))
			{
				var message = $"The return-url \"{returnUrl}\" is invalid";

				this.Logger.LogError(message);

				throw new InvalidOperationException(message);
			}
			// ReSharper restore InvertIf

			return returnUrl;
		}

		protected internal virtual string ResolveReturnUrl(string returnUrl)
		{
			return string.IsNullOrEmpty(returnUrl) ? "~/" : returnUrl;
		}

		public virtual async Task<IActionResult> SignIn(string returnUrl)
		{
			returnUrl = this.ResolveAndValidateReturnUrl(returnUrl);

			var model = new SignInViewModel
			{
				Form =
				{
					ReturnUrl = returnUrl
				}
			};

			return await Task.FromResult(this.View(model));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual async Task<IActionResult> SignIn(SignInForm form, string returnUrl)
		{
			if(form == null)
				throw new ArgumentNullException(nameof(form));

			returnUrl = this.ResolveAndValidateReturnUrl(returnUrl);

			var configuredSecret = this.AuthenticationOptionsMonitor.CurrentValue.Secret ?? string.Empty;
			var secret = form.Secret ?? string.Empty;

			if(string.Equals(configuredSecret, secret, StringComparison.Ordinal))
			{
				var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "Secret-user") }, "Secret"));

				await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

				return this.LocalRedirect(returnUrl);
			}

			var model = new SignInViewModel
			{
				Form = form,
				InvalidSecret = true
			};

			model.Form.ReturnUrl = returnUrl;

			return await Task.FromResult(this.View(model));
		}

		public virtual async Task<IActionResult> SignOut(object _)
		{
			return await Task.FromResult(this.View(new SignOutViewModel()));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual async Task<IActionResult> SignOut(SignOutForm form)
		{
			if(this.User is { Identity: { IsAuthenticated: true } })
				await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			this.HttpContext.Items["SignedOut"] = true;

			return this.View("SignedOut");
		}

		#endregion
	}
}