﻿namespace Avn.Domain.Dtos;

public class TokenDto
{

    //Drop
    public string DropName { get; set; }
    public string Network { get; set; }
    public DropCategoryType DropCategoryType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime ExpireDate { get; set; }


    //token
    public string UniqueCode { get; set; }
    public string OwerWalletAddress { get; set; }
    public bool IsMinted { get; set; }
    public bool IsBurned { get; set; }

}
