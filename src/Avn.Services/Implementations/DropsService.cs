using Avn.Domain.Dtos.Externals.NftStorage;

namespace Avn.Services.Interfaces;

public class DropsService : IDropsService
{
    private readonly IAppUnitOfWork _uow;
    private readonly Lazy<INftStorageAdapter> _nftStorageAdaptar;
    private readonly Lazy<ITokensService> _tokensService;
    private readonly Lazy<IAttachmentService> _attachmentService;
    private readonly Lazy<ISubscriptionService> _subscriptionService;

    public DropsService(IAppUnitOfWork uow,
                        Lazy<INftStorageAdapter> nftStorageAdaptar,
                        Lazy<ITokensService> tokensService,
                        Lazy<IAttachmentService> attachmentService,
                        Lazy<ISubscriptionService> subscriptionService)
    {
        _uow = uow;
        _nftStorageAdaptar = nftStorageAdaptar;
        _tokensService = tokensService;
        _attachmentService = attachmentService;
        _subscriptionService = subscriptionService;
    }

    /// <summary>
    /// Store File into Attachment table
    /// Find Network Wages
    /// Create a drop for user
    /// Send a notification to user
    /// </summary>
    /// <param name="item"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<Guid>> CreateAsync(CreateDropDto item, CancellationToken cancellationToken = default)
    {
        var fileResult = await _attachmentService.Value.UploadFileAsync(item.File, cancellationToken);
        if (!fileResult.IsSuccess)
            return new ActionResponse<Guid>(ActionResponseStatusCode.BadRequest, BusinessMessage.InvalidFileContent);

        var subscriptionModel = await _subscriptionService.Value.GetCurrentModelAsync(item.UserId);

        Drop model = new()
        {
            InsertDate = DateTime.Now,
            DropStatus = DropStatus.Pending,
            Description = item.Description,
            IsActive = true,
            IsPrivate = item.IsPrivate,
            AttachmentId = fileResult.Data,
            CategoryType = item.CategotyType,
            Code = Guid.NewGuid(),
            Count = item.Count,
            DeliveryType = item.DeliveryType,
            EndDate = item.EndDate,
            Name = item.Name,
            NetworkId = item.NetworkId,
            ProjectId = item.ProjectId,
            UserId = item.UserId,
            ExpireDate = item.ExpireDate,
            IsVirtual = item.IsVirtual,
            Location = item.Location,
            StartDate = item.StartDate,
            Wages = 1.0M
        };

        await _uow.DropRepo.AddAsync(model, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return new ActionResponse<Guid>(model.Code);
    }




    public async Task<IActionResponse<IEnumerable<object>>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
        => new ActionResponse<IEnumerable<object>>(await _uow.DropRepo.GetAll().AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(row => new
                {
                    row.Id,
                    row.Code,
                    row.ProjectId,
                    Project = row.Project != null ? row.Project.Name : string.Empty,
                    row.UserId,
                    User = row.User != null ? row.User.FullName : string.Empty,
                    row.Name,
                    row.Description,
                    row.DeliveryType,
                    row.NetworkId,
                    Network = row.Network != null ? row.Network.Name : "",
                    row.DropUri,
                    row.Location,
                    row.StartDate,
                    row.EndDate,
                    row.ExpireDate,
                    row.IsPrivate,
                    row.IsVirtual,
                    row.CategoryType
                }).ToListAsync(cancellationToken));


    /// <summary>
    /// Deactive a drop with a code
    /// </summary>
    /// <param name="code"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<bool>> DeactiveAsync(Guid code, CancellationToken cancellationToken = default)
    {
        var model = await _uow.DropRepo.GetAll().FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
        if (model is null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, BusinessMessage.NotFound);

        model.IsActive = false;

        var result = await _uow.SaveChangesAsync(cancellationToken);
        if (!result.ToSaveChangeResult())
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError, BusinessMessage.ServerError);

        return new ActionResponse<bool>(true);
    }


    public async Task<IActionResponse<bool>> ConfirmAsync(int DropId, CancellationToken cancellationToken = default)
    {
        var model = await _uow.DropRepo.GetAll().FirstOrDefaultAsync(x => x.Id == DropId && x.DropStatus == DropStatus.Pending, cancellationToken);
        if (model is null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, BusinessMessage.NotFound);

        model.DropStatus = DropStatus.Confirmed;
        var result = await _uow.SaveChangesAsync(cancellationToken);
        if (!result.ToSaveChangeResult())
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError, BusinessMessage.ServerError);

        var nftStorageResult = await _nftStorageAdaptar.Value.UploadAsync(new UploadRequestDto(model.Name, model.Description, null, new
        {
            Project = model.Project.Name,
        }), cancellationToken);

        if (!nftStorageResult.IsSuccess)
            return new ActionResponse<bool>(nftStorageResult.StatusCode, nftStorageResult.Message);

        switch (model.DeliveryType)
        {
            case DeliveryType.Link:
                var tokens = Enumerable.Repeat(model, model.Count).Select(row => new CreateTokenDto
                {
                    DropId = model.Id
                }).ToList();
                var tokenResult = await _tokensService.Value.AddRangeAsync(tokens, cancellationToken);
                if (!tokenResult.IsSuccess)
                    return new ActionResponse<bool>(tokenResult.StatusCode, tokenResult.Message);
                break;
            case DeliveryType.QR:
                break;
            default:
                break;
        }

        return new ActionResponse<bool>(true);
    }

    /// <summary>
    /// this method called by admin, and rejects a drop for a resean
    /// </summary>
    /// <param name="dropId">primaryKey of drop entity</param>
    /// <param name="reviewMessage">reason</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<bool>> RejectAsync(int dropId, string reviewMessage, CancellationToken cancellationToken = default)
    {
        var model = await _uow.DropRepo.GetAll().FirstOrDefaultAsync(x => x.Id == dropId && x.DropStatus == DropStatus.Pending, cancellationToken);
        if (model is null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, BusinessMessage.NotFound);
        model.DropStatus = DropStatus.Rejected;
        model.ReviewMessage = reviewMessage;
        var result = await _uow.SaveChangesAsync(cancellationToken);
        if (!result.ToSaveChangeResult())
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError, BusinessMessage.ServerError);

        //TODO: Send an email to user with resean
        return new ActionResponse<bool>(true);
    }

    public Task<IActionResponse<string>> UploadFile(byte[] file)
    {
        throw new NotImplementedException();
    }
}
