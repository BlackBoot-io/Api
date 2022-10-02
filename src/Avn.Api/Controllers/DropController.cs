namespace Avn.Api.Controllers;

public class DropController : BaseController
{
    private readonly IDropsService _dropsService;
    public DropController(IDropsService dropsService) => _dropsService = dropsService;

    /// <summary>
    /// Store File into Attachment table
    /// Create a drop for user
    /// Send a notification to user
    /// </summary>
    /// <param name="item"></param>
    /// <param name="cancellationToken"></param>
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromForm] CreateDropDto item, CancellationToken cancellationToken = default)
    {
        item.UserId = CurrentUserId;
        return Ok(await _dropsService.CreateAsync(item, cancellationToken));
    }

    /// <summary>
    /// Get all drops of a user by UserId
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken = default)
        => Ok(await _dropsService.GetAllAsync(CurrentUserId, cancellationToken));

    /// <summary>
    /// Get a drop by id and userId
    /// </summary>
    /// <param name="dropId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("/drop/{dropId}")]
    public async Task<IActionResult> GetAsync(int dropId, CancellationToken cancellationToken = default)
        => Ok(await _dropsService.GetAsync(CurrentUserId, dropId, cancellationToken));

    /// <summary>
    /// Get a drop by id and userId
    /// </summary>
    /// <param name="dropId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("/drop/{dropCode:guid}")]
    public async Task<IActionResult> GetAsync(Guid dropCode, CancellationToken cancellationToken = default)
        => Ok(await _dropsService.GetAsync(CurrentUserId, dropCode, cancellationToken));


    /// <summary>
    /// Redirect To Ifps Gateway For Drops Image
    /// </summary>
    /// <param name="dropId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("/drop/{dropId}/image")]
    public async Task<IActionResult> GetImageUri(int dropId, CancellationToken cancellationToken = default)
        => Ok(await _dropsService.GetImageUri(dropId, cancellationToken));

    /// <summary>
    /// For the specified drop ID, this endpoint returns paginated info on the token holders including
    /// the token ID, drop transfer count, 
    /// and the owner's information like address, and amount of drops owned.
    /// </summary>
    /// <param name="dropId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("/drop/{dropId}/holders")]
    public async Task<IActionResult> GetAllHoldersAsync(int dropId, CancellationToken cancellationToken = default)
        => Ok(await _dropsService.GetAllHoldersAsync(CurrentUserId, dropId, cancellationToken));

    /// <summary>
    /// Deactive/Active a drop with a code
    /// </summary>
    /// <param name="code"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> ChangeStateAsync(Guid code, CancellationToken cancellationToken = default)
         => Ok(await _dropsService.ChangeStateAsync(code, cancellationToken));

    /// <summary>
    /// Confirm a drop by admin then store file (Image + Metadata) in IPFS
    /// Then update cid
    /// Execute Delivery Strategy (link or Qr)
    /// </summary>
    /// <param name="DropId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("/drop/{dropCode:guid}/confirm")]
    public async Task<IActionResult> ConfirmAsync(Guid dropCode, CancellationToken cancellationToken = default)
        => Ok(await _dropsService.ConfirmAsync(dropCode, cancellationToken));

    /// <summary>
    /// Reject a drop by admin for a resean
    /// Then notify the user
    /// </summary>
    /// <param name="dropId">PrimaryKey of drop entity</param>
    /// <param name="reviewMessage">resean message</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> RejectAsync(int dropId, string reviewMessage, CancellationToken cancellationToken = default)
        => Ok(await _dropsService.RejectAsync(dropId, reviewMessage, cancellationToken));
}