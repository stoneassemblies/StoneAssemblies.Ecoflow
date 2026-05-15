// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EcoFlowDevice.cs" company="StoneAssemblies">
// Copyright © 2023 - 2026 StoneAssemblies Development Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

public class EcoFlowDevice
{
    public string DeviceName { get; set; } = string.Empty;

    public string SerialNumber { get; set; } = string.Empty;

    public string ProductName { get; set; }
}