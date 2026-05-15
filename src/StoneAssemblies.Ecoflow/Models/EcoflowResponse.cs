// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EcoflowResponse.cs" company="StoneAssemblies">
// Copyright © 2023 - 2026 StoneAssemblies Development Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.Ecoflow.Models;

public class EcoflowResponse<TPayload>
{
    public string Code { get; set; }

    public string Message { get; set; }

    public TPayload Data { get; set; }

    public string EagleEyeTraceId { get; set; }

    public string Tid { get; set; }
}