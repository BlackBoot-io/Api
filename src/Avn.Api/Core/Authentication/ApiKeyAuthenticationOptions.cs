// <copyright file="ApiKeyAuthenticationOptions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Authentication;

namespace Avn.Api.Core.Authentication;

/// <summary>
/// This class is for APIKey Authentication Options
/// such as Authentication Schema And Header Name On each request.
/// </summary>
public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// This field is for autentication schema on request.
    /// </summary>
    public const string AuthenticationScheme = "ApiKey";

    /// <summary>
    /// This field is for request header name.
    /// </summary>
    public const string HeaderName = "api-key";
}