// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="StoneAssemblies">
// Copyright © 2023 - 2026 StoneAssemblies Development Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.Ecoflow.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;

    using StoneAssemblies.Ecoflow.Services;
    using StoneAssemblies.Ecoflow.Services.Interfaces;

    public static class ServiceCollectionExtensions
    {
        public static void AddEcoflow(this IServiceCollection serviceCollection)
        {
            ArgumentNullException.ThrowIfNull(serviceCollection);

            serviceCollection.AddHttpClient<IEcoflowClientService, EcoflowClientService>();
            serviceCollection.AddSingleton<IEcoflowSignatureService, EcoflowSignatureService>();
        }
    }
}