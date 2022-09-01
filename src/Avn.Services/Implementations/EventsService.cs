using Avn.Data.UnitofWork;
using Avn.Domain.Dtos.Events;
using Avn.Domain.Entities;
using Avn.Domain.Enums;
using Avn.Services.External.Implementations;

namespace Avn.Services.Interfaces;

public class EventsService : IEventsService
{
    private readonly IAppUnitOfWork _uow;
    private readonly INftStorageAdapter _nftStorageAdaptar;
    public EventsService(IAppUnitOfWork uow, INftStorageAdapter nftStorageAdaptar)
    {
        _uow = uow;
        _nftStorageAdaptar = nftStorageAdaptar;
    }


    public async Task<IActionResponse<IEnumerable<EventDto>>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await _uow.EventRepo.GetAll().AsNoTracking().Where(x => x.UserId == userId).Select(row => new EventDto
        {
            Id = row.Id,
            Code = row.Code,
            ProjectId = row.ProjectId,
            Project = row.Project != null ? row.Project.Name : "",
            UserId = row.UserId,
            User = row.User != null ? row.User.FullName : "",
            Name = row.Name,
            Description = row.Description,
            DeliveryType = row.DeliveryType,
            NetworkId = row.NetworkId,
            Network = row.Network != null ? row.Network.Name : "",
            EventUri = row.DropUri,
            Location = row.Location,
            StartDate = row.StartDate,
            EndDate = row.EndDate,
            ExpireDate = row.ExpireDate,
            IsPrivate = row.IsPrivate,
            IsVirtual = row.IsVirtual

        }).ToListAsync(cancellationToken);
        return new ActionResponse<IEnumerable<EventDto>>(result);

    }

    public async Task<IActionResponse<Guid>> CreateAsync(CreateEventDto item, CancellationToken cancellationToken = default)
    {
        var model = new Drop { };

        await _uow.EventRepo.AddAsync(model, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return new ActionResponse<Guid>(model.Code);
    }

    public async Task<IActionResponse<bool>> DeactiveAsync(Guid code, CancellationToken cancellationToken = default)
    {
        var model = await _uow.EventRepo.GetAll().FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
        if (model == null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, AppResource.NotFound);

        model.IsActive = false;

        await _uow.SaveChangesAsync(cancellationToken);

        return new ActionResponse<bool>(true);
    }


    public async Task<IActionResponse<bool>> ConfirmAsync(int eventId, CancellationToken cancellationToken = default)
    {
        var model = await _uow.EventRepo.GetAll().FirstOrDefaultAsync(x => x.Id == eventId && x.DropStatus == DropStatus.Pending, cancellationToken);
        if (model == null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, AppResource.NotFound);

        model.DropStatus = DropStatus.Confirmed;


        var result = await _uow.SaveChangesAsync(cancellationToken);
        if (result.ToSaveChangeResult())
        {
            await _nftStorageAdaptar.Upload(new object(), cancellationToken);
            //ToDo:Should do The Strategy
        }
        return new ActionResponse<bool>(true);
    }

    public async Task<IActionResponse<bool>> RejectAsync(int eventId, CancellationToken cancellation = default)
    {
        var model = await _uow.EventRepo.GetAll().FirstOrDefaultAsync(x => x.Id == eventId && x.DropStatus == DropStatus.Pending, cancellation);
        if (model == null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, AppResource.NotFound);
        
        model.DropStatus = DropStatus.Rejected;

        await _uow.SaveChangesAsync(cancellation);

        return new ActionResponse<bool>(true);
    }
}
