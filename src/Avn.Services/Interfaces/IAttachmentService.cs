namespace Avn.Services.Interfaces;

public interface IAttachmentService : IScopedDependency
{
    /// <summary>
    /// Upload a file into database and returns primaryKey
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    Task<IActionResponse<int>> UploadFileAsync(byte[] file, CancellationToken cancellationToken);

    /// <summary>
    /// Get Attachment File with Id
    /// </summary>
    /// <param name="attachmentId"></param>
    /// <returns></returns>
    Task<Attachment> GetFile(int attachmentId, CancellationToken cancellationToken);
}