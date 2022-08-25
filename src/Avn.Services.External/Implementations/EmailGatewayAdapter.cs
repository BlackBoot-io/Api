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

namespace Avn.Services.External.Implementations;

public class EmailGatewayAdapter : IEmailGatewayAdapter, IScopedDependency
{
    private readonly IConfiguration _configuration;
    public EmailGatewayAdapter(IConfiguration configuration)
        => _configuration = configuration;

    private (bool, string) SendWithSendinBlue(EmailDto email)
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

            var sendMailResult = apiInstance.SendTransacEmail(smtpEmail);
            if (!string.IsNullOrEmpty(sendMailResult.MessageId))
                return (true, sendMailResult.MessageId?.Split("@")[0]?.Substring(1));
            else
                return (false, string.Empty);
        }
        catch (SmtpCommandException ex)
        {
            return (false, $"SmtpException {ex.Message}");
        }
        catch (Exception e)
        {
            return (false, $"Exception {e.Message}");
        }
    }

    public IActionResponse<bool> Send(EmailDto email)
    {
        ActionResponse<bool> response = new();
        try
        {
            var sendResult = SendWithSendinBlue(email);
            if (sendResult.Item1)
            {
                response.Data = true;
                response.IsSuccess = true;
                response.Message = $"Status: Success | ID: {sendResult.Item2}";
            }
            else
            {
                response.Data = false;
                response.Message = $"Status: Fail | ID: {sendResult.Item2}";
            }

            return response;
        }
        catch (Exception e)
        {
            response.Message = $"Exception {e.Message}";
            return response;
        }
    }
}
