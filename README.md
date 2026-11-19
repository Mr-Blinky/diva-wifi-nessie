# Diva WiFi Nessie

**Unofficial compatibility port of Diva WiFi for OpenSimulator 0.9.3.1 Nessie Dev (.NET 8).**

Diva WiFi provides a web-based interface for grid administration and user-facing services. This project retains the original Diva module structure and web interface, with targeted changes to compile and run on a modern OpenSimulator source tree.

> **Status: v0.9.0 RC1 release candidate (MySQL/MariaDB), plus an experimental PostgreSQL provider on the `feature/pgsql-provider` branch.** RC1 was tested against OpenSimulator Nessie Dev commit `58c211829b`, .NET SDK `8.0.426`, Windows, and MariaDB `11.8`. The PostgreSQL provider was tested on Ubuntu Linux against OpenSimulator master at commit `40bdcdccd2` or later. This is not an official Diva or OpenSimulator release.

## Supported databases

- **MySQL/MariaDB:** supported. The `Diva.Data.MySQL` provider has been built and exercised against MariaDB.
- **PostgreSQL:** experimental. `Diva.Data.PGSQL` has been built and run on a live Ubuntu grid (ROBUST, WiFi, simulators and viewer login). It requires OpenSimulator master at or after commit `40bdcdccd2`; see [Lineage](#lineage).
- **SQLite:** not supported. `Diva.Data.SQLite` has been ported to `System.Data.SQLite` and builds, but it has not been validated. Its schema also lacks the `TOS` column on `GridUser` that WiFi uses. Do not treat the existence of the project as evidence of SQLite compatibility.

## Verified functionality

The .NET 8/Nessie binary package was installed successfully on a clean copy of an existing ROBUST installation. WiFi loaded through Mono.Addins, extracted its web pages and served the interface without compiling anything during installation.

User and administrator interface tests succeeded, including new avatar creation and viewer login, account details and management, inventory, groups, user search/listing, password recovery via SMTP (delivery, link and password update), and web/REST console commands. Remote console access was then disabled in the test configuration.

SMTP uses `SmtpHost`, `SmtpPort`, `SmtpUsername` and `SmtpPassword` in `[WifiService]`. Password reset was verified against an internal SMTP server on port 25 without TLS. Consult [INSTALL.md](INSTALL.md) for generic configuration examples and SMTP port/encryption behaviour.

Explicit Hypergrid region linking was tested through WiFi against two OSGrid regions, including link creation, remote map-image retrieval, WiFi listing, in-world map search and link deletion. Normal Hypergrid teleports are separate from these persistent grid-service links. The earlier DivaData invalid-cast warnings stopped after all three storage providers were configured.

Linux was validated with the PostgreSQL provider on Ubuntu, with ROBUST and the simulator on .NET 8, WiFi served over HTTPS through an nginx reverse proxy, and viewer login in world.

**Not yet separately validated:** localized page behavior, SQLite, MySQL/MariaDB on Linux, and other OpenSimulator commits. A one-off `System.ObjectDisposedException` during ROBUST HTTP-listener shutdown appears to originate in upstream OpenSimulator; further investigation is deferred unless it recurs.

## Installation overview

1. Build the source modules against the matching OpenSimulator checkout as described in [BUILDING.md](BUILDING.md), or use a binary package built for the same revision.
2. Back up the ROBUST installation, configuration, and database before deployment. Test on a non-production copy first.
3. Install the seven required Diva DLLs and accompanying resources in ROBUST's `bin` directory (see [BUILDING.md](BUILDING.md)).
4. Configure the three database service providers, `[WifiService]` and optional SMTP settings in the active `Robust.HG.ini` (see [INSTALL.md](INSTALL.md)).
5. Start ROBUST and check its logs for WiFi addin discovery and initialization. Test the web interface before returning production services to use.

For installations launched from the ROBUST directory, the recommended `[WifiService]` setting `UserDocsPath = "."` keeps editable web pages inside the current working directory. The original Diva fallback `..` stores them one level above it. See [INSTALL.md](INSTALL.md) for details.

Do **not** add a native `[ServiceList] WifiServiceConnector` entry: in the tested setup WiFi is discovered by Mono.Addins when enabled.

## Source layout

The six original module directories are retained beneath `addon-modules/`: `00Data`, `00DivaInterfaces`, `01DivaUtils`, `1DivaOpenSimServices`, `20WifiScriptEngine`, and `21Wifi`. Generated `.csproj` files are intentionally excluded; OpenSimulator Prebuild generates them.

## Roadmap (ideas, not commitments)

- Validate SQLite support and add a `GridUser` migration for the `TOS` column.
- Widen PostgreSQL testing beyond a single grid, then drop the experimental label.
- Review and verify the OpenSimulator/ROBUST version and runtime details shown in the WiFi interface. `WifiScriptFace.Version` still uses `OpenSim.VersionInfo.Version`, but the legacy `GetRuntimeInformation()` call was replaced with `.NET RuntimeInformation.FrameworkDescription` during the Nessie port. Restore or improve the original information display where appropriate, using Nessie-compatible APIs and testing the rendered page.
- Verify localization, and revisit the upstream shutdown exception if it recurs.
- Test against additional Nessie revisions, and MySQL/MariaDB on Linux.
- Investigate a separate **unofficial Diva D3 Nessie standalone** distribution, potentially released as a binary archive and/or a separate repository. This would require dedicated integration and regression testing.

## Lineage

Diva WiFi has passed through several hands. Each step kept the original module structure and web interface while moving it onto a newer platform.

| Stage | Who | Platform | Last release |
|---|---|---|---|
| Original Diva WiFi and Diva Distro | Crista Lopes (Diva Canto) and contributors | OpenSimulator up to 0.9.2.1, Mono | WiFi `wifi-09000` for OpenSim 0.9.0 (16 Aug 2017); Diva Distro `diva-r09210` for OpenSim 0.9.2.1 (24 Oct 2022). See the [Metaverse Ink downloads page](https://metaverseink.com/Downloads.html). |
| .NET 8 compatibility port (this repository) | Mr-Blinky | OpenSimulator 0.9.3.1 Nessie Dev, .NET 8, Windows, MySQL/MariaDB | v0.9.0 RC1 |
| PostgreSQL provider and Linux testing (`feature/pgsql-provider`) | Robyn Starbrook | OpenSimulator master, .NET 8, Ubuntu Linux, PostgreSQL | Experimental |

The original downloads page lists WiFi as "discontinued as of OpenSim 0.8.1" as a standalone package. Its last release targets OpenSim 0.9.0, and Diva Distro's last release dates from 2022. These later stages exist to keep WiFi running on current OpenSimulator.

**PostgreSQL provider notes.** `Diva.Data.PGSQL` builds on OpenSimulator's own `OpenSim.Data.PGSQL` classes. It requires OpenSimulator master at or after commit `40bdcdccd2` (2026-11-16), which includes upstream PostgreSQL connector fixes for column-name quoting and type casting. Earlier OpenSimulator releases are not supported with this provider. It has been run on a live Ubuntu grid with ROBUST, WiFi, simulators and viewer login. Treat it as experimental until it has been tested more widely.

## Credits and licensing

Original Diva WiFi and related code are credited to their respective authors and contributors, including Crista Lopes (Diva Canto), Marck, and OpenSimulator contributors. This compatibility port is independent and unofficial. Original file-level copyright notices are preserved. The original web footer credits its design to Crista Lopes and Marck and links to Creative Commons Attribution 3.0. See [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) for known notices and outstanding licence-verification work.

[Personal Note from the Author (Mr-Blinky)](Personal%20note%20from%20the%20author.md)
