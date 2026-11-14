# Diva WiFi Nessie

**Unofficial compatibility port of Diva WiFi for OpenSimulator 0.9.3.1 Nessie Dev (.NET 8).**

A personal note from the author:

I have to be honest, and in full disclosure, I did not write a single line of code for this port, ChatGPT did all of it. That said, the whole project was not just ChatGPT's work. I spent around 14 hours over 3 days holding ChatGPT's reins and steering it in the right direction. All console work, connands, ini file edits and so on was all done by myself, taking great care to ensure the code was proper and nothing untoward. Extensive testing was done at each and every part of the project. Several test grids and database dumps were taken over the course. Installation and all of the testing were always done by myself. This wasn't so much as a vibe coding project as much as it was a partnership between a grid admin (and lousy programmer) and a good programmer. ChatGPT cooked and sliced the meat, I prepared the bread, butter, seasoning and salad, and put it all together.

Why did I do this? Simple, I have a ROBUST grid, I wanted a web front end. I also like Diva Wifi. It is simple and customizable, and lightweight too. It plugs right into ROBUST and the database. It requires no additional work to run, simply download the binary release, copy over the files, edit robust.ini or robust.hg.ini (don't forget to put the storage provider under the three sections, not just one!) and start up ROBUST. I also did this because I love OpenSimulator and a lot of the community. I figured if I could have this then everyone could. This shouldn't have been like this, my first prompt to ChatGPT was "Hey, go find me diva wifi and give me instructions to installing it", and it came back with something like "Found it! But this won't work with Nessie. We can port it over if you want!", and then it grew legs. 

Diva WiFi provides a web-based interface for grid administration and user-facing services. This project retains the original Diva module structure and web interface, with targeted changes to compile and run on a modern OpenSimulator source tree.

> **Status: v0.9.0 RC1 release candidate (MySQL/MariaDB only).** Tested against OpenSimulator Nessie Dev commit `58c211829b`, .NET SDK `8.0.426`, Windows, and MariaDB `11.8`. This is not an official Diva or OpenSimulator release.

## Supported database

**MySQL/MariaDB only in this revision.** The `Diva.Data.MySQL` provider has been built and exercised against MariaDB. A `Diva.Data.SQLite` project definition is present, but SQLite has **not** been built, validated, or supported in this release. Do not treat the existence of the project as evidence of SQLite compatibility.

## Verified functionality

The .NET 8/Nessie binary package was installed successfully on a clean copy of an existing ROBUST installation. WiFi loaded through Mono.Addins, extracted its web pages and served the interface without compiling anything during installation.

User and administrator interface tests succeeded, including new avatar creation and viewer login, account details and management, inventory, groups, user search/listing, password recovery via SMTP (delivery, link and password update), and web/REST console commands. Remote console access was then disabled in the test configuration.

SMTP uses `SmtpHost`, `SmtpPort`, `SmtpUsername` and `SmtpPassword` in `[WifiService]`. Password reset was verified against an internal SMTP server on port 25 without TLS. Consult [INSTALL.md](INSTALL.md) for generic configuration examples and SMTP port/encryption behaviour.

Explicit Hypergrid region linking was tested through WiFi against two OSGrid regions, including link creation, remote map-image retrieval, WiFi listing, in-world map search and link deletion. Normal Hypergrid teleports are separate from these persistent grid-service links. The earlier DivaData invalid-cast warnings stopped after all three storage providers were configured.

**Not yet separately validated:** localized page behavior, SQLite, Linux, and other OpenSimulator commits. A one-off `System.ObjectDisposedException` during ROBUST HTTP-listener shutdown appears to originate in upstream OpenSimulator; further investigation is deferred unless it recurs.

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

- Validate and, if necessary, port SQLite support.
- Verify localization, and revisit the upstream shutdown exception if it recurs.
- Test against additional Nessie revisions and operating systems.
- Investigate a separate **unofficial Diva D3 Nessie standalone** distribution, potentially released as a binary archive and/or a separate repository. This would require dedicated integration and regression testing.

## Credits and licensing

Original Diva WiFi and related code are credited to their respective authors and contributors, including Crista Lopes (Diva Canto), Marck, and OpenSimulator contributors. This compatibility port is independent and unofficial. Original file-level copyright notices are preserved. The original web footer credits its design to Crista Lopes and Marck and links to Creative Commons Attribution 3.0. See [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) for known notices and outstanding licence-verification work.
