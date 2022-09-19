﻿namespace Avn.Services.Interfaces;

public interface INotificationService : IScopedDependency
{
    /// <summary>
    /// send a notification to user such as sms or email or etc.
    /// </summary>
    /// <param name="template"></param>
    /// <param name="receiver"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    Task<IActionResponse> SendAsync(TemplateType template, string receiver, string content = "");
}