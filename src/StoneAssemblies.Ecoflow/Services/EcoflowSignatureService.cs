// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EcoflowSignatureService.cs" company="StoneAssemblies">
// Copyright © 2023 - 2026 StoneAssemblies Development Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.Ecoflow.Services;

using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Logging;

using StoneAssemblies.Ecoflow.Services.Interfaces;

public class EcoflowSignatureService : IEcoflowSignatureService
{
    private readonly ILogger<EcoflowSignatureService> logger;

    public EcoflowSignatureService(ILogger<EcoflowSignatureService> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        this.logger = logger;
    }

    public string GenerateSignature(string accessKey, string secretKey, Dictionary<string, object> parameters, string nonce, string timestamp, string? sn = "")
    {
        var concatenatedString = $"accessKey={accessKey}&nonce={nonce}&timestamp={timestamp}";
        if (!string.IsNullOrWhiteSpace(sn))
        {
            concatenatedString = $"sn={sn}&accessKey={accessKey}&nonce={nonce}&timestamp={timestamp}";
        }

        var sortedParams = SortAndConcatenateParameters(parameters);
        if (!string.IsNullOrEmpty(sortedParams))
        {
            concatenatedString = $"{sortedParams}&{concatenatedString}";
        }

        var signBytes = this.EncryptWithHMACSHA256(concatenatedString, secretKey);
        var sign = BytesToHexString(signBytes);

        this.logger.LogDebug("Signature string: {ConcatenatedString}, Signature: {Signature}", concatenatedString, sign);
        this.logger.LogDebug("SecretKey length: {Length}, SHA256: {Hash}", secretKey.Length, Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(secretKey))));

        return sign;
    }

    private static string SortAndConcatenateParameters(Dictionary<string, object> parameters, string prefix = "")
    {
        return string.Join("&", parameters.OrderBy(p => p.Key, StringComparer.Ordinal)
            .SelectMany(p => ExpandParameter(p.Key, p.Value, prefix)));
    }

    private static IEnumerable<string> ExpandParameter(string key, object value, string prefix)
    {
        string fullKey = string.IsNullOrEmpty(prefix) ? key : $"{prefix}.{key}";

        if (value is Dictionary<string, object> nestedDict)
        {
            return SortAndConcatenateParameters(nestedDict, fullKey).Split('&');
        }

        if (value is IEnumerable<object> list)
        {
            return list.Select((v, i) => $"{fullKey}[{i}]={v}");
        }

        return new[] { $"{fullKey}={value}" };
    }

    private byte[] EncryptWithHMACSHA256(string data, string secretKey)
    {
        using var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
        return hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(data));
    }

    private static string BytesToHexString(byte[] bytes)
    {
        var sb = new StringBuilder();
        foreach (var b in bytes)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
}