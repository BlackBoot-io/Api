using Avn.Data.UnitofWork;
using Avn.Domain.Dtos.Externals.NftStorage;
using Avn.Services.External.Implementations;

namespace Avn.Services.Interfaces;

public class DropsService : IDropsService
{
    private readonly IAppUnitOfWork _uow;
    private readonly INftStorageAdapter _nftStorageAdaptar;
    public DropsService(IAppUnitOfWork uow, INftStorageAdapter nftStorageAdaptar)
    {
        _uow = uow;
        _nftStorageAdaptar = nftStorageAdaptar;
    }

    public async Task<IActionResponse<IEnumerable<DropDto>>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await _uow.DropRepo.GetAll().AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(row => new DropDto
            {
                Id = row.Id,
                Code = row.Code,
                ProjectId = row.ProjectId,
                Project = row.Project != null ? row.Project.Name : string.Empty,
                UserId = row.UserId,
                User = row.User != null ? row.User.FullName : string.Empty,
                Name = row.Name,
                Description = row.Description,
                DeliveryType = row.DeliveryType,
                NetworkId = row.NetworkId,
                Network = row.Network != null ? row.Network.Name : "",
                DropUri = row.DropUri,
                Location = row.Location,
                StartDate = row.StartDate,
                EndDate = row.EndDate,
                ExpireDate = row.ExpireDate,
                IsPrivate = row.IsPrivate,
                IsVirtual = row.IsVirtual

            }).ToListAsync(cancellationToken);
        return new ActionResponse<IEnumerable<DropDto>>(result);

    }

    public async Task<IActionResponse<Guid>> CreateAsync(CreateDropDto item, CancellationToken cancellationToken = default)
    {
        var model = new Drop { };

        await _uow.DropRepo.AddAsync(model, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return new ActionResponse<Guid>(model.Code);
    }

    public async Task<IActionResponse<bool>> DeactiveAsync(Guid code, CancellationToken cancellationToken = default)
    {
        var model = await _uow.DropRepo.GetAll().FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
        if (model == null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, BusinessMessage.NotFound);

        model.IsActive = false;

        await _uow.SaveChangesAsync(cancellationToken);

        return new ActionResponse<bool>(true);
    }


    public async Task<IActionResponse<bool>> ConfirmAsync(int DropId, CancellationToken cancellationToken = default)
    {
        var model = await _uow.DropRepo.GetAll().FirstOrDefaultAsync(x => x.Id == DropId && x.DropStatus == DropStatus.Pending, cancellationToken);
        if (model == null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, BusinessMessage.NotFound);

        model.DropStatus = DropStatus.Confirmed;


        var result = await _uow.SaveChangesAsync(cancellationToken);
        if (result.ToSaveChangeResult())
        {
            await _nftStorageAdaptar.Upload(new UploadRequestDto(model.Name, model.Description, null, new
            {
                Project = model.Project.Name,
            }), cancellationToken);
            //ToDo:Should do The Strategy
        }
        return new ActionResponse<bool>(true);
    }

    public async Task<IActionResponse<bool>> RejectAsync(int DropId, CancellationToken cancellation = default)
    {
        var model = await _uow.DropRepo.GetAll().FirstOrDefaultAsync(x => x.Id == DropId && x.DropStatus == DropStatus.Pending, cancellation);
        if (model == null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, BusinessMessage.NotFound);

        model.DropStatus = DropStatus.Rejected;

        await _uow.SaveChangesAsync(cancellation);

        return new ActionResponse<bool>(true);
    }
}
