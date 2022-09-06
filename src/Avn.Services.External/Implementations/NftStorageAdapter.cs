using Avn.Domain.Dtos.Externals.NftStorage;
using Avn.Services.External.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


namespace Avn.Services.External.Implementations;

/// <summary>
/// Ineteract With IPFS Through NftStorage
/// </summary>
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


    private async Task<IActionResponse<UploadResponseDto>> UploadInternalAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        var request = await _httpClient.PostAsync("Upload", new ByteArrayContent(data), cancellationToken);
        var response = await request.Content.ReadFromJsonAsync<NftStorageActionResponse>();
        if (!request.IsSuccessStatusCode)
            return new ActionResponse<UploadResponseDto>((ActionResponseStatusCode)request.StatusCode, response.Error.ToString());

        return new ActionResponse<UploadResponseDto>(new UploadResponseDto(response.Value.CId));

    }
    #endregion
    /// <summary>
    /// Get All Uploaded File In IPFS
    /// </summary>
    /// <param name="endDate">results created before provided timestamp </param>
    /// <param name="limit">number of result to return</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<IEnumerable<UploadResponseDto>>> GetAllAsync(DateTime endDate, int limit, CancellationToken cancellationToken = default)
    {
        var request = await _httpClient.GetAsync($"?before={endDate}&limit={limit}", cancellationToken);
        var response = await request.Content.ReadFromJsonAsync<NftStorageActionResponse<IEnumerable<NftStorageUploadResponseValue>>>();
        if (!request.IsSuccessStatusCode)
            return new ActionResponse<IEnumerable<UploadResponseDto>>((ActionResponseStatusCode)request.StatusCode, response.Error.ToString());

        return new ActionResponse<IEnumerable<UploadResponseDto>>(response.Value.Select(row => new UploadResponseDto(row.CId)));
    }

    /// <summary>
    /// Get Specific Uploaded File In IPFS
    /// </summary>
    /// <param name="cid">Content Id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<UploadResponseDto>> GetAsync(string contentId, CancellationToken cancellationToken = default)
    {
        var request = await _httpClient.GetAsync($"{contentId}", cancellationToken);
        var response = await request.Content.ReadFromJsonAsync<NftStorageActionResponse>();
        if (!request.IsSuccessStatusCode)
            return new ActionResponse<UploadResponseDto>((ActionResponseStatusCode)request.StatusCode, response.Error.ToString());

        return new ActionResponse<UploadResponseDto>(new UploadResponseDto(response.Value.CId));
    }
    /// <summary>
    /// Upload File In IPFS
    /// </summary>
    /// <param name="item"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<UploadResponseDto>> UploadAsync(UploadRequestDto item, CancellationToken cancellationToken = default)
    {

        var imageResponse = await UploadInternalAsync(item.Image, cancellationToken);
        if (!imageResponse.IsSuccess)
            return imageResponse;

        var model = new
        {
            Name = item.Name,
            Description = item.Description,
            Image = imageResponse.Data.ContentId,
            Properties = item.Properties
        };


        var response = await UploadInternalAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(model)), cancellationToken);
        if (!response.IsSuccess)
            return response;


        return response;
    }

    /// <summary>
    /// Delete Uploaded File in IPFS
    /// </summary>
    /// <param name="contentId"> contentId </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse> DeleteAsync(string contentId, CancellationToken cancellationToken = default)
    {
        var request = await _httpClient.DeleteAsync($"{contentId}", cancellationToken);
        var response = await request.Content.ReadFromJsonAsync<NftStorageActionResponse>();
        if (!request.IsSuccessStatusCode)
            return new ActionResponse((ActionResponseStatusCode)request.StatusCode, response.Error.ToString());

        return new ActionResponse();
    }

}
