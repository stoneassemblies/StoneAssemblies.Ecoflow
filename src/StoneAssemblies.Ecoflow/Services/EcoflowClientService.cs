// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EcoflowClientService.cs" company="StoneAssemblies">
// Copyright © 2023 - 2026 StoneAssemblies Development Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.Ecoflow.Services;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using StoneAssemblies.Ecoflow.Extensions;
using StoneAssemblies.Ecoflow.Models;
using StoneAssemblies.Ecoflow.Services.Interfaces;

public class EcoflowClientService : IEcoflowClientService
{
    private const string DeviceBaseUrl = "https://api-e.ecoflow.com/iot-open/sign/device";

    private readonly ILogger<EcoflowClientService> logger;

    private readonly HttpClient httpClient;

    private readonly IEcoflowSignatureService ecoflowSignatureService;

    private readonly Random random = new Random();

    public EcoflowClientService(ILogger<EcoflowClientService> logger, HttpClient httpClient, IEcoflowSignatureService ecoflowSignatureService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(ecoflowSignatureService);

        this.logger = logger;
        this.httpClient = httpClient;
        this.ecoflowSignatureService = ecoflowSignatureService;
    }

    public async IAsyncEnumerable<EcoflowDevice> ListAsync(string accessKey, string secretKey)
    {
        this.logger.LogInformation("Listing Ecoflow devices");

        using var request = this.CreateSignedRequest($"{DeviceBaseUrl}/list", accessKey, secretKey);
        HttpResponseMessage? response;
        try
        {
            response = await this.httpClient.SendAsync(request);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error listing devices");
            yield break;
        }

        using (response)
        {
            var content = await response.Content.ReadAsStringAsync();
            this.logger.LogDebug("Received content: {Content}", content);

            if (!response.IsSuccessStatusCode)
            {
                this.logger.LogError("API Error: {Reason}. Content: {Content}", response.ReasonPhrase, content);

                yield break;
            }

            EcoflowResponse<List<EcoflowDevice>>? ecoflowResponse = null;
            try
            {
                var responseData = await response.Content.ReadAsStringAsync();

                ecoflowResponse = JsonConvert.DeserializeObject<EcoflowResponse<List<EcoflowDevice>>>(responseData);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error deserializing devices");
            }

            if (ecoflowResponse is not { Code: "0" })
            {
                yield break;
            }

            foreach (var ecoflowDevice in ecoflowResponse.Data)
            {
                ecoflowDevice.EnsureProductName();

                yield return ecoflowDevice;
            }
        }
    }

    public async Task<string?> GetAllQuotaRawResponseAsync(string accessKey, string secretKey, string serialNumber)
    {
        this.logger.LogInformation("Reading all quotas of device {SerialNumber}", serialNumber);

        using var request = this.CreateSignedRequest($"{DeviceBaseUrl}/quota/all?sn={serialNumber}", accessKey, secretKey, serialNumber);
        HttpResponseMessage? response = null;
        try
        {
            response = await this.httpClient.SendAsync(request);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error requesting quotes of device '{SerialNumber}'", serialNumber);
        }

        if (response is null)
        {
            return string.Empty;
        }

        var content = await response.Content.ReadAsStringAsync();
        this.logger.LogDebug("Received content: {Content}", content);

        if (!response.IsSuccessStatusCode)
        {
            this.logger.LogError("API Error: {Reason}. Content: {Content}", response.ReasonPhrase, content);

            return string.Empty;
        }

        using (response)
        {
            return content;
        }
    }

    public async Task<EcoflowDevice?> GetDeviceAsync(string accessKey, string secretKey, string serialNumber)
    {
        return await this.ListAsync(accessKey, secretKey).FirstOrDefaultAsync(device => device.Sn == serialNumber);
    }

    private HttpRequestMessage CreateSignedRequest(string url, string accessKey, string secretKey, string? serialNumber = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        var nonce = this.random.Next(10000, 100000).ToString();
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

        request.Headers.Add("accessKey", accessKey);
        request.Headers.Add("timestamp", timestamp);
        request.Headers.Add("nonce", nonce);

        this.logger.LogDebug("Creating signed request. Nonce: {Nonce}, Timestamp: {Timestamp}, SerialNumber: {SerialNumber}", nonce, timestamp, serialNumber ?? "(none)");

        var parameters = new Dictionary<string, object>();
        var sign = this.ecoflowSignatureService.GenerateSignature(accessKey, secretKey, parameters, nonce, timestamp, serialNumber);

        request.Headers.Add("sign", sign);

        this.logger.LogDebug("Creating signed request. Nonce: {Nonce}, Timestamp: {Timestamp}, SerialNumber: {SerialNumber}, Signature: {Signature}", nonce, timestamp, serialNumber ?? "(none)", sign);

        return request;
    }
}