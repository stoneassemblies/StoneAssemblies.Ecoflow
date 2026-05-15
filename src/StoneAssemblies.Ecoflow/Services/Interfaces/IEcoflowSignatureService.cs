// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEcoflowSignatureService.cs" company="StoneAssemblies">
// Copyright © 2023 - 2026 StoneAssemblies Development Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.Ecoflow.Services.Interfaces;

public interface IEcoflowSignatureService
{
    string GenerateSignature(string accessKey, string secretKey, Dictionary<string, object> parameters, string nonce, string timestamp, string? sn = "");
}