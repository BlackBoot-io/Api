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

/// <summary>
/// Ineteract With IPFS Through NftStorage
/// </summary>
public class NftStorageAdapter : INftStorageAdapter
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private string Url => _configuration["IPFS:NftStorage:Url"];
    private string ApiKey => _configuration["IPFS:NftStorage:ApiKey"];

    public NftStorageAdapter(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri(Url);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
    }

    #region Private
    private async Task<IActionResponse<string>> UploadInternalAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        var request = await _httpClient.PostAsync($"{Url}/Upload", new ByteArrayContent(data), cancellationToken);
        var response = await request.Content.ReadFromJsonAsync<NftStorageActionResponse>(cancellationToken: cancellationToken);
        if (!request.IsSuccessStatusCode)
            return new ActionResponse<string>((ActionResponseStatusCode)request.StatusCode, response.Error.ToString());

        return new ActionResponse<string>(response.Value.CId);
    }
    #endregion
    /// <summary>
    /// Get All Uploaded File In IPFS
    /// </summary>
    /// <param name="endDate">results created before provided timestamp </param>
    /// <param name="limit">number of result to return</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<IEnumerable<GetResponseDto>>> GetAllAsync(DateTime endDate, int limit, CancellationToken cancellationToken = default)
    {
        var request = await _httpClient.GetAsync($"{Url}?before={endDate}&limit={limit}", cancellationToken);
        var response = await request.Content.ReadFromJsonAsync<NftStorageActionResponse<IEnumerable<NftStorageUploadResponseValue>>>(cancellationToken: cancellationToken);
        if (!request.IsSuccessStatusCode)
            return new ActionResponse<IEnumerable<GetResponseDto>>((ActionResponseStatusCode)request.StatusCode, response.Error.ToString());

        return new ActionResponse<IEnumerable<GetResponseDto>>(response.Value.Select(row => new GetResponseDto(row.CId)));
    }

    /// <summary>
    /// Get Specific Uploaded File In IPFS
    /// </summary>
    /// <param name="contentId">Content Id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<GetResponseDto>> GetAsync(string contentId, CancellationToken cancellationToken = default)
    {
        var request = await _httpClient.GetAsync($"{contentId}", cancellationToken);
        var response = await request.Content.ReadFromJsonAsync<NftStorageActionResponse>(cancellationToken: cancellationToken);
        if (!request.IsSuccessStatusCode)
            return new ActionResponse<GetResponseDto>((ActionResponseStatusCode)request.StatusCode, response.Error.ToString());

        return new ActionResponse<GetResponseDto>(new GetResponseDto(response.Value.CId));
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
            return new ActionResponse<UploadResponseDto>(imageResponse.StatusCode, imageResponse.Message);


        var response = await UploadInternalAsync(
             Encoding.UTF8.GetBytes(JsonSerializer.Serialize(
                 new
                 {
                     item.Name,
                     item.Description,
                     Image = imageResponse.Data,
                     item.Properties
                 })), cancellationToken);

        if (!response.IsSuccess)
            return new ActionResponse<UploadResponseDto>(response.StatusCode, response.Message);


        return new ActionResponse<UploadResponseDto>(new UploadResponseDto(response.Data, imageResponse.Data));


    }

    /// <summary>
    /// Delete Uploaded File in IPFS
    /// </summary>
    /// <param name="contentId"> contentId </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse> DeleteAsync(string contentId, CancellationToken cancellationToken = default)
    {
        var request = await _httpClient.DeleteAsync(contentId, cancellationToken);
        var response = await request.Content.ReadFromJsonAsync<NftStorageActionResponse>(cancellationToken: cancellationToken);
        if (!request.IsSuccessStatusCode)
            return new ActionResponse((ActionResponseStatusCode)request.StatusCode, response.Error.ToString());

        return new ActionResponse();
    }
}