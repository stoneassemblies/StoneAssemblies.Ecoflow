// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="StoneAssemblies">
// Copyright © 2023 - 2026 StoneAssemblies Development Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Spectre.Console;
using Spectre.Console.Cli;

using StoneAssemblies.Ecoflow.Cmd.Services;
using StoneAssemblies.Ecoflow.Cmd.Services.Interfaces;
using StoneAssemblies.Ecoflow.Extensions;

Console.OutputEncoding = Encoding.UTF8;

var services = new ServiceCollection();

services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

services.AddEcoflow();
services.AddSingleton<IAccountManager, AccountManager>();
services.AddSingleton<IDeviceManager, DeviceManager>();

var registrar = new TypeRegistrar(services);

var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.SetApplicationName("gwen");
    config.ValidateExamples();

    config.SetExceptionHandler((ex, resolver) =>
    {
        AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message}");
        return -1;
    });

    config.AddCommand<SyncAccountCommand>("sync");

    config.AddBranch("battery", battery =>
    {
        battery.AddCommand<BatteryInfoCommand>("info")
            .WithDescription("Shows battery information for a device");
    });
});

await app.RunAsync(args);