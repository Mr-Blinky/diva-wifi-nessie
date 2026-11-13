# Diva WiFi Nessie

**Unofficial compatibility port of Diva WiFi for OpenSimulator 0.9.3.1 Nessie Dev (.NET 8).**

Diva WiFi provides a web-based interface for grid administration and user-facing services. This project retains the original Diva module structure and web interface, with targeted changes to compile and run on a modern OpenSimulator source tree.

> **Status: v0.1.0 candidate.** Tested against OpenSimulator Nessie Dev commit `58c211829b`, .NET SDK `8.0.426`, Windows, and MariaDB `11.8`. This is not an official Diva or OpenSimulator release.

## Supported database

**MySQL/MariaDB only in this revision.** The `Diva.Data.MySQL` provider has been built and exercised against MariaDB. A `Diva.Data.SQLite` project definition is present, but SQLite has **not** been built, validated, or supported in this release. Do not treat the existence of the project as evidence of SQLite compatibility.

## Verified functionality

The test deployment started successfully under ROBUST, with WiFi discovered through Mono.Addins. A clean addin-cache and `WifiPages` extraction test succeeded. The admin login, inventory listing, user search/listing, groups, region hyperlinks, and console page loading were exercised. The seven required runtime DLLs and five localization satellite assemblies were built; deployed runtime files matched the build staging hashes.

**Not yet validated:** account creation, password recovery, remote console command execution, localized page behavior, SQLite, Linux, other OpenSimulator commits, and a clean external-checkout build. These features should not be assumed working merely because the UI loads.

## Installation overview

1. Build the source modules against the matching OpenSimulator checkout as described in [BUILDING.md](BUILDING.md), or use a binary package built for the same revision.
2. Back up the ROBUST installation, configuration, and database before deployment. Test on a non-production copy first.
3. Install the seven required Diva DLLs and accompanying resources in ROBUST's `bin` directory (see [BUILDING.md](BUILDING.md)).
4. Configure the database service providers and `[WifiService]` in the active `Robust.HG.ini` (see [BUILDING.md](BUILDING.md)).
5. Start ROBUST and check its logs for WiFi addin discovery and initialization. Test the web interface before returning production services to use.

Do **not** add a native `[ServiceList] WifiServiceConnector` entry: in the tested setup WiFi is discovered by Mono.Addins when enabled.

## Source layout

The six original module directories are retained beneath `addon-modules/`: `00Data`, `00DivaInterfaces`, `01DivaUtils`, `1DivaOpenSimServices`, `20WifiScriptEngine`, and `21Wifi`. Generated `.csproj` files are intentionally excluded; OpenSimulator Prebuild generates them.

## Roadmap (ideas, not commitments)

- Validate and, if necessary, port SQLite support.
- Exercise account registration, password recovery, localization, and remote console execution.
- Test against additional Nessie revisions and operating systems.
- Investigate a separate **unofficial Diva D3 Nessie standalone** distribution, potentially released as a binary archive and/or a separate repository. This would require dedicated integration and regression testing.

## Credits and licensing

Original Diva WiFi and related code are credited to their respective authors and contributors, including Crista Lopes (Diva Canto), Marck, and OpenSimulator contributors. This compatibility port is independent and unofficial. Original file-level copyright notices are preserved. The original web footer credits its design to Crista Lopes and Marck and links to Creative Commons Attribution 3.0. See [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) for known notices and outstanding licence-verification work.
