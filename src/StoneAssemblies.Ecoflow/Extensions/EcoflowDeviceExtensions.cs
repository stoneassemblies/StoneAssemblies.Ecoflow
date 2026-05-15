// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EcoflowDeviceExtensions.cs" company="StoneAssemblies">
// Copyright © 2023 - 2026 StoneAssemblies Development Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.Ecoflow.Extensions;

using StoneAssemblies.Ecoflow.Models;

public static class EcoflowDeviceExtensions
{
    public static void EnsureProductName(this EcoflowDevice device)
    {
        if (!string.IsNullOrWhiteSpace(device.ProductName))
        {
            return;
        }

        device.ProductName = EcoflowModels.WellKnownModels
            .FirstOrDefault(model => device.DeviceName.StartsWith(model, StringComparison.OrdinalIgnoreCase)) ?? EcoflowModels.Unknown;
    }
}