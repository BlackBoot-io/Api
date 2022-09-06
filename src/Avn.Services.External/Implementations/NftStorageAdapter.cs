using Avn.Domain.Dtos.Externals.NftStorage;
using Avn.Services.External.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
        _httpClient.BaseAddress = new Uri(Url);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
    }

    #region Private
    private string Url => _configuration["NftStorage:Url"];
    private string ApiKey => _configuration["NftStorage:ApiKey"];

    private async Task<NftStorageActionResponse> UploadInternalAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        var request = await _httpClient.PostAsync("Upload", new ByteArrayContent(data), cancellationToken);
        var response = await request.Content.ReadFromJsonAsync<NftStorageActionResponse>();
        return response;
    }
    #endregion

    public async Task<IActionResponse<IEnumerable<UploadResponseDto>>> GetAllAsync(DateTime startDate, int limit, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<NftStorageActionResponse<IEnumerable<NftStorageUploadResponseValue>>>($"?before={startDate}&limit={limit}", cancellationToken);
        if (!response.Ok)
            return new ActionResponse<IEnumerable<UploadResponseDto>>(ActionResponseStatusCode.ServerError, response.Error.Name + ":" + response.Error.Message);

        return new ActionResponse<IEnumerable<UploadResponseDto>>(response.Value.Select(row => new UploadResponseDto(row.CId)));
    }

    public async Task<IActionResponse<UploadResponseDto>> GetAsync(string cid, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<NftStorageActionResponse>($"{cid}", cancellationToken);
        if (!response.Ok)
            return new ActionResponse<UploadResponseDto>(ActionResponseStatusCode.ServerError, response.Error.Name + ":" + response.Error.Message);

        return new ActionResponse<UploadResponseDto>(new UploadResponseDto(response.Value.CId));
    }

    public async Task<IActionResponse<UploadResponseDto>> UploadAsync(UploadRequestDto item, CancellationToken cancellationToken = default)
    {

        var imageResponse = await UploadInternalAsync(item.Image, cancellationToken);
        if (!imageResponse.Ok)
            return new ActionResponse<UploadResponseDto>(ActionResponseStatusCode.ServerError, imageResponse.Error.Name + ":" + imageResponse.Error.Message);

        var model = new
        {
            Name = item.Name,
            Description = item.Description,
            Image = imageResponse.Value.CId,
            Properties = item.MetaData
        };


        var response = await UploadInternalAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(model)), cancellationToken);
        if (!response.Ok)
            return new ActionResponse<UploadResponseDto>(ActionResponseStatusCode.ServerError, response.Error.Name + ":" + response.Error.Message);

        return new ActionResponse<UploadResponseDto>(new UploadResponseDto(response.Value.CId));
    }


    public async Task<IActionResponse> DeleteAsync(string cid, CancellationToken cancellationToken = default)
    {
        var request = await _httpClient.DeleteAsync($"{cid}", cancellationToken);
        var response = await request.Content.ReadFromJsonAsync<NftStorageActionResponse>();
        if (!response.Ok)
            return new ActionResponse(ActionResponseStatusCode.ServerError, response.Error.Name + ":" + response.Error.Message);

        return new ActionResponse();
    }

}
