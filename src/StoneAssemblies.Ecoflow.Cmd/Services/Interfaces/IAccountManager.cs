// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAccountManager.cs" company="StoneAssemblies">
// Copyright © 2023 - 2026 StoneAssemblies Development Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.Ecoflow.Cmd.Services.Interfaces;

public interface IAccountManager
{
    string GetDeviceFile(string alias);

    void Save(EcoFlowDevice device);

    EcoFlowDevice? Load(string alias);

    IEnumerable<string> ListAliases();

    Task InitAsync(string accountName, string accessKey, string secretKey);

    Task<AccountInfo?> GetAccountInfoAsync(string accountName);

    string GetAccountDirectory(string accountName);

    string GetRootDirectory();
}