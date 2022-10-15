namespace Avn.Services.Dtos;

public class TokenDto
{
    //Drop
    public int DropId { get; set; }
    public Guid DropCode { get; set; }
    public string DropName { get; set; }
    public string Network { get; set; }
    public DropCategoryType DropCategoryType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime ExpireDate { get; set; }

    //Token
    public Guid TokenId { get; set; }
    public string UniqueCode { get; set; }
    public string OwerWalletAddress { get; set; }
    public bool IsMinted { get; set; }
    public bool IsBurned { get; set; }
}
