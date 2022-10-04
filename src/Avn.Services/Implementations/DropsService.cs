using Avn.Domain.Dtos.Externals.NftStorage;
using Avn.Services.Interfaces.DeliveryStrategies;
using Microsoft.Extensions.Configuration;

namespace Avn.Services.Interfaces;

public class DropsService : IDropsService
{
    private readonly IAppUnitOfWork _uow;
    private readonly Lazy<INftStorageAdapter> _nftStorageAdaptar;
    private readonly Lazy<IAttachmentService> _attachmentService;
    private readonly Lazy<ISubscriptionsService> _subscriptionService;
    private readonly Lazy<INotificationsService> _notificationService;
    private readonly Lazy<IDeliveryFactory> _deliveryFactory;
    private readonly Lazy<IConfiguration> _configuration;

    public DropsService(IAppUnitOfWork uow,
                        Lazy<INftStorageAdapter> nftStorageAdaptar,
                        Lazy<ITokensService> tokensService,
                        Lazy<IAttachmentService> attachmentService,
                        Lazy<ISubscriptionsService> subscriptionService,
                        Lazy<INotificationsService> notificationService,
                        Lazy<IDeliveryFactory> deliveryFactory,
                        Lazy<IConfiguration> configuration)
    {
        _uow = uow;
        _nftStorageAdaptar = nftStorageAdaptar;
        _attachmentService = attachmentService;
        _subscriptionService = subscriptionService;
        _notificationService = notificationService;
        _deliveryFactory = deliveryFactory;
        _configuration = configuration;
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
        var fileBytes = new byte[item.File.Length];

        if (item.File.Length > 0)
        {
            using var ms = new MemoryStream();
            item.File.CopyTo(ms);
            fileBytes = ms.ToArray();
        }

        var fileResult = await _attachmentService.Value.UploadFileAsync(item.UserId, fileBytes, cancellationToken);
        if (!fileResult.IsSuccess)
            return new ActionResponse<Guid>(ActionResponseStatusCode.BadRequest, BusinessMessage.InvalidFileContent);

        var subscriptionModel = await _subscriptionService.Value.GetCurrentModelAsync(item.UserId, cancellationToken);
        if (!subscriptionModel.IsSuccess)
            return new ActionResponse<Guid>(ActionResponseStatusCode.BadRequest, BusinessMessage.InvalidSubscriptionModel);

        var networkInPricing = subscriptionModel.Data.Pricing.NetworkInPricings.FirstOrDefault(X => X.NetworkId == item.NetworkId);
        if (networkInPricing is null)
            return new ActionResponse<Guid>(ActionResponseStatusCode.BadRequest, BusinessMessage.InvalidNetwork);

        Drop drop = new()
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
            Wages = networkInPricing.Network.Wages,
            DropContentId = string.Empty,
            ReviewMessage = string.Empty,
            IsTest = item.IsTest,
            ImageContentId = string.Empty
        };

        await _uow.DropRepo.AddAsync(drop, cancellationToken);

        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse<Guid>(ActionResponseStatusCode.ServerError, BusinessMessage.ServerError);

        await _notificationService.Value.SendAsync(item.UserId,
            new()
            {
                { nameof(item.Name), item.Name },
                { nameof(item.Description), item.Description },
                { nameof(item.StartDate), item.StartDate.ToString() },
                { nameof(item.EndDate), item.EndDate.ToString() }
            }, template: TemplateType.CreateDrop);


        if (item.IsTest)
            await ConfirmAsync(drop.Code, cancellationToken);

