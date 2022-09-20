using Avn.Domain.Dtos.Externals.NftStorage;
using Avn.Services.Implementations;

namespace Avn.Services.Interfaces;

public class DropsService : IDropsService
{
    private readonly IAppUnitOfWork _uow;
    private readonly Lazy<INftStorageAdapter> _nftStorageAdaptar;
    private readonly Lazy<ITokensService> _tokensService;
    private readonly Lazy<IAttachmentService> _attachmentService;
    private readonly Lazy<ISubscriptionService> _subscriptionService;
    private readonly Lazy<INotificationService> _notificationService;
    private readonly Lazy<IUsersService> _userService;
    private readonly Lazy<IDeliveryFactory> _deliveryFactory;

    public DropsService(IAppUnitOfWork uow,
                        Lazy<INftStorageAdapter> nftStorageAdaptar,
                        Lazy<ITokensService> tokensService,
                        Lazy<IAttachmentService> attachmentService,
                        Lazy<ISubscriptionService> subscriptionService,
                        Lazy<INotificationService> notificationService,
                        Lazy<IUsersService> usersService,
                        Lazy<IDeliveryFactory> deliveryFactory)
    {
        _uow = uow;
        _nftStorageAdaptar = nftStorageAdaptar;
        _tokensService = tokensService;
        _attachmentService = attachmentService;
        _subscriptionService = subscriptionService;
        _notificationService = notificationService;
        _userService = usersService;
        _deliveryFactory = deliveryFactory;
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
        #region Validations
        //like time 
        //count
        //...
        #endregion
        var fileResult = await _attachmentService.Value.UploadFileAsync(item.File, cancellationToken);
        if (!fileResult.IsSuccess)
            return new ActionResponse<Guid>(ActionResponseStatusCode.BadRequest, BusinessMessage.InvalidFileContent);

        var subscriptionModel = await _subscriptionService.Value.GetCurrentModelAsync(item.UserId);
        if (!subscriptionModel.IsSuccess)
            return new ActionResponse<Guid>(ActionResponseStatusCode.BadRequest, BusinessMessage.InvalidSubscriptionModel);

        var networkInPricing = subscriptionModel.Data.Pricing.NetworkInPricings.FirstOrDefault(X => X.NetworkId == item.NetworkId);
        if (networkInPricing is null)
            return new ActionResponse<Guid>(ActionResponseStatusCode.BadRequest, BusinessMessage.InvalidNetwork);

        var code = Guid.NewGuid();
        await _uow.DropRepo.AddAsync(new()
        {
            InsertDate = DateTime.Now,
            DropStatus = DropStatus.Pending,
            Description = item.Description,
            IsActive = true,
            IsPrivate = item.IsPrivate,
            AttachmentId = fileResult.Data,
            CategoryType = item.CategotyType,
            Code = code,
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
            Wages = networkInPricing.Network.Wages,
            DropUri = string.Empty,
            ReviewMessage = string.Empty,
        }, cancellationToken);
        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse<Guid>(ActionResponseStatusCode.ServerError, BusinessMessage.ServerError);

        await _notificationService.Value.SendAsync(item.UserId,
                       template: TemplateType.CreateDrop);

        return new ActionResponse<Guid>(code);
    }

    /// <summary>
    /// Get all drops of a user by UserId
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<IEnumerable<object>>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
        => new ActionResponse<IEnumerable<object>>(await _uow.DropRepo.Queryable()
                .AsNoTracking()
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
        var model = await _uow.DropRepo.Queryable().FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
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
        var drop = await _uow.DropRepo.Queryable()
                        .FirstOrDefaultAsync(x => x.Id == DropId && x.DropStatus == DropStatus.Pending, cancellationToken);
        if (drop is null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, BusinessMessage.NotFound);

        var attachment = await _attachmentService.Value.GetFile(drop.AttachmentId);

        var nftStorageResult = await _nftStorageAdaptar.Value.UploadAsync(new UploadRequestDto(
            drop.Name,
            drop.Description,
            attachment.Content,
            new
            {
                drop.StartDate,
                drop.EndDate,
                drop.IsVirtual,
                drop.IsPrivate,
                drop.Location,
                drop.ExpireDate,
                drop.CategoryType,
                Project = drop.Project.Name,
            }), cancellationToken);

        if (!nftStorageResult.IsSuccess)
            return new ActionResponse<bool>(nftStorageResult.StatusCode, nftStorageResult.Message);

        drop.DropStatus = DropStatus.Confirmed;
        drop.DropUri = nftStorageResult.Data.ContentId;

        var result = await _uow.SaveChangesAsync(cancellationToken);
        if (!result.ToSaveChangeResult())
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError, BusinessMessage.ServerError);

        var deliveryResult = await _deliveryFactory.Value.GetInstance(drop.DeliveryType)
                                   .ExecuteAsync(drop.Id, drop.Count, cancellationToken);

        if (!deliveryResult.IsSuccess)
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError, BusinessMessage.ServerError);

        await _notificationService.Value.SendAsync(drop.UserId, TemplateType.ConfirmDrop, file: deliveryResult.Data);
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
        var model = await _uow.DropRepo.Queryable().FirstOrDefaultAsync(x => x.Id == dropId && x.DropStatus == DropStatus.Pending, cancellationToken);
        if (model is null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, BusinessMessage.NotFound);

        model.DropStatus = DropStatus.Rejected;
        model.ReviewMessage = reviewMessage;
        var result = await _uow.SaveChangesAsync(cancellationToken);

        if (!result.ToSaveChangeResult())
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError, BusinessMessage.ServerError);

        await _notificationService.Value.SendAsync(model.UserId, TemplateType.DropRejected);
        return new ActionResponse<bool>(true);
    }
}