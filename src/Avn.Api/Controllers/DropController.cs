namespace Avn.Api.Controllers;

public class DropController : BaseController
{
    private readonly IDropsService _dropsService;
    public DropController(IDropsService dropsService)
    {
        _dropsService = dropsService;
    }


}