namespace Avn.Services.Implementations;

public class AttachmentService : IAttachmentService
{
    private readonly IAppUnitOfWork _uow;
    public AttachmentService(IAppUnitOfWork unitOfWork) => _uow = unitOfWork;

    /// <summary>
    /// Get Attachment File with Id
    /// </summary>
    /// <param name="attachmentId"></param>
    /// <returns></returns>
    public async Task<Attachment> GetFile(int attachmentId, CancellationToken cancellationToken) =>
        await _uow.AttachmentRepo.Queryable().FirstOrDefaultAsync(X => X.Id == attachmentId, cancellationToken);

    /// <summary>
    /// Add Image on data base and return primary Key
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="file"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<int>> UploadFileAsync(Guid userId, byte[] file, CancellationToken cancellationToken)
    {
        Attachment model = new()
        {
            Content = file,
            InsertDate = DateTime.Now,
            UserId = userId
        };
        await _uow.AttachmentRepo.AddAsync(model, cancellationToken);
        var result = await _uow.SaveChangesAsync(cancellationToken);

        if (!result.IsSuccess)
            return new ActionResponse<int>(ActionResponseStatusCode.ServerError, BusinessMessage.ServerError);
        return new ActionResponse<int>(model.Id);
    }
}
