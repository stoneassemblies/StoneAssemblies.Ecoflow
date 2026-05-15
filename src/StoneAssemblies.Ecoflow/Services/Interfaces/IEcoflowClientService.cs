// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEcoflowClientService.cs" company="StoneAssemblies">
// Copyright © 2023 - 2026 StoneAssemblies Development Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.Ecoflow.Services.Interfaces;

using StoneAssemblies.Ecoflow.Models;

public interface IEcoflowClientService
{
    IAsyncEnumerable<EcoflowDevice> ListAsync(string accessKey, string secretKey);

    Task<string?> GetAllQuotaRawResponseAsync(string accessKey, string secretKey, string serialNumber);

    Task<EcoflowDevice?> GetDeviceAsync(string accessKey, string secretKey, string serialNumber);
}