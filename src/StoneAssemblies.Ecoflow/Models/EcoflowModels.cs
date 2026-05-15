// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EcoflowModels.cs" company="StoneAssemblies">
// Copyright © 2023 - 2026 StoneAssemblies Development Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.Ecoflow.Models;

public static class EcoflowModels
{
    public static readonly IReadOnlySet<string> WellKnownModels = new HashSet<string>(new[]
    {
        "DELTA 2",
        "DELTA 2 Max",
        "DELTA 3",
        "DELTA 3 Plus",
        "DELTA 3 Classic",
        "DELTA 3 Max",
        "DELTA 3 Max Plus",
        "DELTA 3 Ultra",
        "DELTA 3 Ultra Plus",
        "DELTA 3 1500",
        "DELTA Max",
        "DELTA Pro",
        "DELTA Mini",
        "River",
        "River Max",
        "River Pro",
        "River 2",
        "River 2 Max",
        "River 2 Pro",
        "Alternator Charger",
        "Smart Generator",
        "Glacier",
        "Wave",
        "Blade",
        "PowerStream",
        "Solar Tracker",
    });

    public const string Unknown = "Unknown";
}