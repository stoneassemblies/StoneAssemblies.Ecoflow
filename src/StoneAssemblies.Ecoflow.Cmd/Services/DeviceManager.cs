// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceManager.cs" company="StoneAssemblies">
// Copyright © 2023 - 2026 StoneAssemblies Development Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.Ecoflow.Cmd.Services;

using System.Net.NetworkInformation;
using System.Text.Json;

using Microsoft.Extensions.Logging;

using StoneAssemblies.Ecoflow.Cmd.Services.Interfaces;
using StoneAssemblies.Ecoflow.Services.Interfaces;

public class DeviceManager : IDeviceManager
{
    private readonly ILogger<DeviceManager> logger;
    private readonly IAccountManager accountManager;
    private readonly IEcoflowClientService ecoflowClientService;

    public DeviceManager(ILogger<DeviceManager> logger, IAccountManager accountManager,
        IEcoflowClientService ecoflowClientService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(accountManager);
        ArgumentNullException.ThrowIfNull(ecoflowClientService);

        this.logger = logger;
        this.accountManager = accountManager;
        this.ecoflowClientService = ecoflowClientService;
    }

    public async IAsyncEnumerable<EcoFlowDevice> SyncDevicesAsync(string accountName)
    {
        var accountInfo = await this.accountManager.GetAccountInfoAsync(accountName);
        if (accountInfo == null)
        {
            throw new InvalidOperationException(
                "No valid account information was found for the specified account. Please initialize or synchronize the account before trying again.");
        }

        var accountDirectory = this.accountManager.GetAccountDirectory(accountName);
        var dirInfo = new DirectoryInfo(accountDirectory);
        foreach (var file in dirInfo.GetFiles("*.json")
                     .Where(f => !f.Name.Equals("info.json", StringComparison.OrdinalIgnoreCase)))
        {
            file.Delete();
        }

        var options = new JsonSerializerOptions { WriteIndented = true };

        await foreach (var device in this.ecoflowClientService.ListAsync(accountInfo.AccessKey, accountInfo.SecretKey))
        {
            var ecoFlowDevice = new EcoFlowDevice
            {
                ProductName = device.ProductName,
                DeviceName = device.DeviceName,
                SerialNumber = device.Sn,
            };

            var json = JsonSerializer.Serialize(ecoFlowDevice, options);
            await File.WriteAllTextAsync(Path.Combine(accountDirectory, $"{ecoFlowDevice.SerialNumber}.json"), json);

            yield return ecoFlowDevice;
        }
    }

    public async Task<string?> GetDeviceQuoteAsync(string deviceName, string? account)
    {
        await foreach (var tuple in this.EnumDevicesAsync(account))
        {
            if (tuple.EcoFlowDevice.DeviceName == deviceName)
            {
                return await this.ecoflowClientService.GetAllQuotaRawResponseAsync(
                    tuple.AccountInfo.AccessKey,
                    tuple.AccountInfo.SecretKey, tuple.EcoFlowDevice.SerialNumber);
            }
        }

        return null;
    }

    private async IAsyncEnumerable<(AccountInfo AccountInfo, EcoFlowDevice EcoFlowDevice)> EnumDevicesAsync(string? account)
    {
        var directory = string.IsNullOrWhiteSpace(account)
            ? this.accountManager.GetRootDirectory()
            : this.accountManager.GetAccountDirectory(account);

        var directoryInfo = new DirectoryInfo(directory);
        foreach (var file in directoryInfo.GetFiles("*.json", SearchOption.AllDirectories)
                     .Where(f => !f.Name.Equals("info.json", StringComparison.OrdinalIgnoreCase)))
        {
            var accountName = file.Directory!.Name;
            var accountInfo = await this.accountManager.GetAccountInfoAsync(accountName);
            if (accountInfo is not null)
            {
                var json = await File.ReadAllTextAsync(file.FullName);
                var ecoFlowDevice = JsonSerializer.Deserialize<EcoFlowDevice>(json);
                if (ecoFlowDevice is not null)
                {
                    yield return (accountInfo, ecoFlowDevice);
                }
            }
        }
    }
}