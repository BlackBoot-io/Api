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
    public async Task<IActionResponse<Attachment>> GetFile(int attachmentId, CancellationToken cancellationToken)
    {
        var model = await _uow.AttachmentRepo.Queryable().FirstOrDefaultAsync(X => X.Id == attachmentId, cancellationToken);
        if (model is null)
            return new ActionResponse<Attachment>(ActionResponseStatusCode.NotFound, BusinessMessage.NotFound);

        return new ActionResponse<Attachment>(model);
    }

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
        _uow.AttachmentRepo.Add(model);
        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.IsSuccess)
            return new ActionResponse<int>(ActionResponseStatusCode.ServerError, dbResult.Message);
        return new ActionResponse<int>(model.Id);
    }
}
