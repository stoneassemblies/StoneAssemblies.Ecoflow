// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAccountManager.cs" company="StoneAssemblies">
// Copyright © 2023 - 2026 StoneAssemblies Development Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.Ecoflow.Cmd.Services.Interfaces;

public interface IAccountManager
{
    Task InitAsync(string accountName, string accessKey, string secretKey);

    Task<AccountInfo?> GetAccountInfoAsync(string accountName);

    string GetAccountDirectory(string accountName);

    string GetRootDirectory();
}