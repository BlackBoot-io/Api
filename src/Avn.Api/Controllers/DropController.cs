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
    public async Task<IActionResult> CreateAsync(CreateDropDto item, CancellationToken cancellationToken = default)
        => Ok(await _dropsService.CreateAsync(item, cancellationToken));

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
    /// Disable a drop with a code
    /// </summary>
    /// <param name="code"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> DeactiveAsync(Guid code, CancellationToken cancellationToken = default)
         => Ok(await _dropsService.DeactiveAsync(code, cancellationToken));

    /// <summary>
    /// Confirm a drop by admin then store file (Image + Metadata) in IPFS
    /// Then update cid
    /// Execute Delivery Strategy (link or Qr)
    /// </summary>
    /// <param name="DropId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> ConfirmAsync(int DropId, CancellationToken cancellationToken = default)
        => Ok(await _dropsService.ConfirmAsync(DropId, cancellationToken));

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