// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDeviceManager.cs" company="StoneAssemblies">
// Copyright © 2023 - 2026 StoneAssemblies Development Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.Ecoflow.Cmd.Services.Interfaces;

public interface IDeviceManager
{
    IAsyncEnumerable<EcoFlowDevice> SyncDevicesAsync(string accountName);

    Task<string?> GetDeviceQuoteAsync(string deviceName, string? account);
}