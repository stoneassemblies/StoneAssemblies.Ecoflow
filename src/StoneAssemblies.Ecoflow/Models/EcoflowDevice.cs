// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EcoflowDevice.cs" company="StoneAssemblies">
// Copyright © 2023 - 2026 StoneAssemblies Development Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.Ecoflow.Models;

public class EcoflowDevice
{
    public string Sn { get; set; }

    public string DeviceName { get; set; }

    public bool Online { get; set; } // 0 = offline, 1 = online

    public string ProductName { get; set; }
}