// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BatteryInfoCommand.cs" company="StoneAssemblies">
// Copyright © 2023 - 2026 StoneAssemblies Development Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;
using System.Text.Json;

using Spectre.Console;
using Spectre.Console.Cli;

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

        [CommandOption("-c|--cells")]
        [Description("Show per‑cell voltage details")]
        public bool ShowCells { get; set; }

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

    private string FormatVoltage(int mv, string? deltaColor = null)
    {
        var voltage = mv / 1000.0d;

        if (!string.IsNullOrWhiteSpace(deltaColor))
        {
            return $"{deltaColor}{voltage:F3} V[/]";
        }

        return voltage switch
        {
            < 3.20 => $"[red]{voltage:F3} V[/]",
            < 3.25 => $"[yellow]{voltage:F3} V[/]",
            _ => $"[green]{voltage:F3} V[/]",
        };
    }

    private string FormatDelta(int deltaMv)
    {
        var markupColor = this.GetMarkupDeltaColor(deltaMv);
        return $"{markupColor}{deltaMv / 1000.0d:F3} V[/]";
    }

    private string GetMarkupDeltaColor(int deltaMv)
    {
        var delta = deltaMv / 1000.0d;
        return delta switch
        {
            < 0.010 => "[green]",
            < 0.020 => "[yellow]",
            < 0.030 => "[orange3]",
            _ => "[red]",
        };
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        try
        {
            this.ansiConsole.MarkupLine($"[bold yellow]Fetching battery metrics for device:[/] [cyan]{settings.DeviceName}[/] (Account: [green]{settings.Account ?? "default"}[/])");

            var quote = await this.deviceManager.GetDeviceQuoteAsync(settings.DeviceName, settings.Account);
            using var doc = JsonDocument.Parse(quote!);
            if (doc.RootElement.GetProperty("code").GetString() == "0")
            {
                var data = doc.RootElement.GetProperty("data");
                var soc = data.GetProperty("pd.soc").GetInt32();
                var soh = data.GetProperty("bms_bmsInfo.soh").GetInt32();
                var cycles = data.GetProperty("bms_bmsInfo.bsmCycles").GetInt32();
                var voltage = data.GetProperty("bms_bmsStatus.vol").GetInt32() / 1000.0;
                var current = data.GetProperty("bms_bmsStatus.amp").GetInt32() / 1000.0;
                var tempMin = data.GetProperty("bms_bmsStatus.minCellTemp").GetInt32();
                var tempMax = data.GetProperty("bms_bmsStatus.maxCellTemp").GetInt32();

                var table = new Table().Border(TableBorder.Rounded)
                    .Title("Main Unit")
                    .AddColumn("Metric")
                    .AddColumn("Value")

                    // .AddRow("Unit Type", "Main Unit")
                    .AddRow("SoC", $"{soc}%")
                    .AddRow("SoH", $"{soh}%")
                    .AddRow("Cycles", $"{cycles}")
                    .AddRow("Voltage", $"{voltage:F2} V")
                    .AddRow("Current", $"{current:F2} A")
                    .AddRow("Temp (min)", $"{tempMin} °C")
                    .AddRow("Temp (max)", $"{tempMax} °C");

                this.ansiConsole.Write(table);
                if (settings.ShowCells && data.TryGetProperty("bms_bmsStatus.cellVol", out var cellVolData))
                {
                    var cells = cellVolData.EnumerateArray()
                        .Select(x => x.GetInt32())
                        .ToArray();

                    this.PrintCellsVoltageTable(cells);
                }

                string[] slaveKeys = ["bms_slave", "bms_slave1", "bms_slave2"];

                var count = 1;
                foreach (var key in slaveKeys)
                {
                    if (data.TryGetProperty($"{key}.sn", out var slaveData) &&
                        !string.IsNullOrWhiteSpace(slaveData.GetString()))
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
                            .Title($"Extra Battery #{count} — {slaveSn}")
                            .Border(TableBorder.Rounded)
                            .AddColumn("Metric")
                            .AddColumn("Value")

                            // .AddRow("Unit Type", $"Extra Battery #{count++}")
                            .AddRow("Serial number", slaveSn ?? "N/A")
                            .AddRow("SoC", $"{slaveSoc}%")
                            .AddRow("SoH", $"{slaveSoh}%")
                            .AddRow("Cycles", $"{slaveCycles}")
                            .AddRow("Voltage", $"{slaveVol:F2} V")
                            .AddRow("Current", $"{slaveAmp:F2} A")
                            .AddRow("Temp (min)", $"{slaveTempMin} °C")
                            .AddRow("Temp (max)", $"{slaveTempMax} °C");

                        this.ansiConsole.Write(slaveTable);

                        if (settings.ShowCells && data.TryGetProperty($"{key}.cellVol", out var cellSlaveVolData))
                        {
                            var cells = cellSlaveVolData
                                .EnumerateArray()
                                .Select(x => x.GetInt32())
                                .ToArray();

                            if (cells.Length == 0)
                            {
                            }
                            else
                            {
                                this.PrintCellsVoltageTable(cells);
                            }
                        }
                    }
                }

                this.ansiConsole.MarkupLine("[bold green]Metrics extraction completed successfully[/]");
            }
            else
            {
                this.ansiConsole.MarkupLine($"[bold red] Error while requesting device data. Details: {doc.RootElement.GetProperty("message").GetString()}[/]");
            }
        }
        catch (Exception ex)
        {
            this.ansiConsole.MarkupLine("[bold red]Error while extracting metrics[/]");
            this.ansiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
        }

        return 0;
    }

    private void PrintCellsVoltageTable(int[] cells)
    {
        if (cells.Length == 0)
        {
            this.ansiConsole.MarkupLine(
                "[yellow] \u26a0\ufe0f Cell voltage data is currently unavailable.[/]\n" +
                "[yellow] This can happen when AC power is active or when the BMS is busy.[/]\n" +
                "[yellow] Try turning off AC input/output to allow the BMS to report per-cell voltages.[/]");
            return;
        }

        var min = cells.Min();
        var max = cells.Max();
        var delta = max - min;

        var deltaColor = this.GetMarkupDeltaColor(delta);
        var minIndexes = cells
            .Select((v, i) => new { v, i })
            .Where(x => x.v == min)
            .Select(x => x.i)
            .ToHashSet();

        var maxIndexes = cells
            .Select((v, i) => new { v, i })
            .Where(x => x.v == max)
            .Select(x => x.i)
            .ToHashSet();

        var cellTable = new Table()
            .Title("Cell Voltages")
            .Border(TableBorder.Rounded)
            .AddColumn("Cell")
            .AddColumn("Voltage");

        for (var i = 0; i < cells.Length; i++)
        {
            if (maxIndexes.Contains(i) || minIndexes.Contains(i))
            {
                cellTable.AddRow($"#{i + 1}", this.FormatVoltage(cells[i], deltaColor));
            }
            else
            {
                cellTable.AddRow($"#{i + 1}", this.FormatVoltage(cells[i]));
            }
        }

        this.ansiConsole.Write(cellTable);

        var summary =
            $"Min: {this.FormatVoltage(min)}   " +
            $"Max: {this.FormatVoltage(max)}   " +
            $"Delta: {this.FormatDelta(delta)}";

        this.ansiConsole.MarkupLine(summary);
    }
}