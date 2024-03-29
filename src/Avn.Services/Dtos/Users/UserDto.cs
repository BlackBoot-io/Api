﻿namespace Avn.Services.Dtos;

public record UserDto
{
    public string Email { get; set; }
    public bool EmailIsApproved { get; set; }
    public string FullName { get; set; } = default!;
    public string Type { get; set; }
    public string OrganizationName { get; set; } = default!;
    public string WalletAddress { get; set; }
    public bool IsActive { get; set; }
}