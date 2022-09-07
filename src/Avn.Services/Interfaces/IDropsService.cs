namespace Avn.Services.Interfaces;

public interface IDropsService : IScopedDependency
{
    /// <summary>
    /// Upload a file into IPFS network then returns CID
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    Task<IActionResponse<string>> UploadFile(byte[] file);

    /// <summary>
    /// Create a drop with Ui/Api
    /// </summary>
    /// <param name="item"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<Guid>> CreateAsync(CreateDropDto item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all drops of a user by UserId
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<IEnumerable<object>>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// update a drop by user such as disable drop or change name and description
    /// but this method is not executed when drop is confirmed
    /// </summary>
    /// <param name="code"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<bool>> UpdateAsync(UpdateDropDto code, CancellationToken cancellationToken = default);

    Task<IActionResponse<bool>> DeactiveAsync(Guid code, CancellationToken cancellationToken = default);


    Task<IActionResponse<bool>> ConfirmAsync(int DropId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reject a drop by admin for a resean
    /// </summary>
    /// <param name="dropId">PrimaryKey of drop entity</param>
    /// <param name="reviewMessage">resean message</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<bool>> RejectAsync(int dropId, string reviewMessage,CancellationToken cancellationToken = default);
}