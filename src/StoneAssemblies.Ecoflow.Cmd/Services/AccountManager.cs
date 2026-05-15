// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountManager.cs" company="StoneAssemblies">
// Copyright © 2023 - 2026 StoneAssemblies Development Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.Ecoflow.Cmd.Services;

using System.Text.Json;

using Microsoft.Extensions.Logging;

using StoneAssemblies.Ecoflow.Cmd.Services.Interfaces;
using StoneAssemblies.Ecoflow.Services.Interfaces;

public class AccountManager : IAccountManager
{
    private readonly ILogger<AccountManager> logger;
    private readonly IEcoflowClientService ecoflowClientService;
    private readonly string devicesFolder;
    private readonly string rootDirectory;

    public AccountManager(ILogger<AccountManager> logger, IEcoflowClientService ecoflowClientService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(ecoflowClientService);

        this.logger = logger;
        this.ecoflowClientService = ecoflowClientService;
        this.rootDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ecoflow");

        this.devicesFolder = Path.Combine(this.rootDirectory, "devices");
        if (!Directory.Exists(this.devicesFolder))
        {
            Directory.CreateDirectory(this.devicesFolder);
        }
    }

    public string GetDeviceFile(string alias)
    {
        return Path.Combine(this.devicesFolder, $"{alias}.json");
    }

    public void Save(EcoFlowDevice device)
    {
        var file = this.GetDeviceFile(device.DeviceName);

        if (File.Exists(file))
        {
            throw new Exception($"A device with alias '{device.DeviceName}' already exists");
        }

        var json = JsonSerializer.Serialize(device, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(file, json);
    }

    public EcoFlowDevice? Load(string alias)
    {
        var file = this.GetDeviceFile(alias);

        if (!File.Exists(file))
        {
            return null;
        }

        var json = File.ReadAllText(file);
        return JsonSerializer.Deserialize<EcoFlowDevice>(json);
    }

    public IEnumerable<string> ListAliases()
    {
        return Directory
            .EnumerateFiles(this.devicesFolder, "*.json")
            .Select(Path.GetFileNameWithoutExtension);
    }

    public async Task InitAsync(string accountName, string accessKey, string secretKey)
    {
        var accountDirectory = this.GetAccountDirectory(accountName);
        if (!Directory.Exists(accountDirectory))
        {
            Directory.CreateDirectory(accountDirectory);
        }

        var accountInfo = new AccountInfo { AccessKey = accessKey, SecretKey = secretKey, };
        var combine = Path.Combine(accountDirectory, "info.json");
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        var json = JsonSerializer.Serialize(accountInfo, options);
        await File.WriteAllTextAsync(combine, json);
    }

    public async Task<AccountInfo?> GetAccountInfoAsync(string accountName)
    {
        var accountDirectory = this.GetAccountDirectory(accountName);
        var accountInfoFile = Path.Combine(accountDirectory, "info.json");
        if (File.Exists(accountInfoFile))
        {
            var json = await File.ReadAllTextAsync(accountInfoFile);
            AccountInfo? accountInfo = null;
            try
            {
                accountInfo = JsonSerializer.Deserialize<AccountInfo>(json);
            }
            catch (Exception ex)
            {
                this.logger.LogDebug(ex, "Error deserializing account info.");
            }

            return accountInfo;
        }

        return null;
    }

    public string GetAccountDirectory(string accountName)
    {
        return Path.Combine(this.rootDirectory, accountName);
    }

    public string GetRootDirectory()
    {
        return this.rootDirectory;
    }
}