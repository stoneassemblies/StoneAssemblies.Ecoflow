// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BatteryInfoCommand.cs" company="StoneAssemblies">
// Copyright © 2023 - 2026 StoneAssemblies Development Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;
using System.Text.Json;

using Spectre.Console;
using Spectre.Console.Cli;

using StoneAssemblies.Ecoflow.Cmd.Services;
using StoneAssemblies.Ecoflow.Cmd.Services.Interfaces;

public class BatteryInfoCommand : AsyncCommand<BatteryInfoCommand.Settings>
{
    private readonly IAnsiConsole ansiConsole;
    private readonly IDeviceManager deviceManager;

    public class Settings : CommandSettings
    {
        [CommandOption("-d|--deviceName <DEVICE_NAME>")]
        [Description("Device name of the EcoFlow device")]
        public string DeviceName { get; init; } = string.Empty;

        [CommandOption("-a|--account <ACCOUNT>")]
        [Description("Optional EcoFlow account name")]
        public string? Account { get; init; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(this.DeviceName))
            {
                return ValidationResult.Error("DeviceName cannot be empty");
            }

            return ValidationResult.Success();
        }
    }

    public BatteryInfoCommand(IAnsiConsole ansiConsole, IDeviceManager deviceManager)
    {
        ArgumentNullException.ThrowIfNull(ansiConsole);
        ArgumentNullException.ThrowIfNull(deviceManager);

        this.ansiConsole = ansiConsole;
        this.deviceManager = deviceManager;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        try
        {
            this.ansiConsole.MarkupLine($"[bold yellow]Fetching battery metrics for device:[/] [cyan]{settings.DeviceName}[/] (Account: [green]{settings.Account ?? "default"}[/])");

            var quote = await this.deviceManager.GetDeviceQuoteAsync(settings.DeviceName, settings.Account);
            using var doc = JsonDocument.Parse(quote!);

            var data = doc.RootElement.GetProperty("data");

            var soc = data.GetProperty("pd.soc").GetInt32();
            var soh = data.GetProperty("bms_bmsInfo.soh").GetInt32();
            var cycles = data.GetProperty("bms_bmsInfo.bsmCycles").GetInt32();
            var voltage = data.GetProperty("bms_bmsStatus.vol").GetInt32() / 1000.0;
            var current = data.GetProperty("bms_bmsStatus.amp").GetInt32() / 1000.0;
            var tempMin = data.GetProperty("bms_bmsStatus.minCellTemp").GetInt32();
            var tempMax = data.GetProperty("bms_bmsStatus.maxCellTemp").GetInt32();

            var table = new Table()
                .AddColumn("Metric")
                .AddColumn("Value")
                .AddRow("Unit Type", "Main Unit")
                .AddRow("SoC", $"{soc}%")
                .AddRow("SoH", $"{soh}%")
                .AddRow("Cycles", $"{cycles}")
                .AddRow("Voltage", $"{voltage:F2} V")
                .AddRow("Current", $"{current:F2} A")
                .AddRow("Temp (min)", $"{tempMin} °C")
                .AddRow("Temp (max)", $"{tempMax} °C");

            this.ansiConsole.Write(table);

            string[] slaveKeys = ["bms_slave", "bms_slave1", "bms_slave2"];

            var count = 1;
            foreach (var key in slaveKeys)
            {
                if (data.TryGetProperty($"{key}.sn", out var slaveData) && !string.IsNullOrWhiteSpace(slaveData.GetString()))
                {
                    var slaveSn = data.GetProperty($"{key}.sn").GetString();
                    var slaveSoc = data.GetProperty($"{key}.soc").GetInt32();
                    var slaveSoh = data.GetProperty($"{key}.soh").GetInt32();
                    var slaveCycles = data.GetProperty($"{key}.bsmCycles").GetInt32();
                    var slaveVol = data.GetProperty($"{key}.vol").GetInt32() / 1000.0;
                    var slaveAmp = data.GetProperty($"{key}.amp").GetInt32() / 1000.0;

                    var slaveTempMin = data.GetProperty($"{key}.minCellTemp").GetInt32();
                    var slaveTempMax = data.GetProperty($"{key}.maxCellTemp").GetInt32();

                    var slaveTable = new Table()
                        .AddColumn("Metric")
                        .AddColumn("Value")
                        .AddRow("Unit Type", $"Extra Battery #{count++}")
                        .AddRow("Serial number", slaveSn ?? "N/A")
                        .AddRow("SoC", $"{slaveSoc}%")
                        .AddRow("SoH", $"{slaveSoh}%")
                        .AddRow("Cycles", $"{slaveCycles}")
                        .AddRow("Voltage", $"{slaveVol:F2} V")
                        .AddRow("Current", $"{slaveAmp:F2} A")
                        .AddRow("Temp (min)", $"{slaveTempMin} °C")
                        .AddRow("Temp (max)", $"{slaveTempMax} °C");

                    this.ansiConsole.Write(slaveTable);
                }
            }

            this.ansiConsole.MarkupLine("[bold green]✔ Metrics extraction completed successfully[/]");
        }
        catch (Exception ex)
        {
            this.ansiConsole.MarkupLine("[bold red]✖ Error while extracting metrics[/]");
            this.ansiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
        }

        return 0;
    }
}