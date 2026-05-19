// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncAccountCommand.cs" company="StoneAssemblies">
// Copyright © 2023 - 2026 StoneAssemblies Development Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;

using Spectre.Console;
using Spectre.Console.Cli;

using StoneAssemblies.Ecoflow.Cmd.Services.Interfaces;
using StoneAssemblies.Ecoflow.Services.Interfaces;

public class SyncAccountCommand : AsyncCommand<SyncAccountCommand.Settings>
{
    private readonly IAnsiConsole ansiConsole;
    private readonly IAccountManager accountManager;
    private readonly IDeviceManager deviceManager;

    private readonly IEcoflowClientService ecoflowClientService;

    public class Settings : CommandSettings
    {
        [CommandOption("-a|--account <ACCOUNT>")]
        [Description("The EcoFlow account")]
        public string Account { get; init; } = string.Empty;

        [CommandOption("-k|--accessKey <ACCESS_KEY>")]
        [Description("The EcoFlow access key")]
        public string AccessKey { get; init; } = string.Empty;

        [CommandOption("-s|--secretKey <SECRET_KEY>")]
        [Description("The EcoFlow secret key")]
        public string SecretKey { get; init; } = string.Empty;

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(this.Account))
            {
                return ValidationResult.Error("Account cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(this.AccessKey))
            {
                return ValidationResult.Error("AccessKey cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(this.SecretKey))
            {
                return ValidationResult.Error("SecretKey cannot be empty");
            }

            return ValidationResult.Success();
        }
    }

    public SyncAccountCommand(IAnsiConsole ansiConsole, IAccountManager accountManager, IDeviceManager deviceManager)
    {
        ArgumentNullException.ThrowIfNull(ansiConsole);
        ArgumentNullException.ThrowIfNull(accountManager);
        ArgumentNullException.ThrowIfNull(deviceManager);

        this.ansiConsole = ansiConsole;
        this.accountManager = accountManager;
        this.deviceManager = deviceManager;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        this.ansiConsole.MarkupLine("[yellow]Synchronizing EcoFlow account...[/]");

        var table = new Table().Border(TableBorder.Rounded)
            .AddColumn("Field")
            .AddColumn("Value")
            .AddRow("Access Key", settings.AccessKey)
            .AddRow("Secret Key", settings.SecretKey);

        this.ansiConsole.Write(table);

        var deviceTable = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("DeviceName")
            .AddColumn("ProductName")
            .AddColumn("SerialNumber");

        await this.accountManager.InitAsync(settings.Account, settings.AccessKey, settings.SecretKey);
        await foreach (var device in this.deviceManager.SyncDevicesAsync(settings.Account).WithCancellation(cancellationToken))
        {
            deviceTable.AddRow(device.DeviceName, device.ProductName, device.SerialNumber);
        }

        this.ansiConsole.Write(deviceTable);
        this.ansiConsole.MarkupLine("[green]Account sync successfully![/]");

        return 0;
    }
}