        return new ActionResponse<Guid>(drop.Code);
    }

    /// <summary>
    /// Get a drop by dropId and userId
    /// </summary>
    /// <param name="userId">PK user entity</param>
    /// <param name="dropId">PK drop entity</param>
    /// <param name="cancellationToken"></param>
    /// <returns>object</returns>
    public async Task<IActionResponse<object>> GetAsync(Guid userId, int dropId, CancellationToken cancellationToken = default)
      => new ActionResponse<object>(await _uow.DropRepo.Queryable()
                 .AsNoTracking()
                 .Where(x => x.UserId == userId && x.Id == dropId)
                 .Select(row => new
                 {
                     row.Id,
                     row.Code,
                     row.Name,
                     row.Description,
                     DeliveryType = row.DeliveryType.ToString(),
                     row.DropContentId,
                     row.ImageContentId,
                     row.Location,
                     row.StartDate,
                     row.EndDate,
                     row.ExpireDate,
                     row.IsPrivate,
                     row.IsVirtual,
                     CategoryType = row.CategoryType.ToString(),
                     row.Count,
                     row.IsActive,
                     row.IsTest,
                     DropStatus = row.DropStatus.ToString(),
                     Project = new
                     {
                         Id = row.ProjectId,
                         Name = row.Project != null ? row.Project.Name : "",
                     },
                     Network = new
                     {
                         Id = row.NetworkId,
                         Name = row.Network != null ? row.Network.Name : "",
                     }
                 }).FirstOrDefaultAsync(cancellationToken));

    /// <summary>
    /// Get a drop by dropCode and userId
    /// </summary>
    /// <param name="userId">PK user entity</param>
    /// <param name="dropCode">PK drop entity</param>
    /// <param name="cancellationToken"></param>
    /// <returns>object</returns>
    public async Task<IActionResponse<object>> GetAsync(Guid userId, Guid dropCode, CancellationToken cancellationToken = default)
      => new ActionResponse<object>(await _uow.DropRepo.Queryable()
                 .AsNoTracking()
                 .Where(x => x.UserId == userId && x.Code == dropCode)
                 .Select(row => new
                 {
                     row.Id,
                     row.Code,
                     row.Name,
                     row.Description,
                     DeliveryType = row.DeliveryType.ToString(),
                     row.DropContentId,
                     row.ImageContentId,
                     row.Location,
                     row.StartDate,
                     row.EndDate,
                     row.ExpireDate,
                     row.IsPrivate,
                     row.IsVirtual,
                     CategoryType = row.CategoryType.ToString(),
                     row.Count,
                     row.IsActive,
                     row.IsTest,
                     DropStatus = row.DropStatus.ToString(),
                     Project = new
                     {
                         Id = row.ProjectId,
                         Name = row.Project != null ? row.Project.Name : "",
                     },
                     Network = new
                     {
                         Id = row.NetworkId,
                         Name = row.Network != null ? row.Network.Name : "",
                     }
                 }).FirstOrDefaultAsync(cancellationToken));

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
                    row.Name,
                    row.Description,
                    DeliveryType = row.DeliveryType.ToString(),
                    row.DropContentId,
                    row.ImageContentId,
                    row.Location,
                    row.StartDate,
                    row.EndDate,
                    row.ExpireDate,
                    row.IsPrivate,
                    row.IsVirtual,
                    CategoryType = row.CategoryType.ToString(),
                    row.Count,
                    row.IsActive,
                    row.IsTest,
                    DropStatus = row.DropStatus.ToString(),
                    Project = new
                    {
                        Id = row.ProjectId,
                        Name = row.Project != null ? row.Project.Name : "",
                    },
                    Network = new
                    {
                        Id = row.NetworkId,
                        Name = row.Network != null ? row.Network.Name : "",
                    }
                }).ToListAsync(cancellationToken));

    /// <summary>
    /// Deactive/Activate a drop with a code
    /// </summary>
    /// <param name="dropCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<bool>> ChangeStateAsync(Guid dropCode, CancellationToken cancellationToken = default)
    {
        var model = await _uow.DropRepo.Queryable().FirstOrDefaultAsync(x => x.Code == dropCode, cancellationToken);
        if (model is null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, BusinessMessage.NotFound);

        model.IsActive = !model.IsActive;

        var result = await _uow.SaveChangesAsync(cancellationToken);
        if (!result.ToSaveChangeResult())
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError, BusinessMessage.ServerError);

        return new ActionResponse<bool>(true);
    }

    public async Task<IActionResponse<bool>> ConfirmAsync(Guid dropCode, CancellationToken cancellationToken = default)
    {
        var drop = await _uow.DropRepo.Queryable()
                        .FirstOrDefaultAsync(x => x.Code == dropCode && x.DropStatus == DropStatus.Pending, cancellationToken);
        if (drop is null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, BusinessMessage.NotFound);

        drop.DropStatus = DropStatus.Confirmed;
        if (!drop.IsTest)
        {
            var attachment = await _attachmentService.Value.GetFile(drop.AttachmentId, cancellationToken);
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
                    Project = new
                    {
                        drop.Project?.Name,
                    }
                }), cancellationToken);

            if (!nftStorageResult.IsSuccess)
                return new ActionResponse<bool>(nftStorageResult.StatusCode, nftStorageResult.Message);

            drop.DropContentId = nftStorageResult.Data.ContentId;
            drop.ImageContentId = nftStorageResult.Data.ImageContentId;
        }
        try
        {
            var result = await _uow.SaveChangesAsync(cancellationToken);
            if (!result.ToSaveChangeResult())
                return new ActionResponse<bool>(ActionResponseStatusCode.ServerError, BusinessMessage.ServerError);
        }
        catch (Exception)
        {
            if (!drop.IsTest)
            {
                await _nftStorageAdaptar.Value.DeleteAsync(drop.DropContentId, cancellationToken);
                await _nftStorageAdaptar.Value.DeleteAsync(drop.ImageContentId, cancellationToken);
            }
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError, BusinessMessage.ServerError);
        }

        var deliveryResult = await _deliveryFactory.Value.GetInstance(drop.DeliveryType)
                                                         .ExecuteAsync(drop.Id, drop.Count, cancellationToken);

        if (!deliveryResult.IsSuccess)
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError, BusinessMessage.ServerError);

        await _notificationService.Value.SendAsync(drop.UserId,
             new()
             {
                 { nameof(drop.Name), drop.Name },
                 { nameof(drop.Description), drop.Description },
                 { nameof(drop.StartDate), drop.StartDate.ToString() },
                 { nameof(drop.EndDate), drop.EndDate.ToString() }
             }
            , TemplateType.ConfirmDrop, file: deliveryResult.Data);
        return new ActionResponse<bool>(true);
    }

    /// <summary>
    /// this method called by admin, and rejects a drop for a resean
    /// </summary>
    /// <param name="dropCode">primaryKey of drop entity</param>
    /// <param name="reviewMessage">reason</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<bool>> RejectAsync(Guid dropCode, string reviewMessage, CancellationToken cancellationToken = default)
    {
        var drop = await _uow.DropRepo.Queryable().FirstOrDefaultAsync(x => x.Code == dropCode && x.DropStatus == DropStatus.Pending, cancellationToken);
        if (drop is null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, BusinessMessage.NotFound);

        drop.DropStatus = DropStatus.Rejected;
        drop.ReviewMessage = reviewMessage;
        var result = await _uow.SaveChangesAsync(cancellationToken);

        if (!result.ToSaveChangeResult())
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError, BusinessMessage.ServerError);

        await _notificationService.Value.SendAsync(drop.UserId,
             new()
             {
                 { nameof(drop.Name), drop.Name },
                 { nameof(drop.Description), drop.Description },
                 { nameof(drop.StartDate), drop.StartDate.ToString() },
                 { nameof(drop.EndDate), drop.EndDate.ToString() }
             }, TemplateType.DropRejected);
        return new ActionResponse<bool>(true);
    }

    /// <summary>
    /// Get Image Uri In Ipfs
    /// Then notify the user
    /// </summary>
    /// <param name="dropCode">PrimaryKey of drop entity</param>
    /// <param name="cancellationToken"></param>
    /// <returns>string</returns>
    public async Task<IActionResponse<string>> GetImageUri(Guid dropCode, CancellationToken cancellationToken = default)
    {
        var drop = await _uow.DropRepo.Queryable().FirstOrDefaultAsync(x => x.Code == dropCode, cancellationToken);

        if (drop is null)
            return new ActionResponse<string>(ActionResponseStatusCode.NotFound, BusinessMessage.NotFound);

        if (drop.DropStatus != DropStatus.Confirmed)
            return new ActionResponse<string>(ActionResponseStatusCode.NotFound, BusinessMessage.DropNotConfirmed);

        if (drop.IsTest)
            return new ActionResponse<string>(ActionResponseStatusCode.NotFound, BusinessMessage.DropIsForTest);

        if (!string.IsNullOrEmpty(drop.ImageContentId))
            return new ActionResponse<string>(ActionResponseStatusCode.NotFound, BusinessMessage.DropHasNoImage);

        return new ActionResponse<string>(ActionResponseStatusCode.Redirect,
            data: $"{_configuration.Value["IPFS:Gateway:Url"]}/{drop.ImageContentId}");
    }

    /// <summary>
    /// For the specified drop ID, this endpoint returns paginated info on the token holders including
    /// the token ID, drop transfer count, 
    /// and the owner's information like address, and amount of drops owned.
    /// </summary>
    /// <param name="currentUserId"></param>
    /// <param name="dropCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<object>> GetAllHoldersAsync(Guid currentUserId, Guid dropCode, CancellationToken cancellationToken)
        => new ActionResponse<object>(await _uow.DropRepo.Queryable()
                             .Include(X => X.Tokens)
                             .Where(X => X.UserId == currentUserId && X.Code == dropCode)
                             .Select(X => new
                             {
                                 X.Name,
                                 X.Description,
                                 X.Count,
                                 Tokens = X.Tokens.Select(T => new
                                 {
                                     T.OwnerWalletAddress,
                                     T.IsMinted,
                                     T.Number,
                                     T.ContractTokenId,
                                     T.UniqueCode
                                 })
                             })
                            .AsNoTracking()
                            .ToListAsync(cancellationToken));
}