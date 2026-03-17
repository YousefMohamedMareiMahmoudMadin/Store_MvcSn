using Microsoft.Extensions.Options;
using Resend;
using Store_Mvc.Models;

namespace Store_Mvc.Services
{
	public class EmailSender
	{
		private readonly IResend _resend;
		private readonly IConfiguration _configuration;

		public EmailSender(IResend resend, IConfiguration configuration)
		{
			_resend = resend;
			_configuration = configuration;
		}

		public async Task SendEmail(string toName, string toEmail, string subject, string htmlContent)
		{
			var senderEmail = _configuration["Resend:SenderEmail"] ?? "onboarding@resend.dev";
			var senderName = _configuration["Resend:SenderName"] ?? "Store_Mvc";

			var message = new EmailMessage();
			message.From = $"{senderName} <{senderEmail}>";
			message.To.Add(toEmail);
			message.Subject = subject;
			message.HtmlBody = htmlContent;

			try
			{
				await _resend.EmailSendAsync(message);
				Console.WriteLine("Email Sent Successfully via Resend");
			}
			catch (Exception e)
			{
				Console.WriteLine("Resend Email Failure: " + e.Message);
			}
		}
	}
}