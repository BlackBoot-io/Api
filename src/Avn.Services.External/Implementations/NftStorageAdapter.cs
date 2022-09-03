using Avn.Domain.Dtos.Externals.NftStorage;
using Avn.Services.External.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


namespace Avn.Services.External.Implementations;


public class NftStorageAdapter : INftStorageAdapter
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    public NftStorageAdapter(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new System.Uri(Url);
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiKey);
    }

    private string Url => _configuration["NftStorage:Url"];
    private string ApiKey => _configuration["NftStorage:ApiKey"];




    public async Task<IActionResponse<UploadResponseDto>> Upload(UploadRequestDto item, CancellationToken cancellationToken = default)
    {

        var imageResult = await UploadInternalAsync(item.Image, cancellationToken);
        if (!imageResult.ok)
            return new ActionResponse<UploadResponseDto>(ActionResponseStatusCode.ServerError, imageResult.error.Name + ":" + imageResult.error.Message);

        var drop = new
        {
            Name = item.Name,
            Description = item.Description,
            Image = imageResult.value.Cid,
            Properties = item.MetaData
        };

        var dropRequest = JsonSerializer.Serialize(drop);
        var dropResult = await UploadInternalAsync(Encoding.UTF8.GetBytes(dropRequest), cancellationToken);
        if (!dropResult.ok)
            return new ActionResponse<UploadResponseDto>(ActionResponseStatusCode.ServerError, dropResult.error.Name + ":" + dropResult.error.Message);

        return new ActionResponse<UploadResponseDto>(new UploadResponseDto(dropResult.value.Cid));
    }



    private async Task<NftStorageUploadResponseDto> UploadInternalAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        var request = await _httpClient.PostAsync("Upload", new ByteArrayContent(data), cancellationToken);
        var response = await request.Content.ReadFromJsonAsync<NftStorageUploadResponseDto>();
        return response;
    }
}
