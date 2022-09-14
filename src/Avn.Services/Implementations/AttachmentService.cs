namespace Avn.Services.Implementations;

public class AttachmentService : IAttachmentService
{
    private readonly IAppUnitOfWork _uow;
    public AttachmentService(IAppUnitOfWork unitOfWork) => _uow = unitOfWork;

    /// <summary>
    /// Add Image on data base and return primary Key
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="file"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<int>> UploadFile(string fileName, byte[] file, CancellationToken cancellationToken)
    {
        Attachment model = new()
        {
            Content = file,
            InsertDate = DateTime.Now,
            Name = fileName
        };
        await _uow.AttachmentRepo.AddAsync(model);
        var result = await _uow.SaveChangesAsync(cancellationToken);

        if (!result.ToSaveChangeResult())
            return new ActionResponse<int>(ActionResponseStatusCode.ServerError, BusinessMessage.ServerError);
        return new ActionResponse<int> { IsSuccess = true, Data = model.Id };
    }
}
