StoneAssemblies.Ecoflow
===============

The ultimate dotnet API to manage Ecoflow devices ;)

Build Status
------------
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=stoneassemblies_StoneAssemblies.Ecoflow&metric=alert_status)](https://sonarcloud.io/dashboard?id=stoneassemblies_StoneAssemblies.Ecoflow)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=stoneassemblies_StoneAssemblies.Ecoflow&metric=ncloc)](https://sonarcloud.io/dashboard?id=stoneassemblies_StoneAssemblies.Ecoflow)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=stoneassemblies_StoneAssemblies.Ecoflow&metric=coverage)](https://sonarcloud.io/dashboard?id=stoneassemblies_StoneAssemblies.Ecoflow)

Branch | Status
------ | :------:
master | [![Build Status](https://dev.azure.com/alexfdezsauco/External%20Repositories%20Builds/_apis/build/status%2Fstoneassemblies.StoneAssemblies.Ecoflow?branchName=master)](https://dev.azure.com/alexfdezsauco/External%20Repositories%20Builds/_build/latest?definitionId=24&branchName=master)
develop | [![Build Status](https://dev.azure.com/alexfdezsauco/External%20Repositories%20Builds/_apis/build/status%2Fstoneassemblies.StoneAssemblies.Ecoflow?branchName=develop)](https://dev.azure.com/alexfdezsauco/External%20Repositories%20Builds/_build/latest?definitionId=24&branchName=develop)

# Contributions

This is an open-source project, so your contributions are welcome. You can get in touch by:

- Creating tickets.
- Contributing by pull requests.
- or just [buying me a coffee](https://qvapay.com/payme/alexander.fernandez.sauco) :wink:

# StoneAssemblies.Ecoflow Example Applications

## GWEN — EcoFlow CLI

GWEN is a command-line tool to query EcoFlow device information using the EcoFlow Developer Program API.

**The name comes from:**
G.W.E.N. — Gathers What EcoFlow Neglects

The idea behind the name was inspired by a scene from The Amazing Spider‑Man, where the teacher tells Peter: “Don’t make promises you can’t keep,” and he quietly replies, “But those are the best kind.” It’s a moment about expectations, trust, and the things we remember when someone doesn’t follow through.

EcoFlow made one of those promises — the promise that you’d be able to see basic battery information like cycles and SoH directly from their app. But that feature never arrived. The option is there, the button exists, but the data never shows up. A promise left hanging.

So GWEN was born from that gap.
If EcoFlow won’t show the information, then GWEN will gather what EcoFlow neglects.
It’s a small nod to that Spider‑Man moment: when someone doesn’t keep a promise, you remember it — and sometimes you build something better because of it.

GWEN shows what the official app does not show.

### Install
1) Download the latest version from the [releases](https://github.com/stoneassemblies/StoneAssemblies.Ecoflow/releases) page depending on your operating system.

2) Extract the executable into a directory of your choice, for example into `C:\Apps\ecoflow`

(Optional) Add that directory to your system PATH so you can run gwen from anywhere.

If you select a non-self-contained asset, you must install the .NET runtime first.

### Usage
gwen [command] [options]

### Commands

**Sync Account**
To sync your account with the EcoFlow Developer Program, replace the placeholders with your actual values:

- `ACCOUNT`: the alias you want to use for your account
- `API_KEY`: your EcoFlow API key
- `SECRET_KEY`: your EcoFlow secret key

> gwen sync --account ACCOUNT --api-key API_KEY --secret-key SECRET_KEY

For example, using the account alias alexfdezsauco:

> gwen sync --account alexfdezsauco --api-key YOUR_API_KEY --secret-key YOUR_SECRET_KEY

This stores your credentials and lists your registered devices.

**Battery Info**
To query battery cycles, SoH, voltage, current, and temperatures:

> gwen battery info --deviceName DEVICE_NAME --account ACCOUNT

If the device name contains spaces, use quotes:

> gwen battery info --deviceName "EcoFlow Delta 2 Max" --account alexfdezsauco

_NOTE: Extra Batteries_
If your unit has extra batteries connected, they will be displayed automatically.

Help
To see all available commands:

> gwen --help
