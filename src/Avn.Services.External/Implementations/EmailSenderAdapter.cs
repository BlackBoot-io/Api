using Avn.Domain.Dtos;
using Avn.Services.External.Interfaces;
using Avn.Shared.Core;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace Avn.Services.External.Implementations;

public class EmailSenderAdapter : IEmailSenderAdapter
{
    private readonly IConfiguration _configuration;
    public EmailSenderAdapter(IConfiguration configuration)
        => _configuration = configuration;

    private async Task<EmailResponseDto> SendWithSendinBlue(EmailRequestDto email)
    {
        try
        {
            var apiInstance = new TransactionalEmailsApi();
            if (!Configuration.Default.ApiKey.ContainsKey("api-key"))
                Configuration.Default.ApiKey.Add("api-key", _configuration["EmailSettings:SendinBlueApiKey"]);

            SendSmtpEmailSender emailSender = new(_configuration["EmailSettings:SenderName"], _configuration["EmailSettings:Sender"]);
            SendSmtpEmailTo to = new(email.Receiver, email.Receiver);
            List<SendSmtpEmailTo> emailReceiver = new() { to };

            string Subject = email.Subject;
            string TextContent = null;
            string HtmlContent = email.Content;

            SendSmtpEmail smtpEmail = new(
                sender: emailSender,
                to: emailReceiver,
                bcc: null,
                cc: null,
                htmlContent: HtmlContent,
                textContent: TextContent,
                subject: Subject,
                replyTo: null,
                attachment: null,
                headers: null,
                templateId: null,
                _params: null,
                messageVersions: null,
                tags: null);

            var sendMailResult = await apiInstance.SendTransacEmailAsync(smtpEmail);
            if (!string.IsNullOrEmpty(sendMailResult.MessageId))
                return new(true, sendMailResult.MessageId?.Split("@")[0]?.Substring(1));
            else
                return new(false, string.Empty);
        }
        catch (SmtpCommandException ex)
        {
            return new(false, $"SmtpException {ex.Message}");
        }
        catch (Exception e)
        {
            return new(false, $"Exception {e.Message}");
        }
    }

    public async Task<IActionResponse> Send(EmailRequestDto email)
    {

        try
        {
            var sendResult = await SendWithSendinBlue(email);
            if (sendResult.IsSucess)
                return new ActionResponse(ActionResponseStatusCode.Success, $"Status: Success | ID: {sendResult.Message}");
            else
                return new ActionResponse(ActionResponseStatusCode.Success, $"Status: Fail | ID: {sendResult.Message}");
        }
        catch (Exception e)
        {
            return new ActionResponse(ActionResponseStatusCode.ServerError, $"Exception {e.Message}");
        }
    }
}
