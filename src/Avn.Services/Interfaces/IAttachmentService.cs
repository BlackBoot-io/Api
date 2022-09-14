namespace Avn.Services.Interfaces;

public interface IAttachmentService : IScopedDependency
{
    /// <summary>
    /// Upload a file into database and returns primaryKey
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    Task<IActionResponse<int>> UploadFile(string fileName, byte[] file, CancellationToken cancellationToken);
}