namespace Avn.Services.Interfaces;

public interface IDropsService : IScopedDependency
{
    /// <summary>
    /// Store File into Attachment table
    /// Create a drop for user
    /// Send a notification to user
    /// </summary>
    /// <param name="item"></param>
    /// <param name="cancellationToken"></param>
    Task<IActionResponse<Guid>> CreateAsync(CreateDropDto item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all drops of a user by UserId
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>object</returns>
    Task<IActionResponse<IEnumerable<object>>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disable/Active a drop with a code
    /// </summary>
    /// <param name="code"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>bool</returns>
    Task<IActionResponse<bool>> ChangeStateAsync(Guid code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirm a drop by admin then store file (Image + Metadata) in IPFS
    /// Then update cid
    /// Execute Delivery Strategy (link or Qr)
    /// </summary>
    /// <param name="DropId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>bool</returns>
    Task<IActionResponse<bool>> ConfirmAsync(int dropId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reject a drop by admin for a resean
    /// Then notify the user
    /// </summary>
    /// <param name="dropId">PrimaryKey of drop entity</param>
    /// <param name="reviewMessage">resean message</param>
    /// <param name="cancellationToken"></param>
    /// <returns>bool</returns>
    Task<IActionResponse<bool>> RejectAsync(int dropId, string reviewMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// For the specified drop ID, this endpoint returns paginated info on the token holders including
    /// the token ID, drop transfer count, 
    /// and the owner's information like address, and amount of drops owned.
    /// </summary>
    /// <param name="currentUserId"></param>
    /// <param name="dropId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<object>> GetAllHoldersAsync(Guid currentUserId, int dropId, CancellationToken cancellationToken);

    /// <summary>
    /// Get Image Uri In Ipfs
    /// Then notify the user
    /// </summary>
    /// <param name="dropId">PrimaryKey of drop entity</param>
    /// <param name="cancellationToken"></param>
    /// <returns>string</returns>
    Task<IActionResponse<string>> GetImageUri(int dropId, CancellationToken cancellationToken = default);
